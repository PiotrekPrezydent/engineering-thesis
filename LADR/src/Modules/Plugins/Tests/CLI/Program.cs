using System.Reflection;

namespace LADR.Modules.Plugins.Tests.CLI;

class Program
{
    static void Main(string[] args)
    {
        var com = ModuleInitializer.InitializeTestsCommands();
        
        var files = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Plugins", "*.dll");
        foreach (var file in files)
        {
            com.AddPlugin(file);
        }
        
        com.CreatePlugins();
        com.InvokePluginMethod("AlphaPlugin","AlphaConsoleWriteLine","JAKISTEKSTEST");
        
    }
}