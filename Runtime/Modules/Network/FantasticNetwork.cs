using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityObject = UnityEngine.Object;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Component.Utility;
using FishNet.Component.Spawning;
using FantasticCore.Runtime.Debug;
using FantasticCore.Runtime.Modules.Network.Extensions;

namespace FantasticCore.Runtime.Modules.Network
{
    /// <summary>
    /// Fantastic Network Module Implementation
    /// </summary>
    internal sealed class FantasticNetwork : IFantasticNetwork
    {
        #region Fields

        private PingDisplay _pingDisplay;
        private PlayerSpawner _playerSpawner;
        
        #region Properties

        public IEnumerable<Type> ModulesDependencies => null;
        
        public NetworkManager NetworkManager { get; private set; }
        
        #endregion

        #endregion

        #region Module

        async UniTask IFantasticModule.InitializeModule()
            => await LoadNetworkSceneAndWaitNetworkInitialization();

        async UniTask IFantasticModule.ResetModule()
        {
            await UniTask.CompletedTask;
        }

        #endregion
        
        public void AddNetworkPrefab(NetworkObject prefab)
        {
            NetworkManager.SpawnablePrefabs.AddObject(prefab);
        }

        public void AddNetworkPrefab(NetworkObject[] prefabs)
        {
            NetworkManager.SpawnablePrefabs.AddObjects(prefabs);
        }

        public void EnablePingDisplay(PingDisplay.Corner corner = PingDisplay.Corner.TopRight)
        {
            EnablePingDisplay(Color.white, corner);
        }

        public void EnablePingDisplay(Color color, PingDisplay.Corner placement = PingDisplay.Corner.TopRight)
        {
            if (_pingDisplay)
            {
                FantasticDebug.Logger.LogMessage("PingDisplay already added!", FantasticLogType.WARN);
                return;
            }

            _pingDisplay = NetworkManager.gameObject.AddComponent<PingDisplay>();
            _pingDisplay.SetStyle(color, placement);
        }

        public void DisablePingDisplay()
        {
            if (!_pingDisplay)
            {
                FantasticDebug.Logger.LogMessage("PingDisplay doesn't exist!", FantasticLogType.ERROR);
                return;
            }
            
            UnityObject.Destroy(_pingDisplay);
        }

        #region Player Spawner

        public void AddPlayerSpawner(NetworkObject player, bool addToDefaultScene = true)
        {
            if (_playerSpawner)
            {
                FantasticDebug.Logger.LogMessage("PlayerSpawner already added!", FantasticLogType.WARN);
                return;
            }

            _playerSpawner = NetworkManager.gameObject.AddComponent<PlayerSpawner>();
            _playerSpawner.Setup(player, addToDefaultScene);
        }

        public void RemovePlayerSpawner()
        {
            if (!_playerSpawner)
            {
                FantasticDebug.Logger.LogMessage("PlayerSpawner doesn't exist!", FantasticLogType.ERROR);
                return;
            }
            
            UnityObject.Destroy(_playerSpawner);
        }

        public void SetSpawnPointsToPlayerSpawner(Transform[] spawnPoints)
        {
            if (!_playerSpawner)
            {
                FantasticDebug.Logger.LogMessage("PlayerSpawner doesn't exist!", FantasticLogType.ERROR);
                return;
            }

            _playerSpawner.SetSpawnPoints(spawnPoints);
        }

        #endregion

        public void StartLocalHost()
        {
            if (!NetworkManager)
            {
                return;
            }
            
            NetworkManager.ServerManager.StartConnection();
            NetworkManager.ClientManager.StartConnection();
        }

        async UniTask IFantasticNetwork.ResetNetworkAfterGame()
        {
            if (NetworkManager)
            {
                UnityObject.Destroy(NetworkManager.gameObject);
            }

            await LoadNetworkSceneAndWaitNetworkInitialization();
        }

        private async UniTask LoadNetworkSceneAndWaitNetworkInitialization()
        {
            AsyncOperationHandle<SceneInstance> operation = 
                Addressables.LoadSceneAsync(IFantasticNetwork.NetworkScene, LoadSceneMode.Additive);
            await operation.Task;

            if (operation.Status != AsyncOperationStatus.Succeeded)
            {
                throw new Exception("Failed FantasticNetwork initialization!");
            }

            NetworkManager = InstanceFinder.NetworkManager;
            await UniTask.WaitUntil(() => NetworkManager.Initialized);
        }
    }
}