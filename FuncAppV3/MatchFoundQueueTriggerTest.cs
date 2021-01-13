using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System;
using System.Linq;
 
namespace PlayFab.Samples
{
    public static class MatchFoundQueueTriggerTest
    {
        [FunctionName("MatchFoundQueueTriggerTest")]
        public static async Task Run(
            [QueueTrigger("matchqueue", Connection = "AzureWebJobsStorage")] string req,
            ILogger log)
        {
            Stopwatch sw = Stopwatch.StartNew();
            object result = null;
            log.LogInformation($"{nameof(MatchFoundQueueTriggerTest)} C# queue trigger function processed a request; {req}");

            var ctx = JsonConvert.DeserializeObject<dynamic>(req);
            
            var msg = $"context is {JsonConvert.SerializeObject(ctx)}\n";
            log.LogInformation($"{msg}");
            
            var authCtx = ctx?.TitleAuthenticationContext;
            var payload = ctx?.PlayStreamEvent?.Payload;

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

                result =  new {Status = "OK", Message = "Success"};
            }
            else {
                result =  new {Status = "BadRequest", Message = "Authentication Context or PlayStream Payload is missing"};
            }
            
            // Post results
            FunctionExecutionContext ctx2 = JsonConvert.DeserializeObject<FunctionExecutionContext>(req);
            await PostFunctionResultToPlayFab(ctx2, result, sw.ElapsedMilliseconds, log);
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

        private static async Task PostFunctionResultToPlayFab(FunctionExecutionContext ctx, object result, long executionTime, ILogger log)
        {
            if(ctx.GeneratePlayStreamEvent.HasValue && ctx.GeneratePlayStreamEvent.Value)
            {
                await HelperFunctions.PostResults(ctx, nameof(MatchFoundQueueTriggerTest), result, (int)executionTime, log);
            }
        }
    }
}