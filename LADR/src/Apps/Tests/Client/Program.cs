using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.SignalR.Client;

namespace LADR.Tests.Client;

class Program
{
static async Task Main(string[] args)
        {
            string serverUrl = "http://127.0.0.1:5000/chat"; 
            
            var connection = new HubConnectionBuilder()
                .WithUrl(serverUrl)
                .WithAutomaticReconnect()
                .Build();
            
            connection.On<string>("ReceiveMessage", (message) =>
            {
                Console.WriteLine($"\nserver send: {message}");
            });

            try
            {
                Console.WriteLine($"connecting {serverUrl}");
                
                await connection.StartAsync();
                Console.WriteLine("connected\n");
                
                while (true)
                {
                    Console.Write("send message (exit = close)");
                    string? input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input)) 
                        continue;
                    if (input.ToLower() == "exit")
                        break;
                    
                    await connection.InvokeAsync("SendMessageToServer", input);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nerror: {ex.Message}");
            }
            
            await connection.StopAsync();
            Console.WriteLine("closed");
            Console.ReadKey();
        }
}