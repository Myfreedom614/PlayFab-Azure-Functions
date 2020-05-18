// Copyright (C) Microsoft Corporation. All rights reserved.

namespace PlayFab.IntegrationTests
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using PlayFab.Samples;

    public static class IdentityQueuedPSV1
    {
        private static int delayMilliseconds = 1100; // Default timeout in MainServer for PlayStream is 1 second

        [FunctionName("IdentityQueuedPSV1")]
        public static async Task Run(
            [QueueTrigger("identitypsv1", Connection = "QueueStorage")] string msg,
            ILogger log)
        {
            Stopwatch sw = Stopwatch.StartNew();
            log.LogInformation($"{nameof(IdentityQueuedPSV1)} C# queue trigger function processed a request; {msg}");

            PlayerPlayStreamFunctionExecutionContext ctx = JsonConvert.DeserializeObject<PlayerPlayStreamFunctionExecutionContext>(msg);
            
            // Simulate work
            await Task.Delay(IdentityQueuedPSV1.delayMilliseconds);

            // Post results
            await PostFunctionResultToPlayFab(ctx, ctx.FunctionArgument, sw.ElapsedMilliseconds, log);
        }

        private static async Task PostFunctionResultToPlayFab(PlayerPlayStreamFunctionExecutionContext ctx, object result, long executionTime, ILogger log)
        {
            if(ctx.GeneratePlayStreamEvent.HasValue && ctx.GeneratePlayStreamEvent.Value)
            {
                await HelperFunctions.PostResults(ctx, nameof(IdentityQueuedPSV1), result, (int)executionTime, log);
            }
        }
    }
}