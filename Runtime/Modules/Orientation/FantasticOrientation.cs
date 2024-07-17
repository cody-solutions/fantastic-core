using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using FantasticCore.Runtime.Modules.Orientation.Elements;

namespace FantasticCore.Runtime.Modules.Orientation
{
    /// <summary>
    /// Fantastic Orientation Module Implementation
    /// </summary>
    internal sealed class FantasticOrientation : MonoBehaviour, IFantasticOrientation
    {
        #region Fields

        public event UnityAction<bool> OnUpdateOrientation;

        #region Properties

        public IEnumerable<Type> ModulesDependencies => null;
        
        public bool IsVertical => Screen.height > Screen.width;

        #endregion
        
        #endregion
        
        #region MonoBehavior
        
        private void Update() => HandleOrientation();

        #endregion

        #region Module

        async UniTask IFantasticModule.InitializeModule()
        {
            OrientationElementBase.FireOrientationChanged(null, OrientationUtils.GetSettings().DefaultOrientation);
            HandleOrientation();
            await UniTask.CompletedTask;
        }

        async UniTask IFantasticModule.ResetModule()
        {
            await UniTask.CompletedTask;
        }

        #endregion
        
        private void HandleOrientation()
        {
            if (!OrientationUtils.GetSettings().EnabledAutomaticOrientationChange) return;
            
            if (IsVertical == OrientationElementBase.IsVertical)
            {
                return;
            }
            
            OrientationElementBase.FireOrientationChanged(this, IsVertical);
            OnUpdateOrientation?.Invoke(IsVertical);
        }
    }
}