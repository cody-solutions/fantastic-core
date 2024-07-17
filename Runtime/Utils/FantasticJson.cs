using System;
using FantasticCore.Runtime.Base_Extensions;
using FantasticCore.Runtime.Debug;
using Newtonsoft.Json;

namespace FantasticCore.Runtime.Utils
{
    public static class FantasticJson
    {
        public static string Parse(object data)
        {
            try
            {
                if (data == null)
                {
                    FantasticDebug.Logger.LogMessage("Parse data is not valid!", FantasticLogType.ERROR);
                    return null;
                }

                var parse = JsonConvert.SerializeObject(data);
                return parse;
            }
            catch (Exception exception)
            {
                FantasticDebug.Logger.LogMessage($"Can not parse from data. Error: {exception.Message}", FantasticLogType.ERROR);
                return null;
            }
        }

        public static T ParseTo<T>(string data, bool logErrorIfFailed = true) where T : class
        {
            try
            {
                if (data.IsNotValid(true, "Parse from data is not valid!"))
                {
                    return null;
                }

                var parsedData = JsonConvert.DeserializeObject<T>(data);
                return parsedData;
            }
            catch (Exception exception)
            {
                if (logErrorIfFailed)
                {
                    FantasticDebug.Logger.LogMessage($"Can not parse to {typeof(T)} type. Error: {exception.Message}", FantasticLogType.ERROR);
                }
                return null;
            }
        }
    }
}