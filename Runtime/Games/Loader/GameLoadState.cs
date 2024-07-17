namespace FantasticCore.Runtime.Games.Loader
{
    public enum GameLoadState
    {
        CHECKING,
        GET_DOWNLOAD_SIZE,
        NEED_DOWNLOAD,
        DOWNLOADING,
        CHECK_FOR_CATALOG_UPDATE,
        LOADING_CATALOG,
        UPDATING_CATALOG,
        READY_TO_PLAY
    }
}