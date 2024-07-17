using System;
using UnityEngine;
using UnityEngine.UI;

namespace FantasticCore.Runtime.Modules.Orientation.Elements
{
    [ExecuteAlways]
    public class OrientationLayoutGroup : OrientationElement<LayoutGroup>
    {
        #region Fields

        private LayoutGroup _currentGroup;
        
        #endregion

        protected override void InitElement() => _currentGroup = GetComponent<LayoutGroup>();

        public override void SaveCurrentState()
        {
            base.SaveCurrentState();
            
            LayoutGroup group = IsVertical ? _vertical : _horizontal;
            group = OrientationUtils.GetCopyOf(group, _currentGroup);
            if (IsVertical) _vertical = group;
            else _horizontal = group;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        protected override void OnOrientationChanged(object sender, bool isVertical)
        {
            LayoutGroup group = IsVertical ? _vertical : _horizontal;
            if (!group)
            {
                return;
            }

            DestroyImmediate(_currentGroup);
            Type type = group.GetType();
            _currentGroup = (LayoutGroup)gameObject.AddComponent(type);
            _currentGroup = OrientationUtils.GetCopyOf(_currentGroup, group);
        }
    }
}