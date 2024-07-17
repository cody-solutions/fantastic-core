using System;
using UnityEngine;
using UnityEngine.UI;

namespace FantasticCore.Runtime.Modules.Orientation.Elements
{
    [ExecuteAlways, RequireComponent(typeof(ScrollRect))]
    public sealed class OrientationScrollElement : OrientationDataElement<SavedScroll, ScrollRect>
    { }

    [Serializable]
    public class SavedScroll : SavedData<ScrollRect>
    {
        #region Fields

        [SerializeField] private bool _horizontalScroll;
        [SerializeField] private bool _verticalScroll;
        [SerializeField] private ScrollRect.MovementType _movementType;
        [SerializeField] private float _elasticity;
        [SerializeField] private bool _inertia;
        [SerializeField] private float _rate;
        [SerializeField] private float _sensitivity;

        #endregion
        
        protected override void SaveData(ScrollRect component)
        {
            _horizontalScroll = component.horizontal;
            _verticalScroll = component.vertical;
            _movementType = component.movementType;
            _elasticity = component.elasticity;
            _inertia = component.inertia;
            _rate = component.decelerationRate;
            _sensitivity = component.scrollSensitivity;
        }

        protected override void PutData(ScrollRect component)
        {
            component.horizontal = _horizontalScroll;
            component.vertical = _verticalScroll;
            component.movementType = _movementType;
            component.elasticity = _elasticity;
            component.inertia = _inertia;
            component.decelerationRate = _rate;
            component.scrollSensitivity = _sensitivity;
        }
    }
}