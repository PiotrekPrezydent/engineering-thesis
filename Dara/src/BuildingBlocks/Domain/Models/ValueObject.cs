using Dara.BuildingBlocks.Domain.Exceptions;
using Dara.BuildingBlocks.Domain.Rules.Abstraction;

namespace Dara.BuildingBlocks.Domain.Models
{
    //td add more logic for validation
    public abstract class ValueObject
    {
        protected void CheckRule(IDomainRule rule)
        {
            if (rule.IsBroken())
                throw new DomainRuleValidationException(rule);
        }
    }
}