using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text.Json;
using System.Security.Principal;

class Test
{
    static void test(string[] args)
    {
        int discoveryPort = 8888;
        if (args.Length >= 1)
        {
            int.TryParse(args[0], out discoveryPort);
        }
        int serverPort = 6000;
        if (args.Length >= 2)
        {
            int.TryParse(args[1], out serverPort);
        }
        var server = new DiscoveryServer(discoveryPort, serverPort);
        var serverThread = new Thread(() =>
        {
            server.StartAsync().GetAwaiter().GetResult();
        });
        serverThread.IsBackground = true;
        serverThread.Start();
        var client = new DiscoveryClient(discoveryPort);
        var wsServer = new JsonWebSocketServer(serverPort);
        var wsServerThread = new Thread(async () =>
        {
            await wsServer.StartAsync();
        });
        wsServerThread.IsBackground = true;
        wsServerThread.Start();
        JsonWebSocketClient wsClient = null;
        Thread wsClientThread = null;
        // var wsClient = new JsonWebSocketClient("ws://localhost:5000/ws");
        string prompt = ">";
        Console.WriteLine("Welcome to LocaLink Console.");
        Console.WriteLine("Type 'start' to start server, 'stop' to stop server.");
        Console.WriteLine("Type 'scan' to scan for servers.");
        Console.WriteLine("Type 'join <ip>:<port>' to join a server.");
        Console.WriteLine("Type 'exit' or 'quit' to close.");

        var ips = Dns.GetHostAddresses(Dns.GetHostName());
        Console.WriteLine("Local IP Addresses:");
        foreach (var ip in ips)
        {
            Console.WriteLine(ip.ToString());
        }

        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (input == "exit" || input == "quit")
                break;
            else if (input.StartsWith("start"))
            {
                Console.WriteLine("Starting server...");
                server.Enable();
                wsServer.Enable();
                Console.WriteLine($"{server.Running}, {wsServer.Running}");
                Console.WriteLine("Server started.");
            }
            else if (input.StartsWith("stop"))
            {
                Console.WriteLine("Stopping server...");
                server.Disable();
                wsServer.Disable();
                Console.WriteLine($"{server.Running}, {wsServer.Running}");
                Console.WriteLine("Server stopped.");
            }
            else if (input.StartsWith("scan"))
            {
                Console.WriteLine("Scanning for servers...");
                var clientThread = new Thread(async () =>
                {
                    client.DiscoverAsync().GetAwaiter().GetResult();
                });
                clientThread.IsBackground = true;
                clientThread.Start();
                clientThread.Join();
                var servers = client.GetDiscoveredServers();
                foreach (var s in servers)
                {
                    Console.WriteLine($"{s.Name} => {s.IP}:{s.Port}");
                }
            }
            else if (input.StartsWith("join "))
            {
                var parts = input.Substring(5).Split(':');
                wsClient = new JsonWebSocketClient($"ws://{parts[0]}:{parts[1]}/ws");
                wsClientThread = new Thread(async () =>
                {
                    await wsClient.StartAsync();
                });
                wsClientThread.IsBackground = true;
                wsClientThread.Start();
                wsClient.OnMessage += (msg) =>
                {
                    if (msg.Type == "chat")
                    {
                        Console.WriteLine($"\b<{msg.Type}: {JsonSerializer.Serialize(msg.Data)}");
                        Console.Write(prompt);
                    }
                    else if (msg.Type == "join")
                    {
                        wsClient.SetToken(msg.Token.ToString());
                    }
                };
                Thread.Sleep(1000);
                var msg = new WsMessage
                {
                    Type = "join",
                    Data = "",
                    Name = "ConsoleUser"
                };
                wsClient.SendAsync(msg);
                Console.WriteLine($"Joining server {parts[0]}:{parts[1]} ...");
            }
            else if (input == "leave")
            {
                if (wsClient != null)
                {
                    wsClient.Stop();
                    Console.WriteLine("Left the server.");
                    wsClient = null;
                    wsClientThread = null;
                }
                else
                {
                    Console.WriteLine("Not connected to any server.");
                }
            }
            else if (input == "token")
            {
                if (wsClient != null)
                {
                    Console.WriteLine($"Token: {wsClient.GetToken()}");
                }
                else
                {
                    Console.WriteLine("Not connected to any server.");
                }
            }
            else
            {
                if (wsClient != null)
                {
                    var msg = new WsMessage
                    {
                        Type = "chat",
                        Data = input,
                        Token = wsClient.GetToken()
                    };
                    wsClient.SendAsync(msg);
                }
                else
                {
                    Console.WriteLine("Not connected to any server.");
                }
            }

            // Console.WriteLine($"You entered: {input}");
        }
    }
}