using System.Text.Json;
using Dara.Shared.Testing.Plugins;
using Microsoft.Extensions.AI;

namespace Dara.Shared.Testing.Tests;

public class CheckFunctionDeclarationTest : ITest
{
    public async Task Run(string api)
    {
        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        var funcs = new List<AIFunction>
        {
            AIFunctionFactory.Create(M1),
            AIFunctionFactory.Create(M2),
            AIFunctionFactory.Create(M3)
        };

        foreach (var func in funcs)
        {
            Console.WriteLine(JsonSerializer.Serialize(func.AsDeclarationOnly(), jsonOptions));
        }
    }


    public void M1()
    {
        
    }

    public void M2(string[] args)
    {
        
    }

    public void M3(FunctionData[] args)
    {
        
    }
    
    
}