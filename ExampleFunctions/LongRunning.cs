// Copyright (C) Microsoft Corporation. All rights reserved.

namespace PlayFab.IntegrationTests
{
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public static class LongRunning
    {
        private static int delayMilliseconds = 10100; // Default timeout in MainServer is 10 seconds

        [FunctionName("LongRunning")]
        public static async Task<object> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"{nameof(LongRunning)} C# HTTP trigger function processed a request.");

            // Simulate work
            await Task.Delay(LongRunning.delayMilliseconds);

            return new { };
        }
    }
}