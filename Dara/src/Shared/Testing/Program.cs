using Dara.Shared.Testing.Tests;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace Dara.Shared.Testing;

class Program
{
    static async Task Main(string[] args)
    {
        var root = Directory.GetCurrentDirectory();
        var dotenvPath = Path.Combine(root, ".env");
        
        DotEnv.Load(dotenvPath);

        string[] apis =
        [
            Environment.GetEnvironmentVariable("API_KEY_1")!,
            Environment.GetEnvironmentVariable("API_KEY_2")!,
            Environment.GetEnvironmentVariable("API_KEY_3")!,
        ];

        ITest[] tests =
        [
            new CallFakeToolTest(),
            new CallCorrentUserToolTest()
        ];
        
        var apiSelection = apis[1];
        var testSelection = tests[1];
        
        await testSelection.Run(apiSelection);
    }
}