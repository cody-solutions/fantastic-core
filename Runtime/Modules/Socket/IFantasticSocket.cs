using System;
using FantasticCore.Runtime.Modules.Socket.Data;

namespace FantasticCore.Runtime.Modules.Socket
{
    /// <summary>
    /// Fantastic Socket Module
    /// </summary>
    public interface IFantasticSocket : IFantasticModule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageData"></param>
        public void SendMessage(SocketPackageData packageData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public void BindListeningPackage<T>(bool state, Action<T> action) where T : SocketPackage;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterPackage<T>() where T : SocketPackage;
    }
}