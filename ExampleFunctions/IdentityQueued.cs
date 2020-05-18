// Copyright (C) Microsoft Corporation. All rights reserved.

namespace PlayFab.IntegrationTests
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using PlayFab.Samples;

    public static class IdentityQueued
    {
        private static int delayMilliseconds = 10100; // Default timeout in MainServer for ExecuteFunction is 10 seconds

        [FunctionName("IdentityQueued")]
        public static async Task Run(
            [QueueTrigger("identity", Connection = "QueueStorage")] string msg,
            ILogger log)
        {
            Stopwatch sw = Stopwatch.StartNew();
            log.LogInformation($"{nameof(IdentityQueued)} C# queue trigger function processed a request; {msg}");

            FunctionExecutionContext ctx = JsonConvert.DeserializeObject<FunctionExecutionContext>(msg);

            // Simulate work
            await Task.Delay(IdentityQueued.delayMilliseconds);

            // Post results
            await PostFunctionResultToPlayFab(ctx, ctx.FunctionArgument, sw.ElapsedMilliseconds, log);
        }

        private static async Task PostFunctionResultToPlayFab(FunctionExecutionContext ctx, object result, long executionTime, ILogger log)
        {
            if(ctx.GeneratePlayStreamEvent.HasValue && ctx.GeneratePlayStreamEvent.Value)
            {
                await HelperFunctions.PostResults(ctx, nameof(IdentityQueued), result, (int)executionTime, log);
            }
        }
    }
}