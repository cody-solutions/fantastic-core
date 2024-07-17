using UnityEngine;
using Cysharp.Threading.Tasks;
using FishNet.Object;
using FishNet.Component.Utility;
using FishNet.Managing;

namespace FantasticCore.Runtime.Modules.Network
{
    /// <summary>
    /// Fantastic Network Module
    /// </summary>
    public interface IFantasticNetwork : IFantasticModule
    {
        #region Fields

        internal const string NetworkScene = "S_Network";

        #region Properties

        /// <summary>
        /// Current Fish-Networking network manager object
        /// </summary>
        public NetworkManager NetworkManager { get; }
        
        #endregion

        #endregion
        
        /// <summary>
        /// Add network spawnable prefab
        /// </summary>
        /// <param name="prefab"></param>
        public void AddNetworkPrefab(NetworkObject prefab);

        /// <summary>
        /// Add network spawnable prefabs
        /// </summary>
        /// <param name="prefabs"></param>
        public void AddNetworkPrefab(NetworkObject[] prefabs);

        /// <summary>
        /// Enable ping display HUD
        /// </summary>
        /// <param name="corner"></param>
        public void EnablePingDisplay(PingDisplay.Corner corner = PingDisplay.Corner.TopRight);
        
        /// <summary>
        /// Disable ping display HUD if it exist
        /// </summary>
        /// <param name="color"></param>
        /// <param name="placement"></param>
        public void EnablePingDisplay(Color color, PingDisplay.Corner placement = PingDisplay.Corner.TopRight);

        /// <summary>
        ///  Disable ping display HUD if it exist
        /// </summary>
        public void DisablePingDisplay();

        #region Player Spawner

        /// <summary>
        /// Add PlayerSpawner to NetworkManager
        /// </summary>
        /// <param name="player"></param>
        /// <param name="addToDefaultScene">True to add player to the active scene when no global scenes are specified through the SceneManager</param>
        public void AddPlayerSpawner(NetworkObject player, bool addToDefaultScene = true);

        /// <summary>
        /// Remove PlayerSpawner from NetworkManager if it exist
        /// </summary>
        public void RemovePlayerSpawner();

        /// <summary>
        /// Set transform spawn points for current PlayerSpawner
        /// </summary>
        /// <param name="spawnPoints"></param>
        public void SetSpawnPointsToPlayerSpawner(Transform[] spawnPoints);

        #endregion

        public void StartLocalHost();
        
        internal UniTask ResetNetworkAfterGame();
    }
}