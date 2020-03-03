// Copyright (C) Microsoft Corporation. All rights reserved.

namespace PlayFab.IntegrationTests
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using PlayFab.CloudScriptModels;
    using PlayFab.Internal;

    public class HelperFunctions
    {
        private static HttpClient _httpClient = new HttpClient();
        private static bool _sdkInitialized = false;

        public static void InitSDK(ILogger log)
        {
            if (!_sdkInitialized)
            {
                log.LogInformation("Initialized PlayFab SDK");

                // Update the PlayFabSettings with dev secret key and title ID
                if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.DeveloperSecretKey))
                {
                    PlayFabSettings.staticSettings.DeveloperSecretKey = Settings.TitleSecret;
                }

                if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
                {
                    PlayFabSettings.staticSettings.TitleId = Settings.TitleId;
                }

                if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.VerticalName) && !string.IsNullOrEmpty(Settings.Cloud))
                {
                    PlayFabSettings.staticSettings.VerticalName = Settings.Cloud;
                }

                _sdkInitialized = true;
            }
        }

        public static Task PostResults(FunctionExecutionContext ctx, string functionName, object functionResult, int executionTime, ILogger log)
        {
            var request = new PostFunctionResultForFunctionExecutionRequest
            {
                Entity = new EntityKey
                {
                    Id = ctx.CallerEntityProfile.Entity.Id,
                    Type = ctx.CallerEntityProfile.Entity.Type,
                },
                FunctionResult = new ExecuteFunctionResult
                {
                    ExecutionTimeMilliseconds = executionTime,
                    Error = null,
                    FunctionName = functionName,
                    FunctionResult = functionResult
                }
            };

            // TODO: Replace this code with an SDK call once an SDK is published that supports PostFunctionResultForFunctionExecution
            return CallPostResultApi("PostFunctionResultForFunctionExecution", ctx.TitleAuthenticationContext, request, log);
        }

        public static Task PostResults(PlayerPlayStreamFunctionExecutionContext ctx, string functionName, object functionResult, int executionTime, ILogger log)
        {
            var request = new PostFunctionResultForPlayerTriggeredActionRequest
            {
                Entity = new EntityKey
                {
                    Id = ctx.PlayerProfile.PlayerId,
                    Type = "master_player_account"
                },
                PlayerProfile = ctx.PlayerProfile,
                PlayStreamEventEnvelope = ctx.PlayStreamEventEnvelope,
                FunctionResult = new ExecuteFunctionResult
                {
                    ExecutionTimeMilliseconds = executionTime,
                    Error = null,
                    FunctionName = functionName,
                    FunctionResult = functionResult
                }
            };

            // TODO: Replace this code with an SDK call once an SDK is published that supports PostFunctionResultForPlayerTriggeredAction
            return CallPostResultApi("PostFunctionResultForPlayerTriggeredAction", ctx.TitleAuthenticationContext, request, log);
        }

        public static Task PostResults(EntityPlayStreamFunctionExecutionContext ctx, string functionName, object functionResult, int executionTime, ILogger log)
        {
            //     public CloudScriptModels.EntityKey Entity { get; set; }
            var request = new PostFunctionResultForEntityTriggeredActionRequest
            {
                Entity = new EntityKey
                {
                    Id = ctx.CallerEntityProfile.Entity.Id,
                    Type = ctx.CallerEntityProfile.Entity.Type
                },
                CallerEntityProfile = ctx.CallerEntityProfile,
                PlayStreamEvent = ctx.PlayStreamEvent,
                FunctionResult = new ExecuteFunctionResult
                {
                    ExecutionTimeMilliseconds = executionTime,
                    Error = null,
                    FunctionName = functionName,
                    FunctionResult = functionResult
                }
            };

            // TODO: Replace this code with an SDK call once an SDK is published that supports PostFunctionResultForEntityTriggeredAction
            return CallPostResultApi("PostFunctionResultForEntityTriggeredAction", ctx.TitleAuthenticationContext, request, log);
        }

        public static Task PostResults(ScheduledTaskFunctionExecutionContext ctx, string functionName, object functionResult, int executionTime, ILogger log)
        {
            var request = new PostFunctionResultForScheduledTaskRequest
            {
                Entity = new EntityKey
                {
                    Id = ctx.TitleAuthenticationContext.Id,
                    Type = "title"
                },
                ScheduledTaskId = ctx.ScheduledTaskNameId,
                FunctionResult = new ExecuteFunctionResult
                {
                    ExecutionTimeMilliseconds = executionTime,
                    Error = null,
                    FunctionName = functionName,
                    FunctionResult = functionResult
                }
            };

            // TODO: Replace this code with an SDK call once an SDK is published that supports PostFunctionResultForScheduledTask
            return CallPostResultApi("PostFunctionResultForScheduledTask", ctx.TitleAuthenticationContext, request, log);
        }

        private static async Task CallPostResultApi<T>(string api, TitleAuthenticationContext ctx, T request, ILogger log)
        {
            string requestUri = HelperFunctions.GetRequestUri(ctx.Id, $"/CloudScript/{api}");
            var postBody = new StringContent(JsonConvert.SerializeObject(request));
            postBody.Headers.Add("X-EntityToken", ctx.EntityToken);
            postBody.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (HttpResponseMessage response = await _httpClient.PostAsync(requestUri, postBody))
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                log.LogInformation($"{api} response body: {responseBody}");

                if (response.IsSuccessStatusCode)
                {
                    log.LogInformation($"{api} succeeded");
                }
                else
                {
                    var resultError = JsonConvert.DeserializeObject<PlayFabJsonError>(responseBody);
                    log.LogError($"Error calling {api}: {resultError.errorMessage}");
                }
            }
        }

        private static string GetRequestUri(string titleId, string endpoint)
        {
            string cloud = Settings.Cloud;
            if (string.IsNullOrWhiteSpace(cloud))
            {
                return $"https://{titleId}.playfabapi.com{endpoint}";
            }
            else
            {
                return $"https://{titleId}.{cloud}.playfabapi.com{endpoint}";
            }
        }
    }
}