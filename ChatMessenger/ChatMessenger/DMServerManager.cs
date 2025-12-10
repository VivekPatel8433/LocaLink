using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatMessenger
{
    public static class DMServerManager
    {
        private static int nextPort = 6000; // starting port

        public static int CreateDMServer(string fromUser, string toUser)
        {
            int port = Interlocked.Increment(ref nextPort);

            Task.Run(() => StartDMServer(port, fromUser, toUser));

            return port;
        }

        private static async Task StartDMServer(int port, string userA, string userB)
        {
            var server = new HttpListener();
            server.Prefixes.Add($"http://localhost:{port}/");
            server.Start();

            while (true)
            {
                var ctx = await server.GetContextAsync();

                if (ctx.Request.IsWebSocketRequest)
                {
                    var wsCtx = await ctx.AcceptWebSocketAsync(null);
                    WebSocket socket = wsCtx.WebSocket;

                    Console.WriteLine($"DM server: {userA} <-> {userB} connect on port {port}");

                    await HandleConnection(socket);
                }
                else
                {
                    ctx.Response.StatusCode = 400;
                    ctx.Response.Close();
                }
            }
        }

        private static async Task HandleConnection(WebSocket socket)
        {
            byte[] buffer = new byte[1024];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }

                // Broadcast echo (temporary until we implement message routing)
                string msg = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine("DM Message: " + msg);
            }
        }
    }
}
