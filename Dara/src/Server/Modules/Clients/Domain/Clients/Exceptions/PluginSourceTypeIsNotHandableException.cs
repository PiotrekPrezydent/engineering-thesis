using Dara.BuildingBlocks.Domain.Exceptions;

namespace Dara.Server.Modules.Clients.Domain.Clients.Exceptions;

public class PluginSourceTypeIsNotHandableException : BaseDomainException
{
    public string SourceType { get; }
    public PluginSourceTypeIsNotHandableException(string sourceType) : base(nameof(PluginSourceTypeIsNotHandableException))
    {
        SourceType = sourceType;
    }

    public override string ToString()
    {
        return $"{SourceType}: is not handable";
    }
}