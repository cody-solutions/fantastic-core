using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace FantasticCore.Runtime.Modules.Orientation.Elements
{
    public abstract class OrientationElement<T> : OrientationElementBase where T : class
    {
        #region Fields

        [FormerlySerializedAs("_verticalRect"), FormerlySerializedAs("_spriteVertical")]
        [SerializeField] protected T _vertical;
        [FormerlySerializedAs("_horizontalRect"), FormerlySerializedAs("_spriteHorizontal")]
        [SerializeField] protected T _horizontal;

        #endregion

        #region MonoBehavior

        protected override void Awake()
        {
            InitElement();
            base.Awake();
        }

        private void OnValidate() => InitElement();

        #endregion
        
        public override void SaveCurrentState()
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, $"Edit Orientation Element - Vertical: {IsVertical}");
            UnityEditor.Undo.RecordObject(this, $"Edit object - Vertical: {IsVertical}");
#endif
        }

        protected abstract void InitElement();
    }
}