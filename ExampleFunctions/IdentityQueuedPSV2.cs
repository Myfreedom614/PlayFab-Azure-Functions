// Copyright (C) Microsoft Corporation. All rights reserved.

namespace PlayFab.IntegrationTests
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public static class IdentityQueuedPSV2
    {
        private static int delayMilliseconds = 1100; // Default timeout in MainServer for PlayStream is 1 second

        [FunctionName("IdentityQueuedPSV2")]
        public static async Task Run(
            [QueueTrigger("identitypsv2", Connection = "QueueStorage")] string msg,
            ILogger log)
        {
            Stopwatch sw = Stopwatch.StartNew();
            log.LogInformation($"{nameof(IdentityQueuedPSV2)} C# queue trigger function processed a request; {msg}");

            EntityPlayStreamFunctionExecutionContext ctx = JsonConvert.DeserializeObject<EntityPlayStreamFunctionExecutionContext>(msg);

            // Simulate work
            await Task.Delay(IdentityQueuedPSV2.delayMilliseconds);

            // Post results
            await PostFunctionResultToPlayFab(ctx, ctx.FunctionArgument, sw.ElapsedMilliseconds, log);
        }

        private static async Task PostFunctionResultToPlayFab(EntityPlayStreamFunctionExecutionContext ctx, object result, long executionTime, ILogger log)
        {
            if(ctx.GeneratePlayStreamEvent.HasValue && ctx.GeneratePlayStreamEvent.Value)
            {
                await HelperFunctions.PostResults(ctx, nameof(IdentityQueuedPSV2), result, (int)executionTime, log);
            }
        }
    }
}