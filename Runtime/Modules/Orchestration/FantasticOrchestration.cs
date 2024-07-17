using System;
using System.Collections.Generic;
using FantasticNetShared.Data.Error;
using Cysharp.Threading.Tasks;
using FantasticCore.Runtime.Data;
using FantasticCore.Runtime.Debug;
using FantasticCore.Runtime.Modules.Orchestration.Data;
using FantasticCore.Runtime.Utils;
using UnityEngine;

namespace FantasticCore.Runtime.Modules.Orchestration
{
    /// <summary>
    /// Fantastic Orchestration Module Implementation
    /// </summary>
    internal sealed class FantasticOrchestration : IFantasticOrchestration
    {
        #region Fields

        private bool _isAllocationProcess;
        private bool _isServerRequestProcess;

        #region Properties
        
        public IEnumerable<Type> ModulesDependencies => null;

        #endregion

        #endregion

        #region Module

        async UniTask IFantasticModule.InitializeModule()
        {
            await UniTask.CompletedTask;
        }

        async UniTask IFantasticModule.ResetModule()
        {
            _isAllocationProcess = false;
            _isServerRequestProcess = false;
            await UniTask.CompletedTask;
        }

        #endregion
        
        public OperationHandleData<AllocationCompleteData> AllocateServer(AllocationData allocation, KeyData key)
        {
            var operation = new OperationHandleData<AllocationCompleteData>();
            if (_isAllocationProcess)
            {
                operation.SetFailed(ErrorData.Create("Allocation process already started!"));
                return operation;
            }

            _isAllocationProcess = true;

            AllocationServerProcess(operation, allocation, key);
            return operation;
        }

        private async void AllocationServerProcess(OperationHandleData<AllocationCompleteData> operation, AllocationData allocation, KeyData key)
        {
            string projectID = Application.cloudProjectId;
            string token = await WebUtils.GetAccessToken(projectID, key, allocation.EnvironmentID);
            if (string.IsNullOrEmpty(token))
            {
                operation.SetFailed(ErrorData.Create("UnCorrect Token!"));
                return;
            }

            // TODO: Move ids to config
            string urlAllocations = $"https://multiplay.services.api.unity.com/v1/allocations/projects/{projectID}/environments/{allocation.EnvironmentID}/fleets/{allocation.FleetID}/allocations";
            var allocationID = Guid.NewGuid().ToString();
            FantasticDebug.Logger.LogMessage($"Generate Allocation: {allocationID}");

            // TODO: Replace with own backend requests
            WebRequestUtils.PostJson(urlAllocations,
                unityWebRequest => { unityWebRequest.SetRequestHeader("Authorization", "Bearer " + token); },
                FantasticJson.Parse(new
                {
                    allocationId = allocationID,
                    buildConfigurationId = allocation.BuildID,
                    regionId = allocation.RegionID,
                    payload = allocation.Payload,
                    restart = allocation.Restart
                }),
                error =>
                {
                    FantasticDebug.Logger.LogMessage("Allocation failed: " + error, FantasticLogType.ERROR);
                    _isAllocationProcess = false;
                    operation.SetFailed(ErrorData.Create("Allocation failed!"));
                }, onSuccess =>
                {
                    FantasticDebug.Logger.LogMessage("Allocation success: " + onSuccess);
                    _isAllocationProcess = false;
                    operation.SetComplete(new AllocationCompleteData()
                    {
                        AllocationID = allocationID,
                        RequestData = onSuccess
                    });
                }
            );
        }

        public OperationHandleData<AllocationRequestData> GetServerConnectionData(ConfigurationData configuration, KeyData key, string allocationId)
        {
            var operation = new OperationHandleData<AllocationRequestData>();
            if (_isServerRequestProcess)
            {
                operation.SetFailed(ErrorData.Create("Get server process already started!"));
                return operation;
            }

            _isServerRequestProcess = true;
            
            GetServerProcess(configuration, key, allocationId, operation);
            return operation;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private async void GetServerProcess(ConfigurationData configuration, KeyData key, string allocationId, OperationHandleData<AllocationRequestData> operation)
        {
            string projectID = Application.cloudProjectId;
            string token = await WebUtils.GetAccessToken(projectID, key, configuration.EnvironmentID);
            if (string.IsNullOrEmpty(token))
            {
                operation.SetFailed(ErrorData.Create("UnCorrect Token!"));
                return;
            }

            string urlAllocations = $"https://multiplay.services.api.unity.com/v1/allocations/projects/{projectID}/environments/{configuration.EnvironmentID}/fleets/{configuration.FleetID}/allocations/{allocationId}";
            WebRequestUtils.Get(urlAllocations,
                unityWebRequest => { unityWebRequest.SetRequestHeader("Authorization", "Bearer " + token); },
                error =>
                {
                    FantasticDebug.Logger.LogMessage("Error: " + error, FantasticLogType.ERROR);
                    operation.SetFailed(ErrorData.Create("Get server failed!"));
                    _isServerRequestProcess = false;
                },
                OnSuccess
            );
            
            return;
            async void OnSuccess(string jsonData)
            {
                if (!_isServerRequestProcess)
                {
                    FantasticDebug.Logger.LogMessage("Request cancel!");
                    
                    operation.SetComplete(new AllocationRequestData()
                    {
                        GamePort = 0,
                        Ip = "0"
                    });
                    
                    return;
                }
                
                FantasticDebug.Logger.LogMessage("Success: " + jsonData);
                var data = FantasticJson.ParseTo<AllocationRequestData>(jsonData);
                await UniTask.Delay(TimeSpan.FromSeconds(1), DelayType.UnscaledDeltaTime);
                if (string.IsNullOrEmpty(data.Ip))
                {
                    GetServerProcess(configuration, key, allocationId, operation);
                }
                else
                {
                    operation.SetComplete(data);
                    _isServerRequestProcess = false;
                }
            }
        }

        public void CancelGetServerRequest() => _isServerRequestProcess = false;
    }
}