using System;
using Cysharp.Threading.Tasks;
using FantasticNetShared.Data.Error;
using FantasticCore.Runtime.Debug;

namespace FantasticCore.Runtime.Data
{
    public class OperationHandleData<TData> : IDisposable where TData : class
    {
        #region Fields

        // Events will call before finish Task
        public event Action<TData> CompleteEvent;
        public event Action<ErrorData> FailedEvent;
        
        private UniTaskCompletionSource _completionSource;

        #region Properties

        /// <summary>
        /// Result operation data. Null if operation is failed
        /// </summary>
        public TData Result { get; private set; }

        /// <summary>
        /// Determine if operation is finished
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public bool IsDone { get; private set; }

        /// <summary>
        /// Current operation status
        /// </summary>
        public OperationHandleStatus Status { get; private set; } = OperationHandleStatus.PROCESS;

        /// <summary>
        /// Can wait current operation, will done after events
        /// </summary>
        public UniTask Task { get; private set; }

        /// <summary>
        /// Operation failed details. Null if <see cref="Status"/> = is SUCCESS
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ErrorData Error { get; private set; }
        
        #endregion
        
        #endregion

        #region Constructor

        public OperationHandleData()
        {
            CreateTask();
        }

        #endregion

        /// <summary>
        /// IDisposable
        /// </summary>
        public void Dispose()
        {
            _completionSource = null;
            CompleteEvent = null;
            FailedEvent = null;
        }

        /// <summary>
        /// Complete operation with success
        /// </summary>
        /// <param name="result"></param>
        public void SetComplete(TData result)
        {
            Status = OperationHandleStatus.SUCCESS;
            SetResult(result);
            CompleteEvent?.Invoke(result);
            CompleteOperation();
        }

        /// <summary>
        /// Complete operation with error
        /// </summary>
        /// <param name="error"></param>
        public void SetFailed(ErrorData error)
        {
            Status = OperationHandleStatus.FAIL;
            Error = error;
            FailedEvent?.Invoke(error);
            CompleteOperation();
        }

        private void SetResult(TData result)
            => Result = result;
        
        private void CompleteOperation()
        {
            SetIsDone(true);
            _completionSource?.TrySetResult();
            Dispose(); // TODO: Dispose ?
        }

        private void CreateTask()
        {
            if (_completionSource != null)
            {
                FantasticDebug.Logger.LogMessage("CompletionSource is already created!", FantasticLogType.WARN);
                return;
            }

            _completionSource = new UniTaskCompletionSource();
            Task = _completionSource.Task;
        }
        
        private void SetIsDone(bool state)
            => IsDone = state;
    }
}