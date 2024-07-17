using System;
using UnityEngine;
using UnityEngine.UI;

namespace FantasticCore.Runtime.Modules.Orientation.Elements
{ 
    [ExecuteAlways]
    [RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
    public sealed class OrientationLayoutElement : OrientationDataElement<SavedLayout, HorizontalOrVerticalLayoutGroup>
    { }

    [Serializable]
    public class SavedLayout : SavedData<HorizontalOrVerticalLayoutGroup>
    {
        #region Fields
        
        [Header("Layout Properties")]
        [SerializeField] private float _spacing;
        
        [SerializeField] private bool _reverseArrangement;
        
        [SerializeField] private bool _childControlHeight;
        [SerializeField] private bool _childControlWidth;
        
        [SerializeField] private bool _childScaleHeight;
        [SerializeField] private bool _childScaleWidth;
        
        [SerializeField] private bool _childForceExpandHeight;
        [SerializeField] private bool _childForceExpandWidth;

        [SerializeField] private Vector4 _padding = Vector4.negativeInfinity;

        #endregion

        protected override void SaveData(HorizontalOrVerticalLayoutGroup group)
        {
            _spacing = group.spacing;
            _reverseArrangement = group.reverseArrangement;
            
            _childControlHeight = group.childControlHeight;
            _childControlWidth = group.childControlWidth;

            _childScaleHeight = group.childScaleHeight;
            _childScaleWidth = group.childScaleWidth;

            _childForceExpandWidth = group.childForceExpandWidth;
            _childForceExpandHeight = group.childForceExpandHeight;

            _padding.x = group.padding.left;
            _padding.y = group.padding.right;
            _padding.z = group.padding.top;
            _padding.w = group.padding.bottom;
        }

        protected override void PutData(HorizontalOrVerticalLayoutGroup group)
        {
            group.spacing = _spacing;
            
            group.reverseArrangement = _reverseArrangement;
            group.childControlHeight = _childControlHeight;
            group.childControlWidth = _childControlWidth;

            group.childScaleHeight = _childScaleHeight;
            group.childScaleWidth = _childScaleWidth;

            group.childForceExpandHeight = _childForceExpandHeight;
            group.childForceExpandWidth = _childForceExpandWidth;

            if (float.IsNegativeInfinity(_padding.y))
            {
                return;
            }

            group.padding = new RectOffset((int)_padding.x, (int)_padding.y, (int)_padding.z, (int)_padding.w);
        }
    }
}