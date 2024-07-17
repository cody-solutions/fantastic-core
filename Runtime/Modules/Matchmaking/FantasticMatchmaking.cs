using System;
using System.Collections.Generic;
using FantasticNetShared.Data.Error;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Cysharp.Threading.Tasks;
using FantasticCore.Runtime.Data;
using FantasticCore.Runtime.Debug;
using FantasticCore.Runtime.Modules.Matchmaking.Data;

namespace FantasticCore.Runtime.Modules.Matchmaking
{
    /// <summary>
    /// Fantastic Matchmaking Module Implementation
    /// </summary>
    internal sealed class FantasticMatchmaking : IFantasticMatchmaking
    {
        #region Fields
        
        private MatchmakingIntervals _matchmakingIntervals;
        private CreateTicketResponse _createTicketResponse;
        private OperationHandleData<MultiplayAssignment> _operation;

        private bool _isCancel;
        
        #region Propeties

        public IEnumerable<Type> ModulesDependencies => null;
        
        public bool IsMatchSearch => _createTicketResponse != null;
        
        private float MatchmakingCheckStatusInterval => _matchmakingIntervals.MatchmakingCheckStatusInterval;
        
        private float MinMatchmakingTimeoutTime => _matchmakingIntervals.MinMatchmakingTimeout;
        
        private float MaxMatchmakingTimeoutTime => _matchmakingIntervals.MaxMatchmakingTimeout;
        
        #endregion

        #endregion

        #region Module

        async UniTask IFantasticModule.InitializeModule()
        {
            _matchmakingIntervals = MatchmakingIntervals.Default;
            await UniTask.CompletedTask;
        }

        async UniTask IFantasticModule.ResetModule()
        {
            _createTicketResponse = null;
            await UniTask.CompletedTask;
        }

        #endregion

        public OperationHandleData<MultiplayAssignment> StartMatchSearch(List<Player> players, string queueName, MatchmakingIntervals matchmakingIntervals = null)
        {
            var operation = new OperationHandleData<MultiplayAssignment>();
            
            if (matchmakingIntervals != null)
            {
                _matchmakingIntervals = matchmakingIntervals;
            }

            FantasticDebug.Logger.LogMessage("Try find match...");
            
            if (IsMatchSearch)
            {
                FantasticDebug.Logger.LogMessage("The search for a match has already begun. If you want to cancel a past search, call CancelFindMatch.", FantasticLogType.ERROR);
                operation.SetFailed(ErrorData.Create("Match already begun."));
                return operation;
            }

            _isCancel = false;

            _operation = operation;
            MatchSearchProcess(players, queueName);
            return operation;
        }

        private async void MatchSearchProcess(List<Player> players, string queueName)
        {
            try
            {
                _createTicketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, new CreateTicketOptions
                {
                    QueueName = queueName
                });

                if (_isCancel)
                {
                    CancelHandle();
                    
                    return;
                }
                
                CheckMatchmakingInterval();
                
                float timeout = UnityEngine.Random.Range(MinMatchmakingTimeoutTime, MaxMatchmakingTimeoutTime);
                await UniTask.Delay(TimeSpan.FromSeconds(timeout));
                MatchmakingStop();
            }
            catch (Exception exception)
            {
                FantasticDebug.Logger.LogMessage($"Failed to create matchmaking ticket! Exception: {exception}", FantasticLogType.ERROR);
            }
        }

        public void CancelMatch()
        {
            if (!IsMatchSearch)
            {
                FantasticDebug.Logger.LogMessage("The match hasn't started!", FantasticLogType.ERROR);
                return;
            }
            
            FantasticDebug.Logger.LogMessage("Cancel find match!");
            _operation.SetComplete(null);
            CancelHandle();
            
            _isCancel = true;
        }

        private async void CancelHandle()
        {
            var ticketId = _createTicketResponse.Id;
            _createTicketResponse = null;
            await MatchmakerService.Instance.DeleteTicketAsync(ticketId);
        }

        private void MatchmakingStop()
        {
            if (!IsMatchSearch) return;
            
            FantasticDebug.Logger.LogMessage("Timeout or Failed!");
            CancelHandle();
            _operation.SetFailed(ErrorData.Create("Matchmaker Timeout or Failed!"));
        }

        private async void CheckMatchmakingInterval()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(MatchmakingCheckStatusInterval));
            
            while (IsMatchSearch)
            {
                CheckMatchmakingTicketStatus();
                await UniTask.Delay(TimeSpan.FromSeconds(MatchmakingCheckStatusInterval));
            }
        }
        
        private async void CheckMatchmakingTicketStatus()
        {
            if (_createTicketResponse == null)
            {
                return;
            }

            FantasticDebug.Logger.LogMessage("Check matchmaking ticket status...");
            TicketStatusResponse ticket = await MatchmakerService.Instance.GetTicketAsync(_createTicketResponse.Id);
            if (ticket == null)
            {
                FantasticDebug.Logger.LogMessage("Keep waiting matchmaking...");
                return;
            }

            if (ticket.Type != typeof(MultiplayAssignment))
            {
                FantasticDebug.Logger.LogMessage("Not multiplay assignment was ignored!");
                return;
            }

            if (ticket.Value is not MultiplayAssignment assignment)
            {
                return;
            }

            FantasticDebug.Logger.LogMessage($"Matchmaking assignment status: {assignment.Status}");
            switch (assignment.Status)
            {
                case MultiplayAssignment.StatusOptions.Timeout:
                    FantasticDebug.Logger.LogMessage("Timeout!");
                    MatchmakingStop();
                    break;
                case MultiplayAssignment.StatusOptions.Failed:
                    FantasticDebug.Logger.LogMessage("Failed multiplay service!");
                    MatchmakingStop();
                    break;
                case MultiplayAssignment.StatusOptions.InProgress:
                    FantasticDebug.Logger.LogMessage("Keep waiting matchmaking...");
                    break;
                case MultiplayAssignment.StatusOptions.Found:
                    MatchFound();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return;
            
            void MatchFound()
            {
                FantasticDebug.Logger.LogMessage($"Matchmaking match found! MatchId: {assignment.MatchId}");
                _createTicketResponse = null;
                if (assignment.Port == null)
                {
                    MatchmakingStop();
                }
                else
                {
                    _operation.SetComplete(assignment);
                }
            }
        }
    }
}