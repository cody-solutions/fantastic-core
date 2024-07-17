using System.Collections.Generic;
using Unity.Services.Matchmaker.Models;
using FantasticCore.Runtime.Data;
using FantasticCore.Runtime.Modules.Matchmaking.Data;

namespace FantasticCore.Runtime.Modules.Matchmaking
{
    /// <summary>
    /// Fantastic Matchmaking Module
    /// </summary>
    public interface IFantasticMatchmaking : IFantasticModule
    {
        #region Propeties

        public bool IsMatchSearch { get; }

        #endregion
        
        /// <summary>
        /// Start a match search for players
        /// </summary>
        /// <param name="players">Matchmaker Players</param>
        /// <param name="queueName">Name of the queue being used</param>
        /// <param name="matchmakingIntervals">Additional matchmaking interval settings</param>
        public OperationHandleData<MultiplayAssignment> StartMatchSearch(List<Player> players, string queueName, MatchmakingIntervals matchmakingIntervals = null);
        
        /// <summary>
        /// Cancel match search
        /// </summary>
        public void CancelMatch();
    }
}