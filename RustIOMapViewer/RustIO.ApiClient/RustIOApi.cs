using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RustIO.ApiClient.Models;

namespace RustIO.ApiClient
{
    public class RustIOApi : IDisposable
    {
        private const string apiUrl = "http://playrust.io/";
        private readonly CookieWebClient webClient;

        private readonly List<GameServer> servers; 

        public RustIOApi()
        {
            webClient = new CookieWebClient();
            webClient.Proxy = new WebProxy("127.0.0.1:8888", false, new[] {"<-loopback>"}); // fiddler debug

            webClient.Headers["Origin"] = "http://playrust.io";

            servers = new List<GameServer>();
        }

        public void Dispose()
        {
            webClient.Dispose();

            foreach (var server in servers)
            {
                server.Dispose();
            }
        }

        public GameServer[] RequestServers()
        {
            var servers = MakeRequest<GameServer[]>("servers.json");

            foreach (var server in servers)
            {
                server.Api = this;
                this.servers.Add(server);
            }

            return servers;
        }

        public ServerStatus RequestStatus(GameServer server)
        {
            return MakeRequest<ServerStatus>($"http://{server.Endpoint}/status.json");
        }

        public Monument[] RequestMonuments(GameServer server)
        {
            if (!server.HasIO)
            {
                return RequestMonuments(server.Level, server.World.Size, server.World.Seed);
            }

            return MakeRequest<Monument[]>($"http://{server.Endpoint}/monuments.json");
        }

        public Monument[] RequestMonuments(string level, int size, long seed)
        {
            SetReferer(level, size, seed);
            return MakeRequest<Monument[]>($"monuments.json?level={level}&size={size}&seed={seed}");
        }

        private void SetReferer(string level, int size, long seed)
        {
            webClient.Headers["Referer"] = $"http://playrust.io/map/?{HttpUtility.UrlEncode(level)}_{size}_{seed}";
        }

        private T MakeRequest<T>(string path)
        {
            string absPath = path.StartsWith("http://") || path.StartsWith("https://")
                             ? path
                             : apiUrl + path;

            try
            {
                var response = webClient.DownloadStringAwareOfEncoding(new Uri(absPath));
                return JsonConvert.DeserializeObject<T>(response);
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;

                if (response == null)
                    throw;

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    var stream = response.GetResponseStream();
                    var encoding = WebUtils.GetEncodingFrom(response.Headers);
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);

                    string jsonResponse = encoding.GetString(bytes);
                    var jObject = JObject.Parse(jsonResponse);

                    if (jObject["error"] != null && jObject["error"].Value<string>() == "notfound")
                    {
                        return default(T);
                    }
                }

                throw;
            }
        }

        public Bitmap RequestMapImage(GameServer server)
        {
            SetReferer(server.Level, server.World.Size, server.World.Seed);

            string requestUrl = server.HasIO
                                ? $"http://{server.Hostname}/map.jpg"
                                : $"http://playrust.io/map.jpg?level={HttpUtility.UrlEncode(server.Level)}&size={server.World.Size}&seed={server.World.Seed}";

            try
            {
                byte[] bytes = webClient.DownloadData(requestUrl);
                using (var memStream = new MemoryStream(bytes))
                {
                    var image = Image.FromStream(memStream);
                    var bitmap = new Bitmap(image, image.Size.Width, image.Size.Height);
                    return bitmap;
                }
            }
            catch (WebException ex)
            {
                return null;
            }
        }
    }
}