using System.Net.Sockets;
using System.Text.Json;

namespace ServerLib;

class ClientObject
{
    protected internal string Id { get;} = Guid.NewGuid().ToString();
    protected internal StreamWriter Writer { get;}
    protected internal StreamReader Reader { get;}
 
    TcpClient client;
    ServerObject server;

    public ClientObject(TcpClient tcpClient, ServerObject serverObject)
    {
        client = tcpClient;
        server = serverObject;
        var stream = client.GetStream();
        Reader = new StreamReader(stream);
        Writer = new StreamWriter(stream);
    }
 
    public async Task ProcessAsync()
    {
        try
        {
            string? message = await Reader.ReadLineAsync();
            Message receivedMessage = JsonSerializer.Deserialize<Message>(message);
            userName = receivedMessage.Sender;
            receivedMessage.Text = $"{userName} вошел в чат";
            receivedMessage.Time = DateTime.Now.ToShortTimeString();
            message = JsonSerializer.Serialize(receivedMessage);
            await server.BroadcastMessageAsync(message, Id);
            Console.WriteLine(message);
            while (true)
            {
                try
                {
                    message = await Reader.ReadLineAsync();
                    if (message == null) continue;
                    receivedMessage = JsonSerializer.Deserialize<Message>(message);
                    receivedMessage.Time = DateTime.Now.ToShortTimeString();
                    message = JsonSerializer.Serialize(receivedMessage);
                    await server.BroadcastMessageAsync(message, Id);
                    Console.WriteLine(message);
                }
                catch
                {
                    Message sendMessage = new Message(userName, $"{userName} покинул чат", DateTime.Now.ToShortTimeString(), true);
                    message = JsonSerializer.Serialize(sendMessage);
                    await server.BroadcastMessageAsync(message, Id);
                    Console.WriteLine(message);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            server.RemoveConnection(Id);
        }
    }
    protected internal void Close()
    {
        Writer.Close();
        Reader.Close();
        client.Close();
    }
}