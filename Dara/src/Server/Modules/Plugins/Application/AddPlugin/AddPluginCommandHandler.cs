using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

namespace Dara.Server.Modules.Plugins.Application.AddPlugin;

public class AddPluginCommandHandler : ICommandHandler<AddPluginCommand>
{
    private readonly IPluginOwnerRepository _repository;

    public AddPluginCommandHandler(IPluginOwnerRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(AddPluginCommand command)
    {
        var owner = await _repository.GetByIdAsync(new PluginOwnerId(command.PluginOwnerId));
        
        var functions = command.PluginDescriptor.Functions.Select(func => 
            PluginFunction.Create(
                func.Data.Name,
                func.Data.Description, 
                func.Data.ReturnTypeName, 
                func.Parameters.Select(p => 
                    new PluginFunctionParameter(
                        p.Name,
                        p.Description,
                        p.TypeName)
                )
            )
        );
        
        owner.AddPlugin(command.PluginDescriptor.Data.Name, command.PluginDescriptor.Data.Description, functions.ToList());
    }
}