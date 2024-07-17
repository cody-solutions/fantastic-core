using System;
using FantasticCore.Runtime.Debug;
using FantasticCore.Runtime.Modules.Socket.Data;
using FantasticCore.Runtime.Utils;

namespace FantasticCore.Runtime.Modules.Socket
{   
    public class SocketClient
    {
        #region Fields

        private readonly Action<SocketPackageData> _onMessage;

        #endregion
        
        #region Constructor

        public SocketClient(Action<SocketPackageData> onMessage)
        {
            _onMessage = onMessage;
        }

        #endregion
        
        private static SocketPackageData GetPackageData(string rawPackage)
        {
            var package = FantasticJson.ParseTo<SocketPackageData>(rawPackage);
            if (package == null)
            {
                FantasticDebug.Logger.LogMessage($"Failed get package from raw package! Raw package: {rawPackage}");
            }
            return package;
        }

        public void SendMessage(SocketPackageData packageData)
        {
            if (packageData == null)
            {
                return;
            }
            
            
        }

        private void OnMessage(string message)
        {
            SocketPackageData packageData = GetPackageData(message);
            if (packageData == null)
            {
                return;
            }

            _onMessage?.Invoke(packageData);
        }
    }
}