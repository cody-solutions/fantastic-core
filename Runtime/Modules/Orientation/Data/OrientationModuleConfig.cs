using UnityEngine;

namespace FantasticCore.Runtime.Modules.Orientation.Data
{
    [CreateAssetMenu(fileName = "SO_OrientationModuleConfig", menuName = "FantasticCore/API/Orientation Config", order = 0)]
    public class OrientationModuleConfig : ScriptableObject
    {
        #region Fields

        [SerializeField] private OrientationSettings _orientationSettings = new();

        #region Propeties

        public OrientationSettings OrientationSettings => _orientationSettings;

        #endregion

        #endregion

        public void SetSetting(OrientationSettings orientationSettings)
        {
            if (orientationSettings == null)
            {
                return;
            }
            
            _orientationSettings = orientationSettings;
        }
    }
}