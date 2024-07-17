using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace FantasticCore.Runtime.Modules.Orientation.Elements
{
    public abstract class OrientationDataElement<T, T2> : OrientationElement<T> where T : SavedData<T2> where T2 : Component
    {
        #region Fields

        private T2 _component;

        #endregion

        protected override void InitElement() => _component = GetComponent<T2>();

        public override void SaveCurrentState()
        {
            base.SaveCurrentState();
            
            if (IsVertical) _vertical.Save(_component);
            else _horizontal.Save(_component);
        }

        protected override void OnOrientationChanged(object sender, bool isVertical)
        {
            if (isVertical) _vertical?.Put(_component);
            else _horizontal?.Put(_component);
        }
    }
    
    [Serializable]
    public abstract class SavedData<T> where T : Component
    {
        #region Fields

        [FormerlySerializedAs("_isInit")]
        [SerializeField, HideInInspector] private bool _isInitialized; 
        
        #endregion

        /// <summary>
        /// Saves data from the Component to this object.
        /// </summary>
        /// <param name="component"></param>
        public void Save(T component)
        {
            if (!component)
            {
                return;
            }

            _isInitialized = true;
            
            SaveData(component);
        }

        protected abstract void SaveData(T component);

        /// <summary>
        /// Uploads data from this object to Component.
        /// </summary>
        /// <param name="component"></param>
        public void Put(T component)
        {
            if (!component || !_isInitialized)
            {
                return;
            }

            PutData(component);
        }
        
        protected abstract void PutData(T component);

        public void ReInit() => _isInitialized = true;
    }
}