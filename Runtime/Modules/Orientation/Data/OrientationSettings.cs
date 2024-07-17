using System;
using UnityEngine;

namespace FantasticCore.Runtime.Modules.Orientation.Data
{
    [Serializable]
    public class OrientationSettings
    {
        #region Fields

        [field: Header("Editor"), SerializeField]
        public bool RefreshOnSceneLoad { get; set; }
        [field: SerializeField]
        public bool EnabledAutomaticOnEditor { get; set; }
        
        [field: Header("Game"), SerializeField]
        public bool DefaultOrientation { get; set; }
        [field: SerializeField]
        public bool EnabledAutomaticOrientationChange { get; set; }

        #endregion

    }
}