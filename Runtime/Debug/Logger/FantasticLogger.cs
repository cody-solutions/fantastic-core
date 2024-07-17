using System;
using UnityDebug = UnityEngine.Debug;

namespace FantasticCore.Runtime.Debug.Logger
{
    internal class FantasticLogger : IFantasticLogger
    {
        public void LogMessage(string prefix, string message, FantasticLogType logType = FantasticLogType.INFO)
        {
            switch (logType)
            {
                case FantasticLogType.INFO:
                    UnityDebug.Log($"[{prefix}] -> {message}");
                    break;
                case FantasticLogType.WARN:
                    UnityDebug.LogWarning($"[{prefix}] -> {message}");
                    break;
                case FantasticLogType.ERROR:
                    UnityDebug.LogError($"[{prefix}] -> {message}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
        }

        public void LogMessage(string message, FantasticLogType logType = FantasticLogType.INFO)
            => LogMessage(FantasticProperties.PackageName, message, logType);

        void IFantasticLogger.Setup() { }
    }
}