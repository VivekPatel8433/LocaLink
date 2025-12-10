using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Net.WebSockets;
using System.Text.Json;

public class ServerIPPort
{
    public string IP { get; set; }
    public int Port { get; set; }
    public string Name { get; set; }

    public ServerIPPort(string ip, int port, string name)
    {
        IP = ip;
        Port = port;
        Name = name;
    }

    public override string ToString()
    {
        return $"{Name} ({IP}:{Port})";
    }

    public void ConsoleOutput()
    {
        Console.WriteLine($"{Name} at {IP}:{Port}");
    }
}

public class DiscoveryClient
{
    // private UdpClient udp;
    // private IPEndPoint broadcastEP;
    private List<ServerIPPort> servers = new List<ServerIPPort>();
    private readonly int listenPort;

    public DiscoveryClient(int port)
    {
        listenPort = port;
    }

    public List<ServerIPPort> GetDiscoveredServers()
    {
        return servers;
    }

    public async Task DiscoverAsync()
    {
        var udp = new UdpClient();
        udp.EnableBroadcast = true;
        udp.Client.ReceiveTimeout = 3000; // optional timeout

        var broadcastEP = new IPEndPoint(IPAddress.Broadcast, listenPort);
        // Console.WriteLine($"Broadcasting discovery request to {broadcastEP}");
        byte[] msg = Encoding.UTF8.GetBytes("DISCOVER_REQUEST");

        // Send broadcast
        await udp.SendAsync(msg, msg.Length, broadcastEP);
        // Console.WriteLine("Broadcasted discovery request.");

        // Listen for responses (possibly multiple)
        DateTime end = DateTime.Now.AddSeconds(3);
        // Console.WriteLine($"{DateTime.Now.ToString()} => {end.ToString()}");

        servers.Clear();
        while (DateTime.Now < end)
        {
            // Console.WriteLine(DateTime.Now.ToString() + " Waiting for responses...");
            try
            {
                var receiveTask = udp.ReceiveAsync();
                var timeoutTask = Task.Delay(3000);
                var completed = await Task.WhenAny(receiveTask, timeoutTask);
                string reply = "";
                var result = default(UdpReceiveResult);
                if (completed == receiveTask)
                {
                    result = receiveTask.Result;
                    reply = Encoding.UTF8.GetString(result.Buffer);
                }
                else
                {
                    // timeout
                    // Console.WriteLine("Receive timeout.");
                    continue;
                }

                // Console.WriteLine($"Found service: {result.RemoteEndPoint} => {reply}");
                if (reply.Split(';')[0] == "DISCOVER_RESPONSE_LOCALINK")
                {
                    var parts = reply.Split(';');
                    int port = 0;
                    string name = "Unknown";
                    foreach (var part in parts)
                    {
                        if (part.StartsWith("PORT="))
                        {
                            int.TryParse(part.Substring(5), out port);
                        }
                        if (part.StartsWith("NAME="))
                        {
                            name = part.Substring(5);
                        }
                    }
                    var server = new ServerIPPort(result.RemoteEndPoint.Address.ToString(), port, name);
                    servers.Add(server);
                    // server.ConsoleOutput();
                }
            }
            catch (SocketException)
            {
                // timeout
                // Console.WriteLine("SocketException");
                break;
            }
        }
        // Console.WriteLine("Discovery finished.");
    }
}


public class JsonWebSocketClient
{
    private ClientWebSocket socket = new();
    private readonly Uri uri;
    private bool running = true;
    private string Token { get; set; } = "";

    public event Action<WsMessage>? OnMessage;

    public void SetToken(string token)
    {
        Token = token;
    }

    public string GetToken()
    {
        return Token;
    }

    public JsonWebSocketClient(string url)
    {
        uri = new Uri(url);
    }

    public async Task StartAsync()
    {
        while (running)
        {
            try
            {
                socket = new ClientWebSocket();
                // Console.WriteLine("[Client] Connecting...");
                await socket.ConnectAsync(uri, CancellationToken.None);
                // Console.WriteLine("[Client] Connected!");

                await ReceiveLoop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Client] Error: " + ex.Message);
            }

            // Console.WriteLine("[Client] Reconnecting in 3 seconds...");
            await Task.Delay(3000);
        }
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[8192]; // 8k

        while (socket.State == WebSocketState.Open)
        {
            try
            {
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                string json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var msg = JsonSerializer.Deserialize<WsMessage>(json)!;

                OnMessage?.Invoke(msg);
            }
            catch
            {
                break;
            }
        }
    }

    public async Task SendAsync(WsMessage msg)
    {
        if (socket.State != WebSocketState.Open)
            return;

        string json = JsonSerializer.Serialize(msg);
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public void Stop()
    {
        running = false;
        if (socket.State == WebSocketState.Open)
            socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Bye", CancellationToken.None);
    }
}


public static class PrivateDmHelper
{

    public static async Task<int> CreatePrivateServerAndInvite(JsonWebSocketServer callerServer, string targetUserName, string fromName)
    {
        try
        {
            // choose a random port in a safe-ish range
            var rand = new Random();
            int port = rand.Next(6001, 65000);

            // create discovery server to advertise this private DM
            var discovery = new DiscoveryServer(8888, port);
            discovery.Enable();
            _ = Task.Run(async () => await discovery.StartAsync());

            // create and start JsonWebSocketServer
            var privateServer = new JsonWebSocketServer(port)
            {
                DiscoveryRef = discovery
            };
            privateServer.Enable();
            _ = Task.Run(async () => await privateServer.StartAsync());

            // Give the server a moment to start listening (best-effort)
            await Task.Delay(250);

            // Send DM invite to the target user via callerServer
            bool sent = await callerServer.SendDmInvite(targetUserName, fromName, port);
            if (!sent)
            {
                // if invite failed, disable and stop the created server/discovery
                discovery.Disable();
                privateServer.Disable();
                return -1;
            }

            return port;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CreatePrivateServerAndInvite error: {ex.Message}");
            return -1;
        }
    }
}