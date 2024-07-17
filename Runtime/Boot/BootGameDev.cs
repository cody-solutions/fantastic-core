using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using FantasticCore.Runtime.Base_Extensions;
using FantasticCore.Runtime.Configuration.Core;
using FantasticCore.Runtime.Modules.Auth;
using FantasticCore.Runtime.Modules.Auth.UI;
using FantasticCore.Runtime.Fast_Play;
using FantasticCore.Runtime.Debug;
using FantasticCore.Runtime.Games.Configuration;

namespace FantasticCore.Runtime.Boot
{
    /// <summary>
    /// Game bootstrapper for develop environment
    /// </summary>
    [SelectionBase, DisallowMultipleComponent]
    [AddComponentMenu("FantasticCore/Boot/Boot Game Dev")]
    internal sealed class BootGameDev : MonoBehaviour
    {
        #region Fields

        [Header("UI")]
        [SerializeField] private EventSystem _eventSystemPrefab;
        
        [Header("Auth")]
        [SerializeField] private AuthUIView _authUIViewPrefab;

        private AuthUIView _authUIView;
        
        #endregion

        #region MonoBehaviour

        private void Awake()
            => LoadBoot();

        #endregion

        /// <summary>
        /// Wait FantasticCore initialization and load game bootstrapper scene
        /// </summary>
        private async void LoadBoot()
        {
            EventSystem eventSystem = Instantiate(_eventSystemPrefab);
            DontDestroyOnLoad(eventSystem.gameObject);
            
            bool success = await FantasticInstance.WaitCoreInitialization();
            if (!success)
            {
                return;
            }

            FantasticCoreConfig coreConfig = FantasticCoreConfig.GetCurrent();
            if (!coreConfig.CustomAuthProvider)
            {
                await LoadAuthStep();
            }

            FantasticGameConfig gameConfig = FantasticCoreConfig.GetActiveGameConfig(coreConfig);
            if (gameConfig.BootSceneKey.IsNotValid())
            {
                FantasticDebug.Logger.LogMessage("Game BootSceneKey isn't valid", FantasticLogType.ERROR);
                return;
            }

            // Strange IDE syntax warning without pragma
#pragma warning disable CS4014
            Addressables.LoadSceneAsync(gameConfig.BootSceneKey);
#pragma warning restore
        }
        
        private async UniTask LoadAuthStep()
        {
            // Dont need authorize if IFantasticAuth doesn't registered
            if (!FantasticInstance.IsModulesRegistered<IFantasticAuth>())
            {
                return;
            }

            if (!_authUIViewPrefab)
            {
                throw new Exception("FantasticAuthPanelPrefab is not valid!");
            }

            FantasticInstance.TryGetModule(out IFantasticAuth auth);
            _authUIView = Instantiate(_authUIViewPrefab);
            var authUIPresenter = new AuthUIPresenter(auth, _authUIView);
            if (FantasticFastPlay.FastPlayEnabled)
            {
                authUIPresenter.SetFastPlayData(FantasticFastPlay.FastPlayData);
            }

            authUIPresenter.Enable();
            await auth.WaitForFullyLogged();
            authUIPresenter.Disable();
        }
    }
}