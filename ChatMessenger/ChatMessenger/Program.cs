using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatMessenger
{
    internal static class Program
    {
        private static DiscoveryServer discoveryServer;
        private static JsonWebSocketServer webSocketServer;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            StartServers();

            Application.Run(new Form1());
        }

        static void StartServers()
        {
            discoveryServer = new DiscoveryServer(5000, 6000);
            discoveryServer.Enable();
            Task.Run(() => discoveryServer.StartAsync());

            webSocketServer = new JsonWebSocketServer(6000);
            webSocketServer.Enable();
            Task.Run(() => webSocketServer.StartAsync());
        }
    }
}
