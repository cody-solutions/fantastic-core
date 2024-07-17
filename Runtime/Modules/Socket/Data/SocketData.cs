using Newtonsoft.Json;

namespace FantasticCore.Runtime.Modules.Socket.Data
{
    public class SocketPackageData
    {
        #region Properties

        [JsonProperty("package")]
        public string Package { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        #endregion
    }

    public abstract class SocketPackage { }
}