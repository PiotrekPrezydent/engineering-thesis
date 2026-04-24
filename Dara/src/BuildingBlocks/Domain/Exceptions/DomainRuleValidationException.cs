using Dara.BuildingBlocks.Domain.Exceptions.Abstraction;
using Dara.BuildingBlocks.Domain.Rules.Abstraction;

namespace Dara.BuildingBlocks.Domain.Exceptions;

public class DomainRuleValidationException : DomainException
{
    public IDomainRule BrokenRule { get; }
    
    public DomainRuleValidationException(IDomainRule brokenRule) : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
    }

    protected override string DomainExcetpionState()
    {
        return "Domain rule is broken";
    }
}