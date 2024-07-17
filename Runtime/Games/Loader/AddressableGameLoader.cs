using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using Cysharp.Threading.Tasks;
using FantasticNetShared.Data.Game;
using FantasticNetShared.Data.Error;
using FantasticCore.Runtime.Base_Extensions;
using FantasticCore.Runtime.Data;
using FantasticCore.Runtime.Games.Loader.Data;
using FantasticCore.Runtime.Modules.Network;

namespace FantasticCore.Runtime.Games.Loader
{
    public sealed class AddressableGameLoader : IFantasticGameLoader
    {
        #region Fields

        private const string DownloadFailed = "Game doesn't avaliable now. Please try again later...";
        private const string DownloadFailedMissingDependencies = "Game download dependencies doesn't exist now. Please try again later...";

        private GameObject[] _localDontDestroyObjects;
        
        #region Properties

        private readonly Dictionary<long, GameStatusData> _registeredGames = new();

        #endregion
        
        #endregion
        
        #region API

        /// <summary>
        /// Get game global status
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public OperationHandleData<GameStatusData> GetGameStatus(GameDTO game)
        {
            var operation = new OperationHandleData<GameStatusData>();
            var status = new GameStatusData(game, GameLoadState.CHECKING);
            PerformOperation();
            return operation;
            
            // TODO: Refactor big scope
            async void PerformOperation()
            {
                if (_registeredGames.TryGetValue(game.Id, out GameStatusData currentStatus))
                {
                    status = currentStatus;
                    if (!status.LastOperationFailed && status.GameLoadState is not (GameLoadState.READY_TO_PLAY or GameLoadState.NEED_DOWNLOAD))
                    {
                        CompleteOperation();
                        return;
                    }
                    
                    status.SetLastOperationFailed(false);
                    status.SetGameLoadState(GameLoadState.CHECK_FOR_CATALOG_UPDATE);
                    
                    OperationHandleData<CheckForGameCatalogsUpdate> checkForCatalogsUpdateOperation = CheckForCatalogUpdates();
                    await checkForCatalogsUpdateOperation.Task;
                    
                    if (checkForCatalogsUpdateOperation.Status != OperationHandleStatus.SUCCESS)
                    {
                        FailOperation(checkForCatalogsUpdateOperation.Error);
                        return;
                    }

                    if (checkForCatalogsUpdateOperation.Result.CatalogIds.Contains(status.LocatorId))
                    {
                        status.SetGameLoadState(GameLoadState.UPDATING_CATALOG);
                        
                        OperationHandleData<UpdateGameCatalogsData> updateCatalogOperation = UpdateCatalog(status.LocatorId);
                        await updateCatalogOperation.Task;
                        
                        if (updateCatalogOperation.Status != OperationHandleStatus.SUCCESS)
                        {
                            FailOperation(updateCatalogOperation.Error);
                            return;
                        }
                    }
                }
                else
                {
                    if (Enum.Parse<GameState>(game.GameState) == GameState.COMING_SOON)
                    {
                        operation.SetFailed(ErrorData.Create("Can not interact with not RELEASED game!"));
                        return;
                    }

                    if (game.RemoteCatalogUrl.IsNotValid())
                    {
                        operation.SetFailed(ErrorData.Create("Remote catalog isn't valid!"));
                        return;
                    }

                    if (PlayerPrefs.HasKey(game.Title))
                    {
                        status.SetIsDownloadedYet(true);
                    }
                    
                    _registeredGames.Add(game.Id, status);
                    status.SetGameLoadState(GameLoadState.LOADING_CATALOG);
                    
                    OperationHandleData<LoadGameCatalogData> loadOperation = LoadCatalog(game.RemoteCatalogUrl);
                    await loadOperation.Task;
                    
                    if (loadOperation.Status != OperationHandleStatus.SUCCESS)
                    {
                        FailOperation(loadOperation.Error);
                        return;
                    }
                    
                    status.SetLocatorId(loadOperation.Result.ResourceLocator.LocatorId);
                }

                status.SetGameLoadState(GameLoadState.GET_DOWNLOAD_SIZE);
                
                OperationHandleData<GameDownloadSizeData> getDownloadSizeOperation = GetDownloadSize(game.AddressableKeys);
                await getDownloadSizeOperation.Task;
                
                if (getDownloadSizeOperation.Status != OperationHandleStatus.SUCCESS)
                {
                    FailOperation(getDownloadSizeOperation.Error);
                    return;
                }

                status.SetGameLoadState(getDownloadSizeOperation.Result.IsDownloaded ? GameLoadState.READY_TO_PLAY : GameLoadState.NEED_DOWNLOAD);
                status.SetDownloadSize(getDownloadSizeOperation.Result);
                
                CompleteOperation();
                return;
                
                void CompleteOperation()
                {
                    operation.SetComplete(status);
                }

                void FailOperation(ErrorData error)
                {
                    status.SetLastOperationFailed(true);
                    operation.SetFailed(error);
                }
            }
        }
        
        /// <summary>
        /// Download game dependencies
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public OperationHandleData<GameStatusData> DownloadGame(GameDTO game)
        {
            var operation = new OperationHandleData<GameStatusData>();
            PerformOperation();
            return operation;
            
            async void PerformOperation()
            {
                if (!_registeredGames.TryGetValue(game.Id, out GameStatusData status))
                {
                    operation.SetFailed(ErrorData.Create(DownloadFailed));
                    return;
                }

                if (status.GameLoadState is GameLoadState.DOWNLOADING)
                {
                    CompleteOperation(status);
                    return;
                }

                status.SetLastOperationFailed(false);
                if (status.GameLoadState is not GameLoadState.NEED_DOWNLOAD)
                {
                    FailOperation(status, ErrorData.Create(DownloadFailedMissingDependencies));
                    return;
                }
                
                status.SetGameLoadState(GameLoadState.DOWNLOADING);
                OperationHandleData<DownloadGameDependenciesData> downloadOperation = DownloadDependencies(game.AddressableKeys, status);
                await downloadOperation.Task;

                if (downloadOperation.Status != OperationHandleStatus.SUCCESS)
                {
                    FailOperation(status, downloadOperation.Error);
                    return;
                }

                PlayerPrefs.SetString(game.Title, true.ToString());
                PlayerPrefs.Save();
                status.SetIsDownloadedYet(true);
                status.SetGameLoadState(GameLoadState.READY_TO_PLAY);
                CompleteOperation(status);
            }

            void CompleteOperation(GameStatusData status)
            {
                operation.SetComplete(status);
            }

            void FailOperation(GameStatusData status, ErrorData error)
            {
                status.SetLastOperationFailed(true);
                operation.SetFailed(error);
            }
        }

        /// <summary>
        /// Clear game cache dependencies
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public async UniTask ClearGameCache(GameDTO game)
        {
            if (!_registeredGames.TryGetValue(game.Id, out GameStatusData status))
            {
                return;
            }

            if (game.RemoteCatalogUrl == null && game.AddressableKeys == null)
            {
                return;
            }
            
            if (!status.LastOperationFailed && status.GameLoadState is not (GameLoadState.READY_TO_PLAY or GameLoadState.NEED_DOWNLOAD))
            {
                return;
            }
            
            status.SetIsDownloadedYet(false);
            PlayerPrefs.DeleteKey(game.Title);
            PlayerPrefs.Save();
            await ClearDependencyCacheAsync(game.AddressableKeys);
        }

        // ReSharper disable once UnusedMember.Global
        public async UniTask PlayGame(GameDTO game)
        {
            AsyncOperationHandle<SceneInstance> operation = Addressables.LoadSceneAsync(game.BootAddressableKey);
            await operation.Task;
        }

        // ReSharper disable once UnusedMember.Global
        public async UniTask ExitGame()
        {
            await ClearGameLocalData();
            AsyncOperationHandle<SceneInstance> operation = Addressables.LoadSceneAsync(FantasticProperties.HubScene);
            await operation.Task;
        }
        
        #endregion

        #region Catalogs

        /// <summary>
        /// Load remote catalog. Pass url with {PLATFORM} pattern. Return operation result
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private OperationHandleData<LoadGameCatalogData> LoadCatalog(string url)
        {
            var operation = new OperationHandleData<LoadGameCatalogData>();
            PerformOperation();
            return operation;
            
            async void PerformOperation()
            {
                // ReSharper disable once JoinDeclarationAndInitializer initialize
                string suffix;
#if UNITY_ANDROID
                    suffix = "Android";
#elif UNITY_IOS
                    suffix = "IOS";
#elif UNITY_STANDALONE_WIN
                suffix = "StandaloneWindows64";
#elif UNITY_STANDALONE_OSX
                    suffix = "OSX";
#else
                    suffix = "WebGL";
#endif
                string finalUrl = url.Replace("{PLATFORM}", suffix);
                AsyncOperationHandle<IResourceLocator> loadCatalogOperation =
                    Addressables.LoadContentCatalogAsync(finalUrl, false);
                while (!loadCatalogOperation.IsDone)
                {
                    await UniTask.Yield();
                }
                
                if (loadCatalogOperation.Status != AsyncOperationStatus.Succeeded)
                {
                    operation.SetFailed(ExtendedErrorData.Create(loadCatalogOperation));
                    return;
                }
                
                operation.SetComplete(LoadGameCatalogData.Create(loadCatalogOperation.Result));
                Addressables.Release(loadCatalogOperation);
            }
        }

        /// <summary>
        /// Check catalog for updates. Return result operation
        /// </summary>
        /// <returns>Result operation</returns>
        private OperationHandleData<CheckForGameCatalogsUpdate> CheckForCatalogUpdates()
        {
            var operation = new OperationHandleData<CheckForGameCatalogsUpdate>();
            PerformOperation();
            return operation;
            
            async void PerformOperation()
            {
                AsyncOperationHandle<List<string>> catalogUpdatesOperation = Addressables.CheckForCatalogUpdates(false);
                await catalogUpdatesOperation.Task;

                if (catalogUpdatesOperation.Status != AsyncOperationStatus.Succeeded)
                {
                    operation.SetFailed(ExtendedErrorData.Create(catalogUpdatesOperation));
                    return;
                }

                foreach (string s in catalogUpdatesOperation.Result)
                {
                    UnityEngine.Debug.Log($"Checked: {s}");
                }
                
                operation.SetComplete(CheckForGameCatalogsUpdate.Create(catalogUpdatesOperation.Result));
                Addressables.Release(catalogUpdatesOperation);
            }
        }

        /// <summary>
        /// Update catalogs. Return result operation
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns>Result operation</returns>
        private OperationHandleData<UpdateGameCatalogsData> UpdateCatalog(string catalog)
            => UpdateCatalogs(new[] { catalog });

        /// <summary>
        /// Update catalogs. Return result operation
        /// </summary>
        /// <param name="catalogs"></param>
        /// <returns>Result operation</returns>
        private static OperationHandleData<UpdateGameCatalogsData> UpdateCatalogs(IEnumerable<string> catalogs)
        {
            IEnumerable<string> enumerable = catalogs.ToList();
            foreach (string catalog in enumerable)
            {
                UnityEngine.Debug.Log(catalog);
            }
            var operation = new OperationHandleData<UpdateGameCatalogsData>();
            PerformOperation();
            return operation;
            
            async void PerformOperation()
            {
                AsyncOperationHandle updateCatalogsOperation = Addressables.UpdateCatalogs(true, enumerable, false);
                while (!updateCatalogsOperation.IsDone)
                {
                    UnityEngine.Debug.Log(updateCatalogsOperation.GetDownloadStatus().Percent);
                    UnityEngine.Debug.Log(updateCatalogsOperation.GetDownloadStatus().TotalBytes);
                    await UniTask.Yield();
                }
                
                if (updateCatalogsOperation.Status != AsyncOperationStatus.Succeeded)
                {
                    operation.SetFailed(ExtendedErrorData.Create(updateCatalogsOperation));
                    return;
                }
                
                operation.SetComplete(UpdateGameCatalogsData.Create(updateCatalogsOperation.Result));
                Addressables.Release(updateCatalogsOperation);
            }
        }

        #endregion
        
        #region Download Size

        /// <summary>
        /// Get download size(in bytes). Return operation result
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private OperationHandleData<GameDownloadSizeData> GetDownloadSize(IEnumerable<string> keys)
        {
            var operation = new OperationHandleData<GameDownloadSizeData>();
            PerformOperation();
            return operation;
            
            async void PerformOperation()
            {
                AsyncOperationHandle<long> downloadSizeOperation = Addressables.GetDownloadSizeAsync(keys);
                await downloadSizeOperation.Task;

                if (downloadSizeOperation.Status != AsyncOperationStatus.Succeeded)
                {
                    operation.SetFailed(ExtendedErrorData.Create(downloadSizeOperation));
                    return;
                }

                operation.SetComplete(GameDownloadSizeData.Create(downloadSizeOperation.Result));
                Addressables.Release(downloadSizeOperation);
            }
        }

        /// <summary>
        /// Download dependencies. Return operation result
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="status"></param>
        private OperationHandleData<DownloadGameDependenciesData> DownloadDependencies(IEnumerable<string> keys,
            GameStatusData status)
        {
            var operation = new OperationHandleData<DownloadGameDependenciesData>();
            PerformOperation();
            return operation;

            async void PerformOperation()
            {
                // ReSharper disable once RedundantArgumentDefaultValue
                AsyncOperationHandle downloadDependenciesOperation =
                    Addressables.DownloadDependenciesAsync(keys, 0, false);
                while (!downloadDependenciesOperation.IsDone)
                {
                    SetOperationProgress();
                    await UniTask.Yield();
                }

                SetOperationProgress();
                if (downloadDependenciesOperation.Status != AsyncOperationStatus.Succeeded)
                {
                    operation.SetFailed(ExtendedErrorData.Create(downloadDependenciesOperation));
                    return;
                }

                operation.SetComplete(DownloadGameDependenciesData.Create());
                Addressables.Release(downloadDependenciesOperation);
                return;

                void SetOperationProgress()
                {
                    status.SetProgress(downloadDependenciesOperation.PercentComplete);
                    status.SetDownloadStatus(downloadDependenciesOperation.GetDownloadStatus());
                }
            }
        }

        #endregion

        #region Clear

        /// <summary>
        /// Clear dependency cache from disk space
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="autoRelease"></param>
        /// <returns></returns>
        private async UniTask<bool> ClearDependencyCacheAsync(IEnumerable<string> keys, bool autoRelease = true)
        {
            AsyncOperationHandle<bool> clearDependencyCacheOperation = Addressables.ClearDependencyCacheAsync(keys, false);
            await clearDependencyCacheOperation.Task;
            bool result = clearDependencyCacheOperation.Result;
            if (autoRelease)
            {
                Addressables.Release(clearDependencyCacheOperation);
            }
            return result;
        }

        /// <summary>
        /// Clean and remove unused bundles from disk space
        /// </summary>
        /// <param name="catalogsIds"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Local
        private static async UniTask<bool> CleanBundleCache(IEnumerable<string> catalogsIds = null)
        {
            AsyncOperationHandle<bool> cleanBundleOperation = Addressables.CleanBundleCache(catalogsIds);
            await cleanBundleOperation.Task;
            return cleanBundleOperation.Result;
        }

        #endregion

        void IFantasticGameLoader.RegisterGameDontDestroyObjects(GameObject[] objects)
            => _localDontDestroyObjects = objects;

        private async UniTask ClearGameLocalData()
        {
            if (FantasticInstance.TryGetModule(out IFantasticNetwork fantasticNetwork))
            {
                await fantasticNetwork.ResetNetworkAfterGame();
            }

            if (!_localDontDestroyObjects.IsArrayValidAndNotEmpty())
            {
                return;
            }

            foreach (GameObject go in _localDontDestroyObjects)
            {
                UnityEngine.Object.Destroy(go);
            }
        }
    }
}