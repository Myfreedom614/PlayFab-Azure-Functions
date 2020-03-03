// Copyright (C) Microsoft Corporation. All rights reserved.

namespace PlayFab.IntegrationTests
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public static class IdentityQueuedTask
    {
        private static int delayMilliseconds = 4600; // Default timeout in MainServer for Task Execution is 4.5 seconds

        [FunctionName("IdentityQueuedTask")]
        public static async Task Run(
            [QueueTrigger("identitytask", Connection = "QueueStorage")] string msg,
            ILogger log)
        {
            Stopwatch sw = Stopwatch.StartNew();
            log.LogInformation($"{nameof(IdentityQueuedTask)} C# queue trigger function processed a request; {msg}");

            ScheduledTaskFunctionExecutionContext ctx = JsonConvert.DeserializeObject<ScheduledTaskFunctionExecutionContext>(msg);    

            // Simulate work
            await Task.Delay(IdentityQueuedTask.delayMilliseconds);

            // Post results
            await PostFunctionResultToPlayFab(ctx, ctx.FunctionArgument, sw.ElapsedMilliseconds, log);
        }

        private static async Task PostFunctionResultToPlayFab(ScheduledTaskFunctionExecutionContext ctx, object result, long executionTime, ILogger log)
        {
            if(ctx.GeneratePlayStreamEvent.HasValue && ctx.GeneratePlayStreamEvent.Value)
            {
                await HelperFunctions.PostResults(ctx, nameof(IdentityQueuedTask), result, (int)executionTime, log);
            }
        }
    }
}