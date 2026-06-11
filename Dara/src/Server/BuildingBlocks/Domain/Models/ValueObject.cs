using Dara.BuildingBlocks.Domain.Exceptions;
using Dara.BuildingBlocks.Domain.Rules;

namespace Dara.BuildingBlocks.Domain.Models;

public abstract class ValueObject
{
    internal abstract void Create();
        
    protected static void CheckRule(IBuisnessRule rule)
    {
        if (rule.IsBroken())
            throw new BuisnessRuleValidationException(rule);
    }
}
