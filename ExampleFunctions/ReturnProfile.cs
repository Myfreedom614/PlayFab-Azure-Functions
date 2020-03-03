// Copyright (C) Microsoft Corporation. All rights reserved.

namespace PlayFab.IntegrationTests
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using PlayFab.ProfilesModels;
    using System.Threading.Tasks;

    public static class ReturnProfile
    {
        [FunctionName("ReturnProfile")]
        public static async Task<EntityProfileBody> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] FunctionExecutionContext req,
            HttpRequest httpRequest,
            ILogger log)
        {
            string body = await httpRequest.ReadAsStringAsync();
            log.LogInformation($"HTTP POST Body: {body}");

            log.LogInformation($"callingEntityKey: {JsonConvert.SerializeObject(req.CallerEntityProfile.Entity)}");
            log.LogInformation($"currentEntity: {JsonConvert.SerializeObject(req.CallerEntityProfile)}");

            return await Task.FromResult(req.CallerEntityProfile);
        }
    }
}
