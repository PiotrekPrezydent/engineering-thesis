using System.Reflection;
using Dara.Shared.Common.Extensions;

namespace Dara.Shared.Common.CLI;

public class CLILineInterpreter
{
    private Dictionary<string, Func<string[], Task>> _handlers;

    public CLILineInterpreter()
    {
        _handlers = new();
    }

    public void BindObjectCommands(object commandsObject)
    {
        var commandObjectType = commandsObject.GetType();
        var commandMethods = commandObjectType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(m => m.CustomAttributes.Any(a => a.AttributeType == typeof(CLICommand)));
        
        foreach (var commandMethod in commandMethods)
        {
            CLICommand command = commandMethod.GetCustomAttribute(typeof(CLICommand)) as CLICommand;
            foreach (var name in command.CommandNames)
                _handlers.Add(name, args => SafeExecute(commandMethod,commandsObject, args));
        }
    }
    
    public async Task HandleAsync(string consoleLine)
    {
        var split = consoleLine.Split(' ');

        string commandName = split[0];
        string[] commandArgs = split.Skip(1).ToArray();
        
        if (_handlers.TryGetValue(commandName, out var handler))
            await handler(commandArgs);
        else
            System.Console.WriteLine($"Command {commandName} not found");
    }
    
    //this should be awaiting invoke
    async Task SafeExecute(MethodInfo method, object instance, params object[] args)
    {
        if (method.CanBeCalledWithArgs(args))
        {
            Task t = (Task)method.Invoke(instance, args);
            await t;
        }
        else
        {
            var providenArgs = args.ValuesWithType().ElementsToString().EnsureText();
            var expectedArgs = method.MethodParametersAsDictionary().ElementsToString().EnsureText();
            
            System.Console.WriteLine($"wrong parameters for method: {method.Name}\nExpected:\n{expectedArgs}\nBut provided\n{providenArgs}");
        }
        
    }
}