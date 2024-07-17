using System;
using UnityEngine;

namespace FantasticCore.Runtime.Modules.Orientation.Elements
{ 
    [ExecuteAlways] 
    [RequireComponent(typeof(RectTransform))]
    public sealed class OrientationRectElement : OrientationDataElement<SavedRect, RectTransform>
    { }
    
    [Serializable]
    public class SavedRect : SavedData<RectTransform>
    {
        #region Fields
        
        [Header("Rect")]
        [SerializeField] private Vector3 _anchoredPosition;
        [SerializeField] private Vector2 _sizeDelta;
        [SerializeField] private Vector2 _minAnchor;
        [SerializeField] private Vector2 _maxAnchor;
        [SerializeField] private Vector2 _pivot;
        [SerializeField] private Vector3 _rotation;
        [SerializeField] private Vector3 _scale;
        
        #endregion
        
        protected override void SaveData(RectTransform component)
        {
            _anchoredPosition = component.anchoredPosition3D;
            _sizeDelta = component.sizeDelta;
            _minAnchor = component.anchorMin;
            _maxAnchor = component.anchorMax;
            _pivot = component.pivot;
            _rotation = component.localEulerAngles;
            _scale = component.localScale;
        }
        
        protected override void PutData(RectTransform component)
        {
            component.anchoredPosition3D = _anchoredPosition;
            component.sizeDelta = _sizeDelta;
            component.anchorMin = _minAnchor;
            component.anchorMax = _maxAnchor;
            component.pivot = _pivot;
            component.localEulerAngles = _rotation;
            component.localScale = _scale;
        }
    }
}