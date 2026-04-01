using System.Net.Sockets;
using System.Text;

namespace LADR.Tests.Client;

public class TCPClient
{
    static void TCP(string[] args)
    {
        string serverIp = "127.0.0.1"; 
        int port = 5000;

        try
        {
            Console.WriteLine("Start connecting");
            
            using TcpClient client = new TcpClient(serverIp, port);
            Console.WriteLine("Connected.");
            
            using NetworkStream stream = client.GetStream();

            string msg = "";
            Console.WriteLine("Write message");
            msg = Console.ReadLine();

            byte[] msgBytes = Encoding.UTF8.GetBytes(msg);
            stream.Write(msgBytes, 0, msgBytes.Length);
            
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            
            Console.WriteLine(receivedMessage);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client err: {ex.Message}");
        }

        Console.WriteLine("Hello, World!");
    }
}