using System;
using UnityEngine;
using UnityEngine.UI;

namespace FantasticCore.Runtime.Modules.Orientation.Elements
{
    [ExecuteAlways, RequireComponent(typeof(Image))]
    public sealed class OrientationImageElement : OrientationDataElement<SavedImage, Image>
    { }

    [Serializable]
    public class SavedImage : SavedData<Image>
    {
        #region Fields

        [SerializeField] private Material _material;
        [SerializeField] private Color _color;
        [SerializeField] private bool _target;
        [SerializeField] private Vector4 _padding;
        [SerializeField] private bool _maskable;
        [SerializeField] private Image.Type _type;
        [SerializeField] private bool _useSpriteMesh;
        [SerializeField] private bool _preserveAspect;

        #endregion
        
        protected override void SaveData(Image component)
        {
            _material = component.material;
            _color = component.color;
            _target = component.raycastTarget;
            _padding = component.raycastPadding;
            _maskable = component.maskable;
            _type = component.type;
            _useSpriteMesh = component.useSpriteMesh;
            _preserveAspect = component.preserveAspect;
        }

        protected override void PutData(Image component)
        {
            component.material = _material;
            component.color = _color;
            component.raycastTarget = _target;
            component.raycastPadding = _padding;
            component.maskable = _maskable;
            component.type = _type;
            component.useSpriteMesh = _useSpriteMesh;
            component.preserveAspect = _preserveAspect;
        }
    }
}