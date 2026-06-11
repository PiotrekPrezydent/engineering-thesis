namespace Dara.Server.BuildingBlocks.Domain.Exceptions;

public class BuisnessRuleValidationException : BaseDomainException
{
    public IBuisnessRule BrokenRule { get; }

    public BuisnessRuleValidationException(IBuisnessRule brokenRule) : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
    }
}