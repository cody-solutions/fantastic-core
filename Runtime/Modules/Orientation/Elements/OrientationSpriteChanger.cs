using UnityEngine;
using UnityEngine.UI;

namespace FantasticCore.Runtime.Modules.Orientation.Elements
{
    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    public class OrientationSpriteChanger : OrientationElement<Sprite>
    {
        #region Fields

        private Image _image;

        #endregion

        protected override void InitElement() => _image = GetComponent<Image>();

        public override void SaveCurrentState()
        {
            base.SaveCurrentState();
            
            if (IsVertical) _vertical = _image.sprite;
            else _horizontal = _image.sprite;
        }

        protected override void OnOrientationChanged(object sender, bool isVertical) => _image.sprite = isVertical ? _vertical : _horizontal;
    }
}
