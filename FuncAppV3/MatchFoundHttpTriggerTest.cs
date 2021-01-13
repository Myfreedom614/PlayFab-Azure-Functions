using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System;
using System.Linq;

namespace PlayFab.Samples
{
    public class DO
    {
        public dynamic DataObject { get; set; }
    }
    public class Member
    {
        public dynamic Entity { get; set; }
        public DO Attributes { get; set; }
    }
    public static class MatchFoundHttpTriggerTest
    {
        [FunctionName("MatchFoundHttpTriggerTest")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"{nameof(MatchFoundHttpTriggerTest)} C# HTTP trigger function processed a request.");

            var reqStr = await req.ReadAsStringAsync();
            var context = JsonConvert.DeserializeObject<dynamic>(reqStr);
            
            var msg = $"context is {JsonConvert.SerializeObject(context)}\n";
            log.LogInformation($"{msg}");
            
            var authCtx = context?.TitleAuthenticationContext;
            var payload = context?.PlayStreamEvent?.Payload;

            if(payload != null && authCtx != null){
                var titleId = authCtx?.Id;
                var eToken = authCtx?.EntityToken;
                var matchId = payload?.MatchId;
                var queueName = payload?.QueueName;
                //log.LogInformation($"MatchId: {matchId}, QueueName: {queueName}");
                
                var matchData = await GetMatchRequest(Convert.ToString(titleId), Convert.ToString(eToken), Convert.ToString(matchId), Convert.ToString(queueName), log);

                var members = matchData?.Members;
                var msg2 = $"Members is {JsonConvert.SerializeObject(members)}\n";
                log.LogInformation($"{msg2}");

                var ms = JsonConvert.DeserializeObject<List<Member>>(JsonConvert.SerializeObject(members));
                var matchIPs = new List<string>();
                foreach(var m in ms){
                    string ip = m.Attributes.DataObject?.MatchIP;
                    if(!string.IsNullOrEmpty(ip)) matchIPs.Add(ip);
                }

                matchIPs = matchIPs.Distinct().ToList();

                matchIPs.ForEach(x => { SendToMatchServerRequest(x, "9000", Convert.ToString(matchId), Convert.ToString(queueName), log); });

                return (ActionResult)new OkObjectResult($"Success");
            }
            
            return (ActionResult)new BadRequestObjectResult($"Authentication Context or PlayStream Payload is missing");
        }

        static async Task<dynamic> GetMatchRequest(string titleId, string eToken,string matchId, string queueName, ILogger log){
            log.LogInformation($"{titleId}\n{eToken}\n{matchId}\n{queueName}");
            var req = new {
                MatchId = matchId,
                QueueName = queueName,
                EscapeObject = false,
                ReturnMemberAttributes = true
                };
            var json = JsonConvert.SerializeObject(req);
            log.LogInformation($"json: {json}");
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = $"https://{titleId}.playfabapi.com/Match/GetMatch";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-EntityToken", eToken);
                var response = await client.PostAsync(url, data);

                string result = response.Content.ReadAsStringAsync().Result;
                //log.LogInformation(result);
                var rst = JsonConvert.DeserializeObject<dynamic>(result);
                return rst?.data;
            }
        }

        static async void SendToMatchServerRequest(string ip, string port, string matchId, string queueName, ILogger log)
        {
            var req = new {
                MatchId = matchId,
                QueueName = queueName
                };
            var json = JsonConvert.SerializeObject(req);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var matchFoundUrl = $"http://{ip}:{port}/matchfound";

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(matchFoundUrl, data);

                string result = response.Content.ReadAsStringAsync().Result;
                log.LogInformation($"Response from {matchFoundUrl}:\n{result}");
            }
           
        }
    }
}