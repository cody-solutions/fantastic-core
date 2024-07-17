using UnityEngine;

namespace FantasticCore.Runtime.Games.Configuration
{
    [CreateAssetMenu(fileName = "SO_GameConfig", menuName = "FantasticCore/Games/Game Config", order = 0)]
    public class FantasticGameConfig : ScriptableObject
    {
        #region Properties

        [field: Header("Main"), SerializeField, Range(1, 1000)]
        public int GameConfigId { get; private set; } = 1;

        [field: Header("For In Game Only"), SerializeField]
        public string Name { get; private set; } = "Game";

        [field: SerializeField]
        public string OverrideBundleName { get; private set; }

        [field: SerializeField]
        public string Version { get; private set; } = "0.0.1";

        [field: SerializeField]
        public int BuildNumber { get; private set; } = 1;

        [field: SerializeField]
        public string BootSceneKey { get; private set; } = "S_BootGame";

        #endregion
    }
}
