using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using FishNet.Object;
using FantasticCore.Runtime.Base_Extensions;
using FantasticCore.Runtime.Data;
using FantasticCore.Runtime.Debug;
using FantasticCore.Runtime.Modules.Network;

namespace FantasticCore.Runtime.Boot
{
    /// <summary>
    /// Game bootstrapper. Inherit your game boot class from this and handle game initialization
    /// </summary>
    [SelectionBase, DisallowMultipleComponent]
    public abstract class BootGame : MonoBehaviour
    {
        #region Fields

        [Header("Boot")]
        [SerializeField] protected bool _autoRun = true;
        [SerializeField] protected string _nextSceneKey = "S_Scene";

        [Header("Network")]
        [SerializeField, Tooltip("Keep empty to skip this process")]
        private string[] _networkObjectPrefabsKeys;

        [SerializeField, Tooltip("Add PlayerSpawner network component with assigned player prefab if down key exist")]
        private bool _addNetworkPlayerSpawner;
        
        [SerializeField, Tooltip("Player addressable network prefab key. Key must be defined in above list too")]
        private string _playerPrefabKey;
        
        #endregion
        
        #region MonoBehaviour

        private void Awake()
        {
            if (!_autoRun)
            {
                return;
            }
            
            Run();
        }

        #endregion

        private static void SetApplicationServerSettings()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public async void Run()
        {
            await OnBaseInitialization();
            await TryRegisterNetworkPrefabs();

            bool loadNextScene = FantasticInstance.CoreConfig.PlatformType switch
            {
                FantasticPlatformType.CLIENT => await OnGameInitialization(),
                FantasticPlatformType.SERVER => await InitializeGameServer(),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (loadNextScene)
            {
                LoadNextScene();
            }
        }

        /// <summary>
        /// Implement and initialize your base game core here
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once VirtualMemberNeverOverridden.Global
        protected virtual async UniTask OnBaseInitialization()
            => await UniTask.CompletedTask;

        /// <summary>
        /// Implement and initialize your game here
        /// </summary>
        /// <returns>LoadNextScene will be called if True</returns>
        // ReSharper disable once VirtualMemberNeverOverridden.Global
        protected virtual UniTask<bool> OnGameInitialization()
            => new(true);

        /// <summary>
        /// Initialize game server
        /// </summary>
        /// <returns>LoadNextScene will be called if True</returns>>
        // ReSharper disable once VirtualMemberNeverOverridden.Global
        protected virtual UniTask<bool> InitializeGameServer()
        {
            SetApplicationServerSettings();
            return new UniTask<bool>(true);
        }

        /// <summary>
        /// Load scene with <see cref="_nextSceneKey"/>> key throw Addressables system
        /// </summary>
        // ReSharper disable once VirtualMemberNeverOverridden.Global
        protected virtual void LoadNextScene()
        {
            if (_nextSceneKey.IsNotValid(true, "NextSceneKey isn't valid!"))
            {
                return;
            }

            Addressables.LoadSceneAsync(_nextSceneKey);
        }

        /// <summary>
        /// Register desired game don't destroy objects in FantasticCore system 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        protected void RegisterGameDontDestroyObjects(GameObject[] objects)
            => FantasticInstance.GameLoader.RegisterGameDontDestroyObjects(objects);

        /// <summary>
        /// Register network prefab objects in FantasticNetworkSystem. Require <see cref="IFantasticNetwork"/>> module
        /// </summary>
        private async UniTask TryRegisterNetworkPrefabs()
        {
            if (_networkObjectPrefabsKeys is null || _networkObjectPrefabsKeys.Length == 0)
            {
                return;
            }
            
            if (!FantasticInstance.TryGetModule(out IFantasticNetwork network))
            {
                FantasticDebug.Logger.LogMessage("Failed get FantasticNetwork module!", FantasticLogType.ERROR);
                return;
            }

            bool playerPrefabAdded = false;
            foreach (string key in _networkObjectPrefabsKeys)
            {
                NetworkObject networkObject = await LoadNetworkObject(key);
                if (!networkObject)
                {
                    continue;
                }

                network.AddNetworkPrefab(networkObject);
                if (playerPrefabAdded)
                {
                    continue;
                }

                if (!_addNetworkPlayerSpawner || key != _playerPrefabKey)
                {
                    continue;
                }
                
                network.AddPlayerSpawner(networkObject);
                playerPrefabAdded = true;
            }

            await UniTask.CompletedTask;
            return;

            async UniTask<NetworkObject> LoadNetworkObject(string key)
            {
                AsyncOperationHandle<GameObject> operation
                    = Addressables.LoadAssetAsync<GameObject>(key);
                await operation.Task;

                if (operation.Status != AsyncOperationStatus.Succeeded)
                {
                    FantasticDebug.Logger.LogMessage($"Failed load addressable with {key} key!" +
                                                     " Skip network object registration...");
                    return null;
                }

                if (operation.Result.TryGetComponent(out NetworkObject networkObject))
                {
                    return networkObject;
                }

                FantasticDebug.Logger.LogMessage($"Asset with {key} key isn't NetworkObject!" +
                                                 " Skip register network object registration...");
                return null;
            }
        }
    }
}