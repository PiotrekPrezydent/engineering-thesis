using Dara.BuildingBlocks.Domain.Models;
using Dara.BuildingBlocks.Domain.Rules.Abstraction;

namespace Dara.BuildingBlocks.Domain.Rules
{
    public abstract class ValueObjectMemberRule<TValueObject, TMember> : IDomainRule where TValueObject : ValueObject
    {
        protected readonly TValueObject _valueObject;
        protected readonly TMember _givenValue;

        public ValueObjectMemberRule(TMember givenValue)
        {
            _givenValue = givenValue;
        }
        public abstract bool IsBroken();

        public string Message => $"{GetType().Name} is broken for: {_givenValue}";
    }
}