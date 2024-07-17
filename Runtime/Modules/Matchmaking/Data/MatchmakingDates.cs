namespace FantasticCore.Runtime.Modules.Matchmaking.Data
{
    public class MatchmakingIntervals
    {
        #region Properties

        public float MatchmakingCheckStatusInterval { get; set; }
        public float MinMatchmakingTimeout { get; set; }
        public float MaxMatchmakingTimeout { get; set; }

        #endregion

        public static MatchmakingIntervals Default => new()
        {
            MatchmakingCheckStatusInterval = 1.1f,
            MinMatchmakingTimeout = 3.0f,
            MaxMatchmakingTimeout = 6.0f
        };
    }
}