using FantasticCore.Runtime.API.Data;
using UnityEngine.Networking;

namespace FantasticCore.Runtime.API.Extensions
{
    public static class APIServiceExtensions
    {
        /// <summary>
        /// Get response API text
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetResponseText(this UnityWebRequest request)
            => request.downloadHandler.text;

        public static void SetFormDataSectionIfValid(this APIRequestData requestData, string sectionName, int? value)
        {
            if (value != null)
            {
                requestData.Form.Add(new MultipartFormDataSection(sectionName, value.ToString()));
            }
        }
        
        public static void SetFormDataSectionIfValid(this APIRequestData requestData, string sectionName, bool? value)
        {
            if (value != null)
            {
                requestData.Form.Add(new MultipartFormDataSection(sectionName, value.ToString()));
            }
        }
    }
}