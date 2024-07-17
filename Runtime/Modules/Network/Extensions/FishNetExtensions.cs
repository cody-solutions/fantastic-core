using UnityEngine;
using FishNet.Component.Spawning;
using FishNet.Component.Utility;
using FishNet.Object;

namespace FantasticCore.Runtime.Modules.Network.Extensions
{
    public static class FishNetExtensions
    {
        #region Player Spawner

        public static void Setup(this PlayerSpawner playerSpawner,
            NetworkObject playerPrefab, bool addToDefaultScene)
        {
            playerSpawner._playerPrefab = playerPrefab;
            playerSpawner._addToDefaultScene = addToDefaultScene;
        }

        public static void SetSpawnPoints(this PlayerSpawner playerSpawner, Transform[] spawnPoints)
            => playerSpawner.Spawns = spawnPoints;

        #endregion

        #region Ping Display

        public static void SetStyle(this PingDisplay pingDisplay,
            Color color, PingDisplay.Corner placement)
        {
            pingDisplay._color = color;
            pingDisplay._placement = placement;
        }

        #endregion
    }
}