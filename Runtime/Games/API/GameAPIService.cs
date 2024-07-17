using System.Collections.Generic;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using FantasticCore.Runtime.API;
using FantasticCore.Runtime.API.Data;
using FantasticNetShared.Data.Game;

namespace FantasticCore.Runtime.Games.API
{
    public static class GameAPIService
    {
        public static async UniTask<APIResponseDataT<GameListData>> GetAllGames(APIRequestData requestData = null)
        {
            requestData ??= new APIRequestData();
            requestData.SetRequestDetails("Game/GetAllGames");
            return APIResponseDataT<GameListData>.Create(await FantasticAPIClient.SendRequest(requestData));
        }
        
        public static async UniTask<APIResponseDataT<GameDTO>> GetGame(long gameId, APIRequestData requestData = null)
        {
            requestData ??= new APIRequestData();
            requestData.SetRequestDetails("Game/GetGame", new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("gameId", gameId.ToString())
            });
            return APIResponseDataT<GameDTO>.Create(await FantasticAPIClient.SendRequest(requestData));
        }
        
        public static async UniTask<APIResponseDataT<GameDTO>> GetGamesInCategory(long categoryId, APIRequestData requestData = null)
        {
            requestData ??= new APIRequestData();
            requestData.SetRequestDetails("Game/GetGamesInCategory", new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("categoryId", categoryId.ToString())
            });
            return APIResponseDataT<GameDTO>.Create(await FantasticAPIClient.SendRequest(requestData));
        }
        
        public static async UniTask<APIResponseDataT<GameListData>> GetGamesInCategory(string categoryTitle, APIRequestData requestData = null)
        {
            requestData ??= new APIRequestData();
            requestData.SetRequestDetails("Game/GetGamesInCategory", new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("categoryTitle", categoryTitle)
            });
            return APIResponseDataT<GameListData>.Create(await FantasticAPIClient.SendRequest(requestData));
        }

        #region User

        public static async UniTask<APIResponseDataT<GameRateData>> RateGame(long userId, long gameId, int rating,
            APIRequestData requestData = null)
        {
            requestData ??= new APIRequestData();
            requestData.SetRequestDetails("Game/RateGame", new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("userId", userId.ToString()),
                new MultipartFormDataSection("gameId", gameId.ToString()),
                new MultipartFormDataSection("rating", rating.ToString())
            }, RequestType.POST);
            return APIResponseDataT<GameRateData>.Create(await FantasticAPIClient.SendRequest(requestData));
        }
        
        public static async UniTask<APIResponseDataT<GameUserFavoriteData>> ToggleFavoriteGame(long userId, long gameId, bool favoriteState,
            APIRequestData requestData = null)
        {
            requestData ??= new APIRequestData();
            requestData.SetRequestDetails("Game/ToggleFavoriteGame", new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("userId", userId.ToString()),
                new MultipartFormDataSection("gameId", gameId.ToString()),
                new MultipartFormDataSection("favoriteState", favoriteState.ToString())
            }, RequestType.POST);
            return APIResponseDataT<GameUserFavoriteData>.Create(await FantasticAPIClient.SendRequest(requestData));
        }

        #endregion
    }
}