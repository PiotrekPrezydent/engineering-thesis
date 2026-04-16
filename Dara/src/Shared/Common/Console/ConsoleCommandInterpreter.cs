using System.Reflection;
using Dara.Shared.Common.Extensions;

namespace Dara.Shared.Common.Console;

public class ConsoleCommandInterpreter
{
    Dictionary<string, Func<string[], Task>> _handlers = new();

    public ConsoleCommandInterpreter()
    {
        
    }

    public void BindObjectCommands(object commandsObject)
    {
        var commandObjectType = commandsObject.GetType();
        var commandMethods = commandObjectType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(m => m.CustomAttributes.Any(a => a.AttributeType == typeof(ConsoleCommand)));
        
        foreach (var commandMethod in commandMethods)
        {
            ConsoleCommand command = commandMethod.GetCustomAttribute(typeof(ConsoleCommand)) as ConsoleCommand;
            _handlers.Add(command.CommandName, args => SafeExecute(commandMethod,commandsObject, args));
        }
    }
    
    public void Handle(string consoleLine)
    {
        var split = consoleLine.Split(' ');

        string commandName = split[0];
        string[] commandArgs = split.Skip(1).ToArray();
        
        if (_handlers.TryGetValue(commandName, out var handler))
            handler(commandArgs);
        else
            System.Console.WriteLine($"Command {commandName} not found");
    }
    
    //this should be awaiting invoke
    Task SafeExecute(MethodInfo method, object instance, params object[] args)
    {
        if (method.CanBeCalledWithArgs(args))
        {
            method.Invoke(instance, args);
        }
        else
        {
            var providenArgs = args.ValuesWithType().ElementsToString().EnsureText();
            var expectedArgs = method.MethodParametersAsDictionary().ElementsToString().EnsureText();
            
            System.Console.WriteLine($"wrong parameters for method: {method.Name}\nExpected:\n{expectedArgs}\nBut provided\n{providenArgs}");
        }
        
        return Task.CompletedTask;
    }
}