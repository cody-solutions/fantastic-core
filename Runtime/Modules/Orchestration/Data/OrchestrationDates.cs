using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace FantasticCore.Runtime.Modules.Orchestration.Data
{
    [Serializable]
    public class AllocationData : ConfigurationData
    {
        #region Properties

        public int BuildID { get; set; }
        
        public string RegionID { get; set; }
        
        public string Payload { get; set; }
        
        public bool Restart { get; set; }

        #endregion
    }

    [Serializable]
    public class ConfigurationData
    {
        #region Properties

        public string EnvironmentID { get; set; }
        
        public string FleetID { get; set; }

        #endregion
    }

    public struct KeyData
    {
        #region Properties

        public string KeyID { get; set; }
        
        public string KeySecret { get; set; }

        #endregion
    }
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [Serializable]
    public class AllocationRequestData
    {
        #region Properties

        [JsonProperty("gamePort")]
        public ushort GamePort { get; set; }
        
        [JsonProperty("ipv4")]
        public string Ip { get; set; }

        #endregion
    }

    public class AllocationCompleteData
    {
        #region Properties

        public string AllocationID { get; set; }
        
        public string RequestData { get; set; }

        #endregion
    }
}