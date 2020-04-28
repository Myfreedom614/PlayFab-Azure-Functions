using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlayFab.Samples;
using PlayFab;
 
namespace PlayFab.Samples
{
    public static class ListMembership
    {
        [FunctionName("ListMembership")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {        
            var serverSettings = new PlayFab.PlayFabApiSettings()
            {
                TitleId = Environment.GetEnvironmentVariable("PlayFab.TitleId", EnvironmentVariableTarget.Process),
                DeveloperSecretKey = Environment.GetEnvironmentVariable("PlayFab.TitleSecret", EnvironmentVariableTarget.Process),
            };

            var authAPI = new PlayFabAuthenticationInstanceAPI(serverSettings);
            var titleResponse = await authAPI.GetEntityTokenAsync(new PlayFab.AuthenticationModels.GetEntityTokenRequest());
            var title = titleResponse.Result.Entity;
            var titleToken = titleResponse.Result.EntityToken;
 
            log.LogInformation($"Title is  : {title.Id}");
            log.LogInformation($"Token is  : {titleToken}");
    
            var request = new PlayFab.GroupsModels.ListMembershipRequest()
            {
                Entity = new PlayFab.GroupsModels.EntityKey
                {
                    Id = "7B66887BFE1A76CE",
                    Type = "title_player_account",
                }
            };
 
            log.LogInformation($"Request is  : {JsonConvert.SerializeObject(request)}");
            
            var titleAuthContext = new PlayFabAuthenticationContext();
            titleAuthContext.EntityToken = titleToken;

            var api = new PlayFabGroupsInstanceAPI(serverSettings, titleAuthContext);
            var result = await api.ListMembershipAsync(request);

            //var result = await PlayFabGroupsAPI.ListMembershipAsync(request);
          
            var groups = result.Result.Groups;
            var msg = $"group is {JsonConvert.SerializeObject(groups)}\n";
            return (ActionResult)new OkObjectResult($"{msg}");
        }
    }
}