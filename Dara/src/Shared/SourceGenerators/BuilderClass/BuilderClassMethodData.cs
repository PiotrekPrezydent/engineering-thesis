namespace Dara.Shared.SourceGenerators.BuilderClass;

public abstract record BuilderClassMethodData(string MethodName, string ParameterType, string ParameterName, string FieldName, string AttributeText = "");

public record SimpleBuilderClassMethodData(string MethodName, string ParameterType, string ParameterName, string FieldName, string AttributeText = "") : BuilderClassMethodData(MethodName, ParameterType, ParameterName,FieldName, AttributeText);

public record CollectionBuilderClassMethodData(string MethodName, string ParameterType, string ParameterName, string FieldName, string CollectionParameterName, bool isTypeObsolete, string AttributeText = "")  : BuilderClassMethodData(MethodName, ParameterType, ParameterName,FieldName, AttributeText);

public record GenericBuilderClassMethodData(string MethodName, string ParameterType, string ParameterName,string FieldName, string GenericParametersStatement, string WhereStatement, string AttributeText = "") : BuilderClassMethodData(MethodName, ParameterType, ParameterName, FieldName, AttributeText);

