using FantasticCore.Runtime.Modules.Assets_Provider.Loader;

namespace FantasticCore.Runtime.Modules.Assets_Provider
{
    /// <summary>
    /// Fantastic Auth Module
    /// </summary>
    public interface IFantasticAssetsProvider : IFantasticModule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterAssetLoader<T>() where T : IBaseAssetLoader, new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void UnRegisterAssetLoader<T>() where T : IBaseAssetLoader;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loader"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool TryGetAssetLoader<T>(out T loader) where T : class, IBaseAssetLoader;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logErrorIfNot"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool IsLoaderRegistered<T>(bool logErrorIfNot = false) where T : IBaseAssetLoader;
    }
}