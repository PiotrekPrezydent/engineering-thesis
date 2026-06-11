namespace Dara.Server.Modules.Plugins.Domain.Plugins;

// public class PluginFunction : ValueObject
// {
//     public string Name { get; }
//     
//     public string Description { get; }
//     
//     public IReadOnlyList<PluginFunctionParameter> Parameters { get; }
//     
//     public Type? ReturnType { get; }
//     
//     private PluginFunction(string name, string description, IReadOnlyList<PluginFunctionParameter> parameters, Type? returnType = null)
//     {
//         Name = name;
//         Description = description;
//         Parameters = parameters;
//         ReturnType = returnType;
//     }
//
//     public static PluginFunction Create(string name, string description, IReadOnlyList<PluginFunctionParameter> parameters,
//         Type? returnType = null)
//     {
//         return new PluginFunction(name, description, parameters, returnType);
//     }
//     
//     
// }