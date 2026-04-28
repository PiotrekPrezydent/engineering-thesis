using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Infrastructure;
using Dara.BuildingBlocks.Infrastructure.Abstractions;

namespace Dara.Apps.Tests.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();

            builder.Services.AddSingleton<IModuleCommandRunner, ModuleCommandRunner>();
            builder.Services.AddSingleton<IHandler<TestCommand>, TestHandler>();

            builder.Services.AddSingleton<TestApp>();
        
            
        
            var app = builder.Build();
            var test = app.Services.GetRequiredService<TestApp>();
            
            test.Test();
        
            //app.MapHub<AppHub>("/app");

            //app.MapGet("/", () => "Hello World!");
        

            app.Run();
        }
    }

    public class TestApp
    {
        private IModuleCommandRunner _commandRunner;
    
        public TestApp(IModuleCommandRunner commandRunner)
        {
            _commandRunner = commandRunner;
        }

        public void Test()
        {
            _commandRunner.ExecuteAsync(new TestCommand("ELO"));
        }
    }

    public record TestCommand(string Message) : IModuleCommand;

    public class TestHandler : IHandler<TestCommand>
    {
        public async Task HandleAsync(TestCommand request)
        {
            Console.WriteLine("HANDLER TEST");
        }
    }
}
