using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LADR.Tests.Server;

public class TCPServer
{
    static async Task TCP(string[] args)
    {
        int port = 5000;
        
        TcpListener server = new TcpListener(IPAddress.Any, port);
        try
        {
            server.Start();
            Console.WriteLine("Server started");
            
            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("Err:" + e.Message);
        }
        finally
        {
            server.Stop();
        }
    }

    static async Task HandleClientAsync(TcpClient client)
    {
        //auto dispose
        using (client)
        using (NetworkStream stream = client.GetStream())
        {
            string endPoint = client.Client.RemoteEndPoint.ToString();
            Console.WriteLine($"Received endpoint {endPoint}");

            int pingCount = 1;
            try
            {
                while (true)
                {
                    string msg = $"Ping: {pingCount}";
                    byte[] data = Encoding.UTF8.GetBytes(msg);

                    await stream.WriteAsync(data, 0, data.Length);
                    Console.WriteLine($"Sent {client.Client.RemoteEndPoint}");

                    pingCount++;
                    await Task.Delay(500);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"SocketException: {e.Message}");
            }
            finally
            {
                Console.WriteLine("end");
            }
        }
    }
}