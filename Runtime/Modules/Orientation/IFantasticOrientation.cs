using UnityEngine.Events;

namespace FantasticCore.Runtime.Modules.Orientation
{
    /// <summary>
    /// Fantastic Orientation Module
    /// </summary>
    public interface IFantasticOrientation : IFantasticModule
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        public event UnityAction<bool> OnUpdateOrientation;

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool IsVertical { get; }

        #endregion
        
        #endregion
    }
}