using System.Collections.Generic;
using FantasticCore.Runtime.Base_Extensions;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using JetBrains.Annotations;
using FantasticNetShared.Data.Game;

namespace FantasticCore.Runtime.Games.Loader.Data
{
    public sealed class LoadGameCatalogData
    {
        #region Properties

        public IResourceLocator ResourceLocator { get; private set; }

        #endregion

        #region Constructor

        private LoadGameCatalogData(IResourceLocator resourceLocator)
        {
            ResourceLocator = resourceLocator;
        }

        #endregion
        
        public static LoadGameCatalogData Create(IResourceLocator resourceLocator)
            => new(resourceLocator);
    }

    public sealed class GameDownloadSizeData
    {
        #region Properties

        // ReSharper disable once MemberCanBePrivate.Global
        public long Size { get; }

        [CanBeNull]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string FormattedSize { get; private set; }

        // ReSharper disable once UnusedMember.Global
        public bool IsDownloaded => Size == 0;

        #endregion

        #region Constructor

        private GameDownloadSizeData(long size, bool formatSize = true)
        {
            Size = size;
            if (formatSize)
            {
                FormatSize();
            }
        }

        #endregion

        public static GameDownloadSizeData Create(long size, bool formatSize = true)
            => new(size, formatSize);

        private void FormatSize()
            => FormattedSize = Size.FormatDownloadBytes();
    }

    public sealed class DownloadGameDependenciesData
    {
        #region Constructor

        private DownloadGameDependenciesData()
        {
        }

        #endregion

        public static DownloadGameDependenciesData Create()
            => new();
    }

    public sealed class CheckForGameCatalogsUpdate
    {
        #region Properties

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public List<string> CatalogIds { get; private set; }

        #endregion
        
        #region Constructor

        private CheckForGameCatalogsUpdate(List<string> catalogIds)
            => CatalogIds = catalogIds;

        #endregion
        
        public static CheckForGameCatalogsUpdate Create(List<string> catalogIds)
            => new(catalogIds);
    }

    public sealed class UpdateGameCatalogsData
    {
        #region Properties

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public object Result { get; private set; }

        #endregion

        #region Constructor

        private UpdateGameCatalogsData(object result)
            => Result = result;

        #endregion
        
        public static UpdateGameCatalogsData Create(object result)
            => new(result);
    }

    public sealed class GameStatusData
    {
        #region Properties

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public GameDTO Game { get; private set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public GameLoadState GameLoadState { get; private set; }

        /// <summary>
        /// Addressable resource locator id
        /// </summary>
        public string LocatorId { get; private set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public GameDownloadSizeData DownloadSizeData { get; private set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public bool IsDownloadedYet { get; private set; }
        
        /// <summary>
        /// Operation download status
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DownloadStatus DownloadStatus { get; private set; }
        
        /// <summary>
        /// Operation progress
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public float Progress { get; private set; }

        public bool LastOperationFailed { get; private set; }

        #endregion

        #region Constructor

        public GameStatusData(GameDTO game, GameLoadState gameLoadState, string localLocatorId = null)
        {
            Game = game;
            GameLoadState = gameLoadState;
            LocatorId = localLocatorId;
        }

        #endregion

        public void SetGameLoadState(GameLoadState gameLoadState)
            => GameLoadState = gameLoadState;

        public void SetDownloadSize(GameDownloadSizeData downloadSizeData)
            => DownloadSizeData = downloadSizeData;

        public void SetLocatorId(string locatorId)
            => LocatorId = locatorId;

        public void SetIsDownloadedYet(bool isDownloaded)
            => IsDownloadedYet = isDownloaded;
        
        /// <summary>
        /// Set <see cref="Progress"/>>
        /// </summary>
        /// <param name="value"></param>
        public void SetProgress(float value)
            =>  Progress = value;

        /// <summary>
        /// Set <see cref="DownloadStatus"/>
        /// </summary>
        /// <param name="status"></param>
        public void SetDownloadStatus(DownloadStatus status)
            => DownloadStatus = status;

        /// <summary>
        /// Set is last operation failed
        /// </summary>
        /// <param name="state"></param>
        public void SetLastOperationFailed(bool state)
            => LastOperationFailed = state;
    }
}