using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PlayFab.Samples
{
    public static class CSAFForScheduledTask
    {
        [FunctionName("CSAFForScheduledTask")]
        public static async Task<object> Run(
            // [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]  ScheduledTaskFunctionExecutionContext req,
            // HttpRequest httpRequest,
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]  HttpRequest httpRequest,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            log.LogInformation($"Settings.TitleSecret: {Settings.TitleSecret}");

            string body = await httpRequest.ReadAsStringAsync();
            log.LogInformation($"HTTP POST Body: {body}");

            ScheduledTaskFunctionExecutionContext<object> ctx = JsonConvert.DeserializeObject<ScheduledTaskFunctionExecutionContext<object>>(body);
            log.LogInformation($"ScheduledTaskFunctionExecutionContext: {JsonConvert.SerializeObject(ctx)}");


            return await Task.FromResult(ctx.FunctionArgument);
        }
    }
}