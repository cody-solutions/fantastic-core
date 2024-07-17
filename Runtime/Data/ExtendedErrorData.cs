using System.Net;
using UnityEngine.ResourceManagement.AsyncOperations;
using FantasticNetShared.Data.Error;

namespace FantasticCore.Runtime.Data
{
    public abstract class ExtendedErrorData : ErrorData
    {
        #region Constructor

        private ExtendedErrorData(string error)
            : base(error, HttpStatusCode.BadRequest, 0) { }

        #endregion

        public static ErrorData Create(AsyncOperationHandle operationHandle) 
            => Create(operationHandle.OperationException.Message);
    }
}