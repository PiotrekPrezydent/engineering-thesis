namespace Dara.Shared.SourceGenerators.BuilderClass.Methods;

public interface IBuilderMethodData;

public record BaseMethodData(string MethodName,string PropertyName, string ParameterType, string ParameterName, bool AddObsoleteAttribute = false) : IBuilderMethodData;

public record GenericMethodData(string ArgumentsDeclaration, string WhereStatement, BaseMethodData BaseMethodData) : IBuilderMethodData;

public record CollectionMethodData(string CollectionParameterName, BaseMethodData BaseMethodData) : IBuilderMethodData;