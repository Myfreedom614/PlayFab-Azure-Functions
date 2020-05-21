using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
 
namespace PlayFab.Samples
{
    public static class TimeoutHttpTriggerTest
    {
        [FunctionName("TimeoutHttpTriggerTest")]
        public static async Task<dynamic> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"{nameof(TimeoutHttpTriggerTest)} C# HTTP trigger function processed a request.");
            
            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
            dynamic args = context.FunctionArgument;
            
            int delayMilliseconds = 0;
            if(args != null && args["delayMilliseconds"] != null)
            {
                delayMilliseconds = args["delayMilliseconds"];
            }
            // Simulate work
            await Task.Delay(delayMilliseconds);

            return new { delayMilliseconds = delayMilliseconds };
        }
    }
}