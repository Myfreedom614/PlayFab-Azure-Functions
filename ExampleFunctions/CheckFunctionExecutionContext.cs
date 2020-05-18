// Copyright (C) Microsoft Corporation. All rights reserved.

namespace PlayFab.IntegrationTests
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;
    using PlayFab.Samples;

    public static class CheckFunctionExecutionContext
    {
        [FunctionName("CheckFunctionExecutionContext")]
        public static async Task<bool> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var body = await req.ReadAsStringAsync();
            log.LogInformation($"HTTP POST Body: {body}");

            var context = PlayFab.Json.PlayFabSimpleJson.DeserializeObject<FunctionExecutionContext<string>>(body);

            return
                context.TitleAuthenticationContext != null &&
                !string.IsNullOrEmpty(context.TitleAuthenticationContext.EntityToken) &&
                !string.IsNullOrEmpty(context.TitleAuthenticationContext.Id);
        }
    }
}
