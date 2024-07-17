using FantasticCore.Runtime.Modules.Socket.Data;
using FantasticCore.Runtime.Utils;

namespace FantasticCore.Runtime.Modules.Socket
{
    public static class SocketPackageBuilder
    {
        public static SocketPackageData Build<T>(T data) where T : SocketPackage
        {
            return new SocketPackageData
            {
                Package = typeof(T).Name,
                Data = FantasticJson.Parse(data)
            };
        }
    }
}