namespace Dara.Server.BuildingBlocks.Application.Authorization;

internal static class ModulePermissions
{
    // one instance of the permission type for every value of its enum
    public static IReadOnlyList<IModulePermission> All(Type permissionType)
    {
        var enumType = GetEnumType(permissionType)
            ?? throw new InvalidOperationException($"permission type {permissionType.Name} must derive from ModulePermission<TPermissions>");

        if (permissionType.GetConstructor([enumType]) is null)
            throw new InvalidOperationException($"permission type {permissionType.Name} needs a public constructor that takes {enumType.Name}");

        return Enum.GetValues(enumType)
            .Cast<object>()
            .Select(value => (IModulePermission)Activator.CreateInstance(permissionType, value)!)
            .ToList();
    }

    private static Type? GetEnumType(Type permissionType)
    {
        for (var type = permissionType; type is not null; type = type.BaseType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ModulePermission<>))
                return type.GenericTypeArguments[0];
        }

        return null;
    }
}
