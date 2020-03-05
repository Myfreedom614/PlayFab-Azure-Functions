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
    public static class Settings
    {
        private const string _titleId = "PlayFab.TitleId";
        private const string _titleSecret = "PlayFab.TitleSecret";
        private const string _cloudSetting = "PlayFab.Cloud";

        public static string Cloud => Environment.GetEnvironmentVariable(_cloudSetting, EnvironmentVariableTarget.Process);
        public static string TitleId => Environment.GetEnvironmentVariable(_titleId, EnvironmentVariableTarget.Process);
        public static string TitleSecret => Environment.GetEnvironmentVariable(_titleSecret, EnvironmentVariableTarget.Process);
    }

    public static class HelloWorld
    {
        [FunctionName("HelloWorld")]
        public static async Task<dynamic> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]  HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
            dynamic args = context.FunctionArgument;
            var message = $"Hello {context.CallerEntityProfile.Lineage.MasterPlayerAccountId} (MasterPlayerAccountId)!";
            log.LogInformation(message);
            dynamic inputValue = null;
            if(args != null && args["inputValue"] != null)
            {
            inputValue = args["inputValue"];
            }
            log.LogInformation($"HelloWorld args: {new { input=inputValue}}");
            return new { messageValue = message };
        }
    }
}