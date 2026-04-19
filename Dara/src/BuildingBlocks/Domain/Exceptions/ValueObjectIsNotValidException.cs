namespace Dara.BuildingBlocks.Domain.Exceptions;

public class ValueObjectIsNotValidException<TValueObject> : BaseDomainException where TValueObject : ValueObject
{
    public TValueObject ValueObject { get; }

    public object Value { get; }

    public string VariableName { get; }
    
    
    public ValueObjectIsNotValidException(TValueObject valueObject, string variableName, object value, string message) : base(message)
    {
        ValueObject = valueObject;
        Value = value;
        VariableName = variableName;
    }
    


    protected override string DomainExcetpionState()
    {
        string ret = "";
        
        ret += $"VO_TYPE:\t[ {ValueObject.GetType().Name} ]\n";
        ret += $"VO_PARAM:\t[ {VariableName} ]\n";
        ret += $"PARAM_VAL:\t[ {Value} ]\n";
        
        return ret;
    }
}