namespace Dara.Server.Modules.Plugins.Domain.Plugins;

public record TypeData(string Name, bool IsNullable, bool IsArray, IReadOnlyList<TypeData> Properties, IReadOnlyList<string> EnumValues)
{
    public bool IsPrimitive => Properties.Count == 0 && EnumValues.Count == 0;
    
    public bool IsEnum => EnumValues.Count > 0;
    
    public static TypeData CreatePrimitive(string name,
        bool isNullable = false,
        bool isArray = false)
    {
        return new TypeData(name,isNullable, isArray, new List<TypeData>().AsReadOnly(), new List<string>().AsReadOnly());
    }

    public static TypeData CreateReferenced(string name,
        IEnumerable<TypeData> properties,
        bool isNullable = false,
        bool isArray = false)
    {
        return new TypeData(name, isNullable, isArray, properties.ToList(), new List<string>().AsReadOnly());
    }

    public static TypeData CreateEnum(string name,
        IEnumerable<string> enumValues,
        bool isNullable = false,
        bool isArray = false)
    {
        return new TypeData(name, isNullable, isArray, new List<TypeData>().AsReadOnly(), enumValues.ToList());
    }
}