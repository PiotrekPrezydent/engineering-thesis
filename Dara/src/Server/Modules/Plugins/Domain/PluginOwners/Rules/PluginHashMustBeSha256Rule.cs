using System.Text.RegularExpressions;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;

public class PluginHashMustBeSha256Rule : IBuisnessRule
{
    private static readonly Regex Sha256 = new("^[0-9a-fA-F]{64}$");

    private readonly PluginHash? _hash;

    public PluginHashMustBeSha256Rule(PluginHash? hash)
    {
        _hash = hash;
    }

    public string Message => "Plugin hash must be a sha-256 written as 64 hex characters";
    public bool IsBroken()
    {
        return _hash?.Value is null || !Sha256.IsMatch(_hash.Value);
    }
}
