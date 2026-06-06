namespace Dara.BuildingBlocks.Domain.Rules
{
    public interface IDomainRule
    {
        public string Message { get; }
        public bool IsBroken();
    }
}