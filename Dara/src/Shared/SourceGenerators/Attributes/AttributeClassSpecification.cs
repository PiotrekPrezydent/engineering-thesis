using System;
using Dara.Shared.SourceGenerators.Common;

namespace Dara.Shared.SourceGenerators.Attributes;

public record AttributeClassSpecification(
    string Namespace,
    string ClassName,
    AttributeUsageData UsageData,
    params AttributeParameterData[] ParametersDatas)
{
    public static AttributeClassSpecification GenerateBuilderAttributeSpecification { get; } = new(
        Names.AttributesNamespace, 
        Names.GenerateBuilderAttributeName, 
        new(AttributeTargets.Class)
    );

    public static AttributeClassSpecification BuilderMethodNameAttributeSpecification { get; } = new(
        Names.AttributesNamespace, 
        Names.BuilderMethodNameAttributeName, 
        new(AttributeTargets.Property), 
        [
            new(typeof(string), "MethodName")
        ]
    );
    
    public static AttributeClassSpecification ObsoleteMethodOnRepeatedTypeAttributeClassSpecification { get; } = new(
        Names.AttributesNamespace, 
        Names.ObsoleteMethodOnRepeatedTypeAttributeName, 
        new(AttributeTargets.Property),
        [
            new(typeof(Type), "NonRepeatableType")
        ]
    );
}