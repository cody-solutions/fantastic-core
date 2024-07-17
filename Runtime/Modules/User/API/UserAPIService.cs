using Cysharp.Threading.Tasks;
using FantasticCore.Runtime.API;
using FantasticCore.Runtime.API.Data;
using FantasticNetShared.Data.Game.Category;

namespace FantasticCore.Runtime.Modules.User.API
{
    public static class UserAPIService
    {
        public static async UniTask<APIResponseDataT<GameCategoriesDTO>> GetGamesTab(
            APIRequestData requestData = null)
        {
            requestData ??= new APIRequestData();
            requestData.SetRequestDetails("User/GetGamesTab");
            return APIResponseDataT<GameCategoriesDTO>
                .Create(await FantasticAPIClient.SendRequest(requestData));
        }
    }
}