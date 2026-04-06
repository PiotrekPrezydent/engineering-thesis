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
            var asse = Assembly.LoadFile(file);
            var type = asse.ExportedTypes.First();
            var instanceObj = Activator.CreateInstance(type);
            Console.WriteLine(instanceObj == null ? "null" : instanceObj.ToString());
            com.AddPlugin(instanceObj);
            

        }
        Console.WriteLine("added plugins");
        com.PrintAllPlugins();
    }
}