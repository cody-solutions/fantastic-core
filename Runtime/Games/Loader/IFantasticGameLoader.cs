using UnityEngine;
using Cysharp.Threading.Tasks;
using FantasticNetShared.Data.Game;
using FantasticCore.Runtime.Data;
using FantasticCore.Runtime.Games.Loader.Data;

namespace FantasticCore.Runtime.Games.Loader
{
    public interface IFantasticGameLoader
    {
        public OperationHandleData<GameStatusData> GetGameStatus(GameDTO game);

        public OperationHandleData<GameStatusData> DownloadGame(GameDTO game);

        public UniTask ClearGameCache(GameDTO game);

        public UniTask PlayGame(GameDTO game);

        public UniTask ExitGame();

        internal void RegisterGameDontDestroyObjects(GameObject[] objects);
    }
}