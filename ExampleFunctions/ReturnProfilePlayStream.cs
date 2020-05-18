// Copyright (C) Microsoft Corporation. All rights reserved.

namespace PlayFab.IntegrationTests
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using PlayFab.CloudScriptModels;
    using System.Threading.Tasks;
    using PlayFab.Samples;

    public static class ReturnProfilePlayStream
    {
        [FunctionName("ReturnProfilePlayStream")]
        public static async Task<PlayerProfileModel> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] PlayerPlayStreamFunctionExecutionContext req,
            HttpRequest httpRequest,
            ILogger log)
        {
            string body = await httpRequest.ReadAsStringAsync();
            log.LogInformation($"HTTP POST Body: {body}");

            if(req.PlayStreamEventEnvelope != null)
            {
                log.LogInformation($"eventEnvelope: {JsonConvert.SerializeObject(req.PlayStreamEventEnvelope)}");
            }
            else
            {
                log.LogInformation("PlayStreamEventEnvelope was null");
            }

            if(req.PlayerProfile != null)
            {
                log.LogInformation($"playerProfile: {JsonConvert.SerializeObject(req.PlayerProfile)}");
            }
            else
            {
                log.LogInformation("PlayerProfile was null");
            }

            return await Task.FromResult(req.PlayerProfile);
        }
    }
}
