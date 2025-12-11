using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;


public class DiscoveryServer
{
    private readonly UdpClient udp;
    public string Name = "Unknown";
    public int Port = 6000;
    public bool Running = false;

    public DiscoveryServer(int listenPort, int wsPort)
    {
        udp = new UdpClient(listenPort);
        udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        Port = wsPort;
    }

    public void Enable() => Running = true;
    public void Disable() => Running = false;

    public async Task StartAsync()
    {
        while (true)
        {
            if (Running)
            {
                var result = await udp.ReceiveAsync();
                string msg = Encoding.UTF8.GetString(result.Buffer);

                if (msg == "DISCOVER_REQUEST")
                {
                    string reply = $"DISCOVER_RESPONSE_LOCALINK;PORT={Port};NAME={Name}";
                    byte[] data = Encoding.UTF8.GetBytes(reply);
                    await udp.SendAsync(data, data.Length, result.RemoteEndPoint);
                }
            }
            else
            {
                await Task.Delay(100);
            }
        }
    }
}

public class WsMessage
{
    public string Type { get; set; } = "";
    public object? Data { get; set; }
    public string? Token { get; set; }
    public string? Name { get; set; }
}

public class User
{
    public string Name { get; set; } = "Guest";
    public string Token { get; set; } = "";
    public WebSocket Socket { get; set; }

    public User(string name, string token, WebSocket socket)
    {
        Name = name;
        Token = token;
        Socket = socket;
    }
}

// ---------- NEW model for drawing stroke ----------
public class DrawingStroke
{
    public int X1 { get; set; }
    public int Y1 { get; set; }
    public int X2 { get; set; }
    public int Y2 { get; set; }
    public int ColorArgb { get; set; }
    public int Size { get; set; } = 4;
    public string User { get; set; } = "";
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}

// ---------- NEW drawing state stored in memory ----------
public class DrawingState
{
    private readonly List<DrawingStroke> strokes = new List<DrawingStroke>();
    private readonly object locker = new object();

    public void AddStroke(DrawingStroke s)
    {
        lock (locker)
        {
            strokes.Add(s);
        }
    }

    public List<DrawingStroke> GetAllStrokes()
    {
        lock (locker)
        {
            // return a shallow copy to avoid locking while clients read
            return new List<DrawingStroke>(strokes);
        }
    }

    public void Clear()
    {
        lock (locker)
        {
            strokes.Clear();
        }
    }

    // Hook to persist to DB (user will implement later)
    public void SaveToDatabase()
    {
        // For now this is a no-op; later replace with real DB logic.
        // Example: store strokes into a SQL table (StrokeId, X1, Y1, X2, Y2, ColorArgb, Size, User, Timestamp)
    }
}

public class JsonWebSocketServer
{
    public int Port { get; set; }
    public bool Running { get; set; } = false;
    private int TokenCounter = 0;
    private readonly List<User> users = new List<User>();
    private readonly string ManagerToken = "";

    public DiscoveryServer? DiscoveryRef { get; set; } = null;

    // NEW: drawing state in memory
    public DrawingState Drawing { get; private set; } = new DrawingState();

    public JsonWebSocketServer(int port)
    {
        Port = port;
    }

    public string GenerateToken()
    {
        TokenCounter++;
        return $"token_{TokenCounter}_{DateTime.UtcNow.Ticks}";
    }

    public void Enable() => Running = true;
    public void Disable() => Running = false;

    private bool IsLocalConnection(string ip)
    {
        var ips = Dns.GetHostAddresses(Dns.GetHostName());
        foreach (var localIp in ips)
        {
            if (localIp.ToString() == ip) return true;
        }
        return false;
    }

    public async Task StartAsync()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add($"http://+:{Port}/");
        listener.Start();
        Console.WriteLine($"WebSocket server started on ws://localhost:{Port}");

        while (true)
        {
            if (Running)
            {
                var ctx = await listener.GetContextAsync();
                if (!ctx.Request.IsWebSocketRequest)
                {
                    ctx.Response.StatusCode = 400;
                    ctx.Response.Close();
                    continue;
                }
                _ = HandleClientAsync(ctx);
            }
            else
            {
                await Task.Delay(100);
            }
        }
    }

    private async Task HandleClientAsync(HttpListenerContext ctx)
    {
        var clientIp = ctx.Request.RemoteEndPoint.Address.ToString();
        var wsCtx = await ctx.AcceptWebSocketAsync(null);
        var socket = wsCtx.WebSocket;

        var buffer = new byte[8192];
        string token = "";

        while (socket.State == WebSocketState.Open && Running)
        {
            var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close) break;

            string json = Encoding.UTF8.GetString(buffer, 0, result.Count);
            WsMessage msg = null;
            try
            {
                msg = JsonSerializer.Deserialize<WsMessage>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
            }
            catch
            {
                // ignore malformed
                continue;
            }

            if (msg == null) continue;

            if (msg.Type == "ping")
            {
                await SendJsonAsync(socket, new WsMessage { Type = "pong", Data = DateTime.UtcNow.ToString() });
            }

            if (msg.Type == "join")
            {
                token = GenerateToken();
                bool isLocal = IsLocalConnection(clientIp);
                Console.WriteLine($"User joining: {msg.Name}, from {clientIp}, Local Manager: {isLocal}");

                var user = new User(msg.Name ?? "Guest", token, socket);
                users.Add(user);

                if (DiscoveryRef != null && !string.IsNullOrWhiteSpace(msg.Name))
                {
                    DiscoveryRef.Name = msg.Name;
                }

                await SendJsonAsync(socket, new WsMessage
                {
                    Type = "join",
                    Data = DateTime.UtcNow.ToString(),
                    Token = token
                });

                // send user list
                await SendUserListToSocket(socket);

                // NEW: send current drawing strokes to this newly joined client
                try
                {
                    var strokes = Drawing.GetAllStrokes();
                    await SendJsonAsync(socket, new WsMessage
                    {
                        Type = "draw_load",
                        Data = strokes // will be serialized as array of strokes
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending draw_load: " + ex.Message);
                }
            }

            if (msg.Type == "chat")
            {
                await BroadcastChatAsync(socket, msg.Data?.ToString() ?? "");
            }

            // NEW: handle incoming stroke
            if (msg.Type == "draw")
            {
                // msg.Data is expected to be a JSON object representing DrawingStroke
                try
                {
                    // parse data into DrawingStroke
                    // Since WsMessage.Data is object, the deserialized type may be JsonElement
                    DrawingStroke stroke = null;
                    if (msg.Data is JsonElement je && je.ValueKind != JsonValueKind.Undefined)
                    {
                        stroke = je.Deserialize<DrawingStroke>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    else
                    {
                        // fallback: try serialize->deserialize
                        stroke = JsonSerializer.Deserialize<DrawingStroke>(JsonSerializer.Serialize(msg.Data));
                    }

                    if (stroke != null)
                    {
                        if (string.IsNullOrEmpty(stroke.User) && !string.IsNullOrEmpty(msg.Name))
                            stroke.User = msg.Name;

                        Drawing.AddStroke(stroke);

                        // Broadcast this stroke to all connected clients (including sender)
                        var payload = new WsMessage { Type = "draw", Data = stroke };
                        string payloadJson = JsonSerializer.Serialize(payload);
                        byte[] payloadBytes = Encoding.UTF8.GetBytes(payloadJson);

                        foreach (var u in users)
                        {
                            if (u.Socket.State == WebSocketState.Open)
                            {
                                try
                                {
                                    await u.Socket.SendAsync(payloadBytes, WebSocketMessageType.Text, true, CancellationToken.None);
                                }
                                catch { /* ignore send errors per-client */ }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error processing draw message: " + ex.Message);
                }
            }

            // Optional: request to persist the drawing to DB (server-side)
            if (msg.Type == "draw_save")
            {
                try
                {
                    // call Drawing.SaveToDatabase() or your custom DB updater
                    Drawing.SaveToDatabase();
                    await SendJsonAsync(socket, new WsMessage { Type = "draw_saved", Data = "ok" });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error saving drawing to DB: " + ex.Message);
                    await SendJsonAsync(socket, new WsMessage { Type = "draw_saved", Data = "error" });
                }
            }

            // dm_create handling remains unchanged
            if (msg.Type == "dm_create")
            {
                try
                {
                    var doc = JsonDocument.Parse(msg.Data?.ToString() ?? "{}");
                    string toUser = doc.RootElement.TryGetProperty("To", out var elTo) ? elTo.GetString() ?? "" : "";
                    int invitedPort = doc.RootElement.TryGetProperty("Port", out var elPort) ? (elPort.GetInt32()) : 0;
                    string fromName = msg.Name ?? "Unknown";

                    if (!string.IsNullOrEmpty(toUser) && invitedPort != 0)
                    {
                        await SendDmInvite(toUser, fromName, invitedPort);
                    }
                }
                catch
                {
                    // ignore parse errors
                }
            }
        }

        users.RemoveAll(u => u.Socket == socket);
        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Bye", CancellationToken.None);
        Console.WriteLine("Client disconnected.");
    }

    private async Task SendJsonAsync(WebSocket socket, WsMessage msg)
    {
        string json = JsonSerializer.Serialize(msg);
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        if (socket.State == WebSocketState.Open)
            await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task BroadcastChatAsync(WebSocket sender, string message)
    {
        string formattedMessage = $"Friend: {message}";
        var msg = new WsMessage { Type = "chat", Data = formattedMessage };
        string json = JsonSerializer.Serialize(msg);
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        foreach (var user in users)
        {
            if (user.Socket.State == WebSocketState.Open && user.Socket != sender)
            {
                await user.Socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    private async Task SendUserListToSocket(WebSocket socket)
    {
        try
        {
            var names = new List<string>();
            foreach (var u in users) names.Add(u.Name);
            await SendJsonAsync(socket, new WsMessage { Type = "user_list", Data = names });
        }
        catch
        {
            // ignore
        }
    }

    public async Task<bool> SendDmInvite(string toUserName, string fromName, int port)
    {
        try
        {
            var target = users.Find(u => u.Name == toUserName);
            if (target != null && target.Socket.State == WebSocketState.Open)
            {
                var payload = new { From = fromName, Port = port };
                await SendJsonAsync(target.Socket, new WsMessage { Type = "dm_invite", Data = payload });
                Console.WriteLine($"Sent DM invite to {toUserName} to connect to port {port} from {fromName}");
                return true;
            }

            Console.WriteLine($"DM invite failed: {toUserName} not connected on this server.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending DM invite: {ex.Message}");
            return false;
        }
    }

    public List<string> GetConnectedUsernames()
    {
        var list = new List<string>();
        foreach (var u in users) list.Add(u.Name);
        return list;
    }
}