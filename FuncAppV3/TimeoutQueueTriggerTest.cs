using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
 
namespace PlayFab.Samples
{
    public static class TimeoutQueueTriggerTest
    {
        [FunctionName("TimeoutQueueTriggerTest")]
        public static async Task Run(
            [QueueTrigger("timeoutqueue", Connection = "AzureWebJobsStorage")] string msg,
            ILogger log)
        {
            Stopwatch sw = Stopwatch.StartNew();
            log.LogInformation($"{nameof(TimeoutQueueTriggerTest)} C# queue trigger function processed a request; {msg}");

            FunctionExecutionContext ctx = JsonConvert.DeserializeObject<FunctionExecutionContext>(msg);
            
            dynamic args = ctx.FunctionArgument;
            
            int delayMilliseconds = 0;
            if(args != null && args["delayMilliseconds"] != null)
            {
                delayMilliseconds = args["delayMilliseconds"];
            }
            // Simulate work
            await Task.Delay(delayMilliseconds);

            // Post results
            await PostFunctionResultToPlayFab(ctx, ctx.FunctionArgument, sw.ElapsedMilliseconds, log);
        }

        private static async Task PostFunctionResultToPlayFab(FunctionExecutionContext ctx, object result, long executionTime, ILogger log)
        {
            if(ctx.GeneratePlayStreamEvent.HasValue && ctx.GeneratePlayStreamEvent.Value)
            {
                await HelperFunctions.PostResults(ctx, nameof(TimeoutQueueTriggerTest), result, (int)executionTime, log);
            }
        }
    }
}