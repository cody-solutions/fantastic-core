using System;
using FantasticCore.Runtime.Debug;
using UnityEngine;

namespace FantasticCore.Runtime.Modules.Orientation.Elements
{
    [ExecuteAlways]
    public sealed class OrientationGameObjectActive : OrientationElement<GameObjectStateData>
    {
        #region Fields

        [SerializeField] private GameObject _object;

        #endregion

        public override void SaveCurrentState()
        {
            base.SaveCurrentState();
            if (!_object)
            {
                FantasticDebug.Logger.LogMessage("GameObject is null!", FantasticLogType.ERROR);
                return;
            }

            if (IsVertical) _vertical.Save(_object.activeSelf);
            else _horizontal.Save(_object.activeSelf);
        }

        protected override void OnOrientationChanged(object sender, bool isVertical)
        {
            if (!_object)
            {
                FantasticDebug.Logger.LogMessage("GameObject is null!", FantasticLogType.WARN);
                return;
            }
            
            _object.SetActive(isVertical ? _vertical.State : _horizontal.State);
        }
        
        protected override void InitElement()
        {
            if (!_object)
            {
                FantasticDebug.Logger.LogMessage("GameObject is null!", FantasticLogType.WARN);
                return;
            }
            
            if (!gameObject.transform.IsChildOf(_object.transform)) return;
            
            _object = null;
            FantasticDebug.Logger.LogMessage("The selected GameObject cannot be the parent of a component or the component itself.", FantasticLogType.ERROR);
        }
    }

    [Serializable]
    public class GameObjectStateData
    {
        #region Fields

        [SerializeField] private bool _state;

        #region Propeties

        public bool State => _state;

        #endregion

        #endregion

        public void Save(bool state) => _state = state;
    }
}