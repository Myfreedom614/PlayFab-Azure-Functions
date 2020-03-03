using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlayFab.Samples;

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
    
    public class TestObject
    {
        public string Name { get; set; }
    }

    public static class HelloWorld
    {
        [FunctionName("HelloWorld")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] FunctionExecutionContext<TestObject> req,
            HttpRequest httpRequest,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.FunctionArgument.Name;

            string requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. PlayFab.TitleId:{Settings.TitleId}, PlayFab.TitleSecret:{Settings.TitleSecret}, PlayFab.Cloud:{Settings.Cloud}";

            return new OkObjectResult(responseMessage);
        }
    }
}
