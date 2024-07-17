using UnityEngine;

namespace FantasticCore.Runtime.UI
{
    public abstract class BaseUIView : MonoBehaviour
    {
        #region Fields

        [Header("UI View")]
        [SerializeField] private GameObject _panel;
        [SerializeField] private bool _setAwakePanelEnabledState = true;

        #region Properties

        public bool IsPanelEnabled => _panel.activeInHierarchy;

        #endregion
        
        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            OnCreated();
            HandleAwakeEnabledState();
        }

        private void OnEnable()
        {
            OnShowed();
        }

        private void OnDisable()
        {
            OnHided();
        }

        private void OnDestroy()
        {
            OnDestroyed();
        }

        #endregion
        
        public void Show()
        {
            if (IsPanelEnabled)
            {
                return;
            }

            _panel.SetActive(true);
        }
        
        public void Hide()
        {
            if (!IsPanelEnabled)
            {
                return;
            }
            
            _panel.SetActive(false);
        }

        protected virtual void OnCreated()
        {
        }

        protected virtual void OnDestroyed()
        {
            
        }

        protected virtual void OnShowed()
        {
            
        }

        protected virtual void OnHided()
        {
            
        }

        private void HandleAwakeEnabledState()
        {
            if (_setAwakePanelEnabledState)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

#if UNITY_EDITOR
        #region Context Menu

        [ContextMenu("Show")]
        private void ShowContextMenu()
        {
            Show();
        }

        [ContextMenu("Hide")]
        private void HideContextMenu()
        {
            Hide();
        }

        #endregion
#endif
    }
}