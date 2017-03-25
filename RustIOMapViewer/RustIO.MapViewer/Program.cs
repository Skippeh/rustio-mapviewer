using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using RustIO.ApiClient;

namespace RustIO.MapViewer
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (var api = new RustIOApi())
            {
                var servers = api.RequestServers();
                var server = servers.First(s => !s.HasIO && s.Players >= 100);

                var monuments = api.RequestMonuments(server);

                if (monuments != null)
                {
                    Bitmap bitmap = api.RequestMapImage(server);
                    
                    bitmap.Save("test.jpg", ImageFormat.Jpeg);
                    try
                    {
                        Process.Start("test.jpg");
                    }
                    catch (Exception ex) { /* ignore */ }
                }
                else
                {
                    Console.WriteLine("Map not generated");
                }

                if (server.HasIO)
                {
                    Task.WaitAll(server.ConnectWebsocket());

                    if (server.WebsocketConnected)
                    {
                        Console.WriteLine("connected");
                    }
                }
            }
        }
    }
}