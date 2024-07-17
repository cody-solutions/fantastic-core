using FantasticCore.Runtime.Debug;

namespace FantasticCore.Runtime.Base_Extensions
{
    public static class StringExtensions
    {
        public static bool IsValid(this string value)
            => !string.IsNullOrWhiteSpace(value);

        public static bool IsNotValid(this string value, bool logIfNotValid = false, string logMessage = null)
        {
            bool isNotValid = !IsValid(value);
            if (logIfNotValid && isNotValid)
            {
                FantasticDebug.Logger.LogMessage(logMessage, FantasticLogType.ERROR);
            }
            return isNotValid;
        }
    }
}