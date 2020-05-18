// Copyright (C) Microsoft Corporation. All rights reserved.

namespace PlayFab.IntegrationTests
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System.Threading.Tasks;
    using PlayFab.Samples;

    public static class IdentityArrayTask
    {
        [FunctionName("IdentityArrayTask")]
        public static async Task<object> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest,
            ILogger log)
        {
            string body = await httpRequest.ReadAsStringAsync();
            log.LogInformation($"HTTP POST Body: {body}");

            ScheduledTaskFunctionExecutionContext<object[]> req = JsonConvert.DeserializeObject<ScheduledTaskFunctionExecutionContext<object[]>>(body);

            return await Task.FromResult(req.FunctionArgument);
        }
    }
}
