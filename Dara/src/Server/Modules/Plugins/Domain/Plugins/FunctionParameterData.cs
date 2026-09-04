namespace Dara.Server.Modules.Plugins.Domain.Plugins;

public record FunctionParameterData(
    TypeData TypeData,
    string Name,
    string Description,
    bool HasDefaultValue,
    string? DefaultValueText)
{
    public static FunctionParameterData CreateRequiredParameter(TypeData typeData,
        string name,
        string description = "")
    {
        return new FunctionParameterData(typeData, name, description, false, null);
    }

    public static FunctionParameterData CreateOptionalParameter(TypeData typeData,
        string name,
        string defaultValueText,
        string description = "")
    {
        return new FunctionParameterData(typeData, name, description, false, defaultValueText);
    }
}