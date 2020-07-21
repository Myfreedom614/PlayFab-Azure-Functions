using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlayFab.ServerModels;
using System;

namespace PlayFab.Samples
{
    public static class GetCharacterInventoryFunc
    {
        [FunctionName("GetCharacterInventoryFunc")]
        public static async Task<dynamic> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"{nameof(GetCharacterInventoryFunc)} C# HTTP trigger function processed a request.");
            var serverSettings = new PlayFab.PlayFabApiSettings()
            {
                TitleId = Environment.GetEnvironmentVariable("PlayFab.TitleId", EnvironmentVariableTarget.Process),
                DeveloperSecretKey = Environment.GetEnvironmentVariable("PlayFab.TitleSecret", EnvironmentVariableTarget.Process),
            };
            
            var authAPI = new PlayFabAuthenticationInstanceAPI(serverSettings);
            var titleResponse = await authAPI.GetEntityTokenAsync(new PlayFab.AuthenticationModels.GetEntityTokenRequest());
            var title = titleResponse.Result.Entity;
            var titleToken = titleResponse.Result.EntityToken;

            var titleAuthContext = new PlayFabAuthenticationContext();
            titleAuthContext.EntityToken = titleToken;
            
            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
            dynamic args = context.FunctionArgument;
            var message = $"Args: {args}";
            log.LogInformation(message);

            var request = new GetCharacterInventoryRequest {
                PlayFabId = args["playfabId"] ?? context.CallerEntityProfile.Lineage.MasterPlayerAccountId,
                CharacterId  = args["characterId"]
            };
            
            var serverApi = new PlayFabServerInstanceAPI(serverSettings, titleAuthContext);

            return await serverApi.GetCharacterInventoryAsync(request);
        }
    }
}