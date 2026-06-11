namespace Dara.Server.Modules.Plugins.Domain.PluginOwners;

// public class PluginOwner : Entity, IAggregateRoot
// {
//     public bool IsActive { get; set; }
//
//     public List<PluginId> Plugins;
//
//     internal PluginOwner(PluginOwnerId pluginOwnerId, string name) : base(pluginOwnerId)
//     {
//         IsActive = false;
//
//         Plugins = new();
//     }
//
//     public static PluginOwner Create(PluginOwnerId pluginOwnerId, string name)
//     {
//         return new(pluginOwnerId, name);
//     }
//
//     public void AddPlugin()
//     {
//         var plugin = Plugin.Create(Id, "", new List<PluginFunction>());
//         Plugins.Add(plugin.Id);
//     }
//
//     public void RemovePlugin(PluginId pluginId)
//     {
//         Plugins.Remove(pluginId);
//     }
//     
// }