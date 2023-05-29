using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace ClientLib;

public class Client
{
    private StreamWriter? writer;
    private StreamReader? reader;

    private TcpClient? client;

    public bool Connected
    {
        get
        {
            return client?.Connected ?? false;
        }
    }

    public async void Connect(IPEndPoint endPoint, string username)
    {
        Close();
        client = new TcpClient();
        try
        {
            client.Connect(endPoint);
            var stream = client.GetStream();
            writer = new(stream);
            reader = new(stream);
            Message sendMessage = new Message(username, "", DateTime.Now.ToShortTimeString(), true);
            string message = JsonSerializer.Serialize(sendMessage);
            await writer.WriteLineAsync(message);
            await writer.FlushAsync();
        }
        catch (SocketException)
        {
            Close();
        }
    }
    
    public async Task SendMessageAsync(string message)
    {
        if (Connected)
        {
            await writer.WriteLineAsync(message);
            await writer.FlushAsync();
        }
    }
    
    public async Task<string?> ReceiveMessageAsync()
    {
        if (Connected)
        {
            while (true)
            {
                try
                {
                    string? message = await reader?.ReadLineAsync();
                    if (!string.IsNullOrEmpty(message)) return message;
                }
                catch
                {
                    break;
                }
            }
        }
        return null;
    }

    ~Client()
    {
        Close();
    }

    public void Close()
    {
        writer?.Close();
        reader?.Close();
        client?.Close();
    }
}