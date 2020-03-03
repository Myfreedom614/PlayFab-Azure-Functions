// Copyright (C) Microsoft Corporation. All rights reserved.

namespace PlayFab.IntegrationTests
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    public static class IdentityTask
    {
        [FunctionName("IdentityTask")]
        public static async Task<object> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] ScheduledTaskFunctionExecutionContext req,
            HttpRequest httpRequest,
            ILogger log)
        {
            string body = await httpRequest.ReadAsStringAsync();
            log.LogInformation($"HTTP POST Body: {body}");

            return await Task.FromResult(req.FunctionArgument);
        }
    }
}
