namespace Dara.Server.BuildingBlocks.Domain;

public interface IBuisnessRule
{
    public string Message { get; }
    public bool IsBroken();
}
