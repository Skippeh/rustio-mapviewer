using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RustIO.ApiClient;

namespace RustIO.MapDownloader
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (var api = new RustIOApi())
            {
                var servers = api.RequestServers();

                Console.Write("Partial server name: ");
                string serverName = Console.ReadLine().ToLower();

                var server = servers.FirstOrDefault(s => s.Hostname.ToLower() == serverName) ??
                             servers.FirstOrDefault(s => s.Hostname.ToLower().StartsWith(serverName)) ??
                             servers.FirstOrDefault(s => s.Hostname.ToLower().Contains(serverName));

                if (server == null)
                {
                    Console.WriteLine("No server found.");
                    return;
                }
                else
                {
                    Console.WriteLine("Found server: " + server.Hostname);
                }

                if (server.World == null)
                {
                    Console.WriteLine("Server's map size or seed missing.");
                    return;
                }

                var monuments = api.RequestMonuments(server);

                if (monuments != null)
                {
                    Console.WriteLine("Downloading map");

                    Bitmap bitmap = api.RequestMapImage(server);

                    string fileName = $"{GetFriendlyFileName(server.Hostname)}.jpg";
                    Console.WriteLine("Saving to " + fileName);
                    bitmap.Save(fileName, ImageFormat.Jpeg);
                    try
                    {
                        Process.Start(fileName);
                    }
                    catch (Exception) { /* ignore */ }
                }
                else
                {
                    Console.WriteLine("Map not generated");
                    try
                    {
                        Process.Start($"http://playrust.io/map/?{server.Endpoint}");
                    }
                    catch (Exception) { /* ignore */ }
                }
            }
        }

        private static string GetFriendlyFileName(string str)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                str = str.Replace(c, '_');
            }

            return str;
        }
    }
}