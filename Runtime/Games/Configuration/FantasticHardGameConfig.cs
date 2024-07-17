using UnityEngine;

namespace FantasticCore.Runtime.Games.Configuration
{
    [CreateAssetMenu(fileName = "SO_HardGameConfig", menuName = "FantasticCore/Games/Game Config (Hard)", order = 0)]
    public class FantasticHardGameConfig : FantasticGameConfig
    {
        #region Properties

        [field: Header("UI"), SerializeField]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Sprite IconSprite { get; private set; }
        
        [field: SerializeField]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Sprite BannerSprite { get; private set; }
        
        [field: SerializeField]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Sprite[] ScreensSprites { get; private set; }

        #endregion
    }
}
