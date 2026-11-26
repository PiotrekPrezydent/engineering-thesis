using System.Text.Json;
using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;

public class InternalCommandProcessor : IInternalCommandProcessor
{
    private readonly DbContext _context;
    private readonly IInternalCommandRepository _repository;
    private readonly IInternalCommandsTypeMapper _typeMapper;
    private readonly ICommandExecutor _commandExecutor;
    private readonly MessageProcessingOptions _options;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<InternalCommandProcessor> _logger;

    public InternalCommandProcessor(DbContext context, IInternalCommandRepository repository, IInternalCommandsTypeMapper typeMapper, ICommandExecutor commandExecutor, MessageProcessingOptions options, TimeProvider timeProvider, ILogger<InternalCommandProcessor> logger)
    {
        _context = context;
        _repository = repository;
        _typeMapper = typeMapper;
        _commandExecutor = commandExecutor;
        _options = options;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task ProcessAsync(Guid commandId, CancellationToken ct)
    {
        var command = await _repository.GetAsync(commandId, ct);
        if (command == null || command.ProcessedDate != null)
            return;

        _logger.LogDebug("STARTING INTERNAL COMMAND {CommandId} {CommandType}, attempt {Attempt}", commandId, command.Type, command.Attempts + 1);

        var type = _typeMapper.GetTypeForCommandWithTypeName(command.Type);
        var payload = JsonSerializer.Deserialize(command.Data, type) as ICommand
                      ?? throw new InvalidOperationException($"Internal command {commandId} of type {command.Type} has no data.");

        // tracked only; the unit of work of the command handler saves it together with the changes of the handler
        command.MarkProcessed(_timeProvider.GetUtcNow().UtcDateTime);

        await ExecuteAsync((dynamic)payload);

        // a handler without the unit of work decorator; nothing to save after a committed handler
        await _context.SaveChangesAsync(CancellationToken.None);

        _logger.LogDebug("PROCESSED INTERNAL COMMAND {CommandId} {CommandType}", commandId, command.Type);
    }

    public async Task RecordFailureAsync(Guid commandId, Exception error, CancellationToken ct)
    {
        var command = await _repository.GetAsync(commandId, ct);
        if (command == null || command.ProcessedDate != null)
            return;

        command.RecordFailure(error, _timeProvider.GetUtcNow().UtcDateTime, _options.Retry);
        await _context.SaveChangesAsync(ct);

        _logger.LogWarning("INTERNAL COMMAND {CommandId} {CommandType} FAILED {Attempts} TIME(S), NEXT ATTEMPT AT {NextAttemptAt:O}", commandId, command.Type, command.Attempts, command.NextAttemptAt);
    }

    // dynamic gives the runtime type of the command, so the container finds its handler
    private Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        return _commandExecutor.ExecuteAsync(command);
    }
}
