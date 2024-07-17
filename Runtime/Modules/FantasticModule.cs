using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace FantasticCore.Runtime.Modules
{
    /// <summary>
    /// Base Fantastic Module
    /// </summary>
    public interface IFantasticModule
    {
        /// <summary>
        /// Dependency modules. Module require this modules to correct work
        /// It's bad that is Type but for now it's ok :D
        /// TODO: Limit to accept only IFantasticModule
        /// </summary>
        public IEnumerable<Type> ModulesDependencies { get; }

        /// <summary>
        /// Initialize module when module was registered
        /// </summary>
        internal UniTask InitializeModule();

        /// <summary>
        /// Reset module after module was unregistered
        /// </summary>
        internal UniTask ResetModule();
    }
}