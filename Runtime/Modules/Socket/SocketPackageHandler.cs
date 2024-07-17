using System;
using System.Collections.Generic;
using FantasticCore.Runtime.Debug;
using FantasticCore.Runtime.Modules.Socket.Data;
using FantasticCore.Runtime.Modules.Socket.Packages;

namespace FantasticCore.Runtime.Modules.Socket
{
    public class SocketPackageHandler
    {
        #region Fields

        private Dictionary<string, IRegisteredPackage> _registeredPackages;

        #endregion
        
        #region Constructor

        public SocketPackageHandler()
        {
            _registeredPackages = new Dictionary<string, IRegisteredPackage>();
            RegisterBasePackages();
        }

        #endregion
        
        public bool IsPackageRegistered<T>(bool logErrorIfNotRegistered = false, string notRegisteredError = null) where T : SocketPackage
        {
            bool registered = IsPackageRegistered(typeof(T).Name);
            if (!registered && logErrorIfNotRegistered)
            {
                FantasticDebug.Logger.LogMessage(notRegisteredError, FantasticLogType.ERROR);
            }
            return registered;
        }
        
        public void RegisterPackage<T>() where T : SocketPackage
        {
            if (IsPackageRegistered<T>())
            {
                FantasticDebug.Logger.LogMessage($"Can not register {typeof(T).Name} package. Package is already registered!", FantasticLogType.ERROR);
                return;
            }

            var registeredPackage = new RegisteredPackage<T>();
            _registeredPackages.TryAdd(typeof(T).Name, registeredPackage);
        }

        public void BindListeningPackage<T>(bool state, Action<T> action) where T : SocketPackage
        {
            if (!IsPackageRegistered<T>(true, $"Can not find registered {typeof(T).Name} package. Register it first!"))
            {
                return;
            }

            var registeredPackage = _registeredPackages[typeof(T).Name] as RegisteredPackage<T>;
            if (state)
            {
                registeredPackage!.AddAction(action);
                return;
            }

            registeredPackage!.RemoveAction(action);
        }

        internal void HandlePackage(SocketPackageData packageData)
        {
            if (!IsPackageRegistered(packageData.Package))
            {
                FantasticDebug.Logger.LogMessage($"Can not find registered {packageData.Package} package. Register it first!");
                return;
            }

            IRegisteredPackage registeredPackage = _registeredPackages[packageData.Package];
            registeredPackage.TriggerActions(packageData.Data);
        }
        
        internal void ResetHandler()
        {
            _registeredPackages = null;
        }

        private bool IsPackageRegistered(string package)
        {
            return _registeredPackages.ContainsKey(package);
        }
        
        private void RegisterBasePackages()
        {
            RegisterPackage<KickPackage>();
            RegisterPackage<InvitePackage>();
        }
    }
}