using System;
using FantasticCore.Runtime.Debug;
using UnityEngine;

namespace FantasticCore.Runtime.Modules.Orientation.Elements
{
    public abstract class OrientationElementBase : MonoBehaviour
    {
        #region Fields

        private static event EventHandler<bool> OrientationChanged;
        
        #region Properties

        public static bool IsVertical { get; private set; }

        #endregion

        #endregion
        
        #region MonoBehavior

        protected virtual void Awake()
        {
            OrientationChanged += OnOrientationChanged;
            OnOrientationChanged(this, IsVertical);
        }
        
        protected virtual void OnDestroy() => OrientationChanged -= OnOrientationChanged;

        #endregion

        public abstract void SaveCurrentState();
        public void PutCurrentState() => OnOrientationChanged(this, IsVertical);
        
        public static void FireOrientationChanged(object s, bool isVertical) => OrientationChanged?.Invoke(s, isVertical);
        protected abstract void OnOrientationChanged(object sender, bool isVertical);

        [ContextMenu("Orientation Change")]
        private void OrientationChange()
        {
            if (!isActiveAndEnabled)
            {
                FantasticDebug.Logger.LogMessage("The current GameObject (or component) is not active and therefore its state will not be changed.", FantasticLogType.WARN);
            }
            
            OrientationChanged?.Invoke(null, !IsVertical);
        }
        
        static OrientationElementBase() => OrientationChanged += (_, e) => IsVertical = e;
    }
}