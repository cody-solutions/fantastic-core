using FantasticCore.Runtime.Modules.Socket.Data;
using Newtonsoft.Json;

namespace FantasticCore.Runtime.Modules.Socket.Packages
{
    public class KickPackage : SocketPackage
    {
        #region Properties

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        #endregion
    }
    
    public class InvitePackage : SocketPackage
    {
        #region Properties

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("roomId")]
        public int RoomId { get; set; }

        #endregion
    }
}