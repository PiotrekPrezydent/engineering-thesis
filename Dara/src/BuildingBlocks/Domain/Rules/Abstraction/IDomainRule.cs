namespace Dara.BuildingBlocks.Domain.Rules.Abstraction
{
    public interface IDomainRule
    {
        public bool IsBroken();

        public string Message { get; }
    }
}