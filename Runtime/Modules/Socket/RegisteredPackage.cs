using System;
using System.Collections.Generic;
using FantasticCore.Runtime.Utils;
using FantasticCore.Runtime.Modules.Socket.Data;

namespace FantasticCore.Runtime.Modules.Socket
{
    public interface IRegisteredPackage
    {
        internal void TriggerActions(string data);
    }
    
    internal sealed class RegisteredPackage<T> : IRegisteredPackage where T : SocketPackage
    {
        #region Fields

        private readonly HashSet<Action<T>> _actions = new();

        #endregion

        void IRegisteredPackage.TriggerActions(string data)
        {
            foreach (Action<T> action in _actions)
            {
                action?.Invoke(FantasticJson.ParseTo<T>(data));
            }
        }
        
        internal void AddAction(Action<T> action)
        {
            _actions.Add(action);
        }

        internal void RemoveAction(Action<T> action)
        {
            _actions.Remove(action);
        }
    }
}