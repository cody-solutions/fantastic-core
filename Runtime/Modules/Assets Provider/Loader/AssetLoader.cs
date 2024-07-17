using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using FantasticCore.Runtime.Base_Extensions;
using FantasticCore.Runtime.Debug;

namespace FantasticCore.Runtime.Modules.Assets_Provider.Loader
{
    /// <summary>
    /// Base loader interface
    /// </summary>
    public interface IBaseAssetLoader { }
    
    /// <summary>
    /// Loader class for addressable asset
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AssetLoader<T> : IBaseAssetLoader where T : MonoBehaviour
    {
        #region Properties

        public bool IsLoaded => LoadedAsset != null;
        
        protected abstract string AssetId { get; }

        protected T LoadedAsset { get; private set; }
        
        #endregion

        public async UniTask<T> TryLoad(bool activateAfterLoad = true,
            bool dontDestroyOnload = false)
        {
            if (IsLoaded)
            {
                return LoadedAsset;
            }

            AsyncOperationHandle<GameObject> operationHandle = Addressables.InstantiateAsync(AssetId);
            await operationHandle.Task;
            if (operationHandle.Status != AsyncOperationStatus.Succeeded)
            {
                FantasticDebug.Logger.LogMessage($"Can't load {AssetId} asset! Error: {operationHandle.OperationException}",
                    FantasticLogType.ERROR);
                return null;
            }

            if (!operationHandle.Result.TryGetComponent(out T asset))
            {
                FantasticDebug.Logger.LogMessage($"Loaded {AssetId} asset missing {typeof(T).Name} component! Unloading it...",
                    FantasticLogType.ERROR);
                Addressables.Release(operationHandle);
                return null;
            }

            LoadedAsset = asset;
            if (activateAfterLoad)
            {
                LoadedAsset.gameObject.Activate();
            }

            if (dontDestroyOnload)
            {
                Object.DontDestroyOnLoad(LoadedAsset);
            }
            return LoadedAsset;
        }

        public bool TryUnLoad()
        {
            if (!IsLoaded)
            {
                return false;
            }

            GameObject loadedGameObject;
            (loadedGameObject = LoadedAsset.gameObject).Deactivate();
            LoadedAsset = null;
            Addressables.ReleaseInstance(loadedGameObject);
            return true;
        }

        protected bool TryGet(out T asset)
        {
            if (IsLoaded)
            {
                return LoadedAsset.TryGetComponent(out asset);
            }

            FantasticDebug.Logger.LogMessage($"Can't get {AssetId} asset. Load it first!", FantasticLogType.ERROR);
            asset = null;
            return false;
        }
    }
}