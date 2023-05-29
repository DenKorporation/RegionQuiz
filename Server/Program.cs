using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using ServerLib;


IPEndPoint? serverIp;

bool isConnected = false;

ServerObject server = new ServerObject();

while (!isConnected)
{
    Console.WriteLine("Choose EndPoint: ");
    bool isBusy = false;
    while (!IPEndPoint.TryParse(Console.ReadLine() ?? "", out serverIp) || (isBusy =
               IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections()
                   .FirstOrDefault(tcpConnection => tcpConnection.LocalEndPoint.Equals(serverIp)) is not null ||
               IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()
                   .FirstOrDefault(tcpConnection =>
                   {
                       if (tcpConnection.Address.Equals(IPAddress.Any))
                       {
                           return tcpConnection.Port.Equals(serverIp.Port);
                       }

                       return tcpConnection.Equals(serverIp);
                   }) is not null))
    {
        if (isBusy)
        {
            Console.WriteLine("This EndPoint is busy");
            isBusy = false;
        }
        else
        {
            Console.WriteLine("Incorrect EndPoint");
        }

        Console.WriteLine("Try again");
    }

    Console.WriteLine($"server: {serverIp}. Trying start listening...");
    try
    {
        server.tcpListener = new TcpListener(serverIp);
        server.tcpListener.Start();
        Console.WriteLine("Listening started");
        isConnected = true;
    }
    catch (SocketException)
    {
        Console.WriteLine("start failed. Try again?(y/other)");
        if (Console.ReadLine() != "y")
        {
            return;
        }
    }
}

await server.ListenAsync();