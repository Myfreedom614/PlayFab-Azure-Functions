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
    public static class ListMembership2
    {
        [FunctionName("ListMembership2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {        
            // Update the PlayFabSettings with dev secret key and title ID
            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.DeveloperSecretKey))
            {
                PlayFabSettings.staticSettings.DeveloperSecretKey = Environment.GetEnvironmentVariable("PlayFab.TitleSecret", EnvironmentVariableTarget.Process);
            }

            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            {
                PlayFabSettings.staticSettings.TitleId = Environment.GetEnvironmentVariable("PlayFab.TitleId", EnvironmentVariableTarget.Process);
            }

            var titleResponse = await PlayFabAuthenticationAPI.GetEntityTokenAsync(new PlayFab.AuthenticationModels.GetEntityTokenRequest());
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
            
            var result = await PlayFabGroupsAPI.ListMembershipAsync(request);
          
            var groups = result.Result.Groups;
            var msg = $"group is {JsonConvert.SerializeObject(groups)}\n";
            return (ActionResult)new OkObjectResult($"{msg}");
        }
    }
}