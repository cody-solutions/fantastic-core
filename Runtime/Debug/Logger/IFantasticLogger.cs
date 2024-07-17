namespace FantasticCore.Runtime.Debug.Logger
{
    public interface IFantasticLogger
    {
        public void LogMessage(string prefix, string message,
            FantasticLogType logType = FantasticLogType.INFO);

        public void LogMessage(string message, FantasticLogType logType = FantasticLogType.INFO);

        internal void Setup();
    }
}