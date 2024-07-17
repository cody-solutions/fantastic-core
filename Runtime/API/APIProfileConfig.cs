using UnityEngine;
using FantasticCore.Runtime.API.Data;

namespace FantasticCore.Runtime.API
{
    [CreateAssetMenu(fileName = "SO_APIProfileConfig", menuName = "FantasticCore/API/Profile Config", order = 0)]
    public class APIProfileConfig : ScriptableObject
    {
        #region Properties

        [field: SerializeField] 
        public APIProfileData Profile { get; private set; }

        #endregion
    }
}