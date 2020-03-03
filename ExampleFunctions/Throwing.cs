// Copyright (C) Microsoft Corporation. All rights reserved.

namespace Playfab.IntegrationTests
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    public static class Throwing
    {
        [FunctionName("Throwing")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"{nameof(Throwing)} C# HTTP trigger function processed a request.");

            // Simulate work
            await Task.Delay(50);

            // Simulate error
            throw new NullReferenceException();
        }
    }
}
