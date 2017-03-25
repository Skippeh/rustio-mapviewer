using Newtonsoft.Json;

namespace RustIO.ApiClient.Models
{
    public class ServerStatus
    {
        [JsonProperty("env")] public Environment Environment;
        public string Hostname;
        public string Level;
        public int MaxPlayers;
        public int Players;
        public int Sleepers;
        public World World;
    }
}