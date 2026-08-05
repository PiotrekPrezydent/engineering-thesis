using System;

namespace Dara.Shared.SourceGenerators.Attributes;

public record AttributeParameterData(Type ParameterType, string ParameterName, bool IsOptional = false)
{
    public object DefaultValue { get; set; }
}