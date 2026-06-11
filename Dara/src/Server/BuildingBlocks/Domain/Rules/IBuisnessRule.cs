namespace Dara.BuildingBlocks.Domain.Rules;

public interface IBuisnessRule
{
    public string Message { get; }
    public bool IsBroken();
}
