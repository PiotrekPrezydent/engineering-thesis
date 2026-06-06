using Dara.BuildingBlocks.Domain.Exceptions.Abstraction;
using Dara.BuildingBlocks.Domain.Rules;

namespace Dara.BuildingBlocks.Domain.Exceptions
{
    public class DomainRuleValidationException : DomainException
    {
        public IDomainRule BrokenRule { get; }
    
        public DomainRuleValidationException(IDomainRule brokenRule) : base(brokenRule.Message)
        {
            BrokenRule = brokenRule;
        }

        public override string ToString()
        {
            return $"{BrokenRule.GetType().FullName}: {BrokenRule.Message}";
        }
    }
}