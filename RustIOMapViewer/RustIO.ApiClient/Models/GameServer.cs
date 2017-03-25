using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebSocketSharp;

namespace RustIO.ApiClient.Models
{
    public class GameServer : IDisposable
    {
        internal RustIOApi Api { get; set; }

        public string Endpoint;
        public string Country;
        public string Hostname;
        public string Level;
        public int Players;
        public int MaxPlayers;
        public string[] Tags;
        public DateTime UpdateTime;
        public float Fps;
        [JsonProperty("env")] public Environment Environment;
        public World World;
        public ServerVersion Version;
        public bool Modded => modded == 1;
        public bool HasIO => io == 1;

        [JsonProperty] private int? modded;
        [JsonProperty] private int? io;
        
        public bool WebsocketConnected => webSocket != null && webSocket.ReadyState == WebSocketState.Open;

        private WebSocket webSocket;
        private CancellationTokenSource cancellationSource;

        public async Task<bool> ConnectWebsocket()
        {
            if (!HasIO)
                return false;

            cancellationSource = new CancellationTokenSource();
            webSocket = new WebSocket($"ws://{Endpoint}/ms", cancellationSource.Token, 102392, OnWSOpen, OnWSClose, OnWSMessage, OnWSError);

            if (!await webSocket.Connect())
            {
                return false;
            }
            
            return true;
        }

        public async void DisconnectWebsocket()
        {
            await webSocket.Close();
            cancellationSource = null;
            webSocket = null;
        }

        private async Task OnWSOpen()
        {

        }

        private async Task OnWSClose(CloseEventArgs ev)
        {

        }

        private async Task OnWSMessage(MessageEventArgs message)
        {
            
        }

        private async Task OnWSError(ErrorEventArgs error)
        {

        }

        public void Dispose()
        {
            if (WebsocketConnected)
                DisconnectWebsocket();

            webSocket?.Dispose();
        }
    }
}