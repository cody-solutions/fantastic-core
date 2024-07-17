using FantasticCore.Runtime.Data;
using FantasticCore.Runtime.Modules.Orchestration.Data;

namespace FantasticCore.Runtime.Modules.Orchestration
{
    /// <summary>
    /// Fantastic Orchestration Module
    /// </summary>
    public interface IFantasticOrchestration : IFantasticModule
    {
        /// <summary>
        /// Used to allocate a new server
        /// </summary>
        /// <param name="allocation">Allocation Settings</param>
        /// <param name="key">Secret Key</param>
        public OperationHandleData<AllocationCompleteData> AllocateServer(AllocationData allocation, KeyData key);

        /// <summary>
        /// Used to retrieve allocated server data
        /// </summary>
        /// <param name="configuration">Allocated server configuration</param>
        /// <param name="key">Secret Key</param>
        /// <param name="allocationId">Allocation ID</param>
        public OperationHandleData<AllocationRequestData> GetServerConnectionData(ConfigurationData configuration, KeyData key, string allocationId);

        /// <summary>
        /// Used to cancel GetServerConnectionData.
        /// Request will be marked completed, but will return "0" ip & port.
        /// </summary>
        public void CancelGetServerRequest();
    }
}