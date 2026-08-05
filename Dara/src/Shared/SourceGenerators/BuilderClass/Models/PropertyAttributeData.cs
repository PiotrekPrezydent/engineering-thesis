namespace Dara.Shared.SourceGenerators.BuilderClass.Models;

public abstract record PropertyAttributeData();

public record CustomMethodNameAttributeData(string MethodName) : PropertyAttributeData;

public record ObsoleteMethodOnRepeatedTypeAttributeData(TypeData NonRepeatableTypeData) : PropertyAttributeData;