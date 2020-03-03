// Copyright (C) Microsoft Corporation. All rights reserved.

namespace PlayFab.IntegrationTests
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using PlayFab.DataModels;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class PlayerDetails
    {
        public DateTime LastMissionFailure { get; set; }
        public DateTime LastMissionSuccess { get; set; }
        public Guid Guid { get; set; }
        public double[] MapPosition { get; set; }
        public bool IsPaidUpgrade { get; set; }
    }

    public class GameSettings
    {
        public string Screen { get; set; }
        public string[] Favorites { get; set; }
    }

    public class TestValue
    {
        public PlayerDetails PlayerDetails { get; set; }
        public GameSettings GameSettings { get; set; }
    }

    public class TestObject
    {
        public string ObjectName { get; set; }
        public TestValue ObjectValue { get; set; }
        public string TitleSecretKey { get; set; }
    }

    public static class UpdateSecureObject
    {
        private static Random _random = new Random();

        [FunctionName("UpdateSecureObject")]
        public static async Task<SetObjectsResponse> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] FunctionExecutionContext<TestObject> req,
            HttpRequest httpRequest,
            ILogger log)
        {
            string body = await httpRequest.ReadAsStringAsync();
            log.LogInformation($"HTTP POST Body: {body}");

            log.LogInformation($"callingEntityKey: {JsonConvert.SerializeObject(req.CallerEntityProfile.Entity)}");
            log.LogInformation($"currentEntity: {JsonConvert.SerializeObject(req.CallerEntityProfile)}");

            string name = req.FunctionArgument.ObjectName;
            TestValue val = req.FunctionArgument.ObjectValue;
            val.PlayerDetails.MapPosition = new[] { _random.NextDouble(), _random.NextDouble(), _random.NextDouble() };

            var titleSettings = new PlayFabApiSettings
            {
                TitleId = req.TitleAuthenticationContext.Id,
                VerticalName = Settings.Cloud
            };

            var titleAuthContext = new PlayFabAuthenticationContext();
            titleAuthContext.EntityToken = req.TitleAuthenticationContext.EntityToken;

            var setObjectsRequest = new SetObjectsRequest
            {
                Entity = new DataModels.EntityKey
                {
                    Id = req.CallerEntityProfile.Entity.Id,
                    Type = req.CallerEntityProfile.Entity.Type
                },
                Objects = new System.Collections.Generic.List<SetObject>
                {
                    new SetObject
                    {
                        DataObject = val,
                        ObjectName = name
                    }
                }
            };

            var dataAPI = new PlayFabDataInstanceAPI(titleSettings, titleAuthContext);
            Stopwatch sw = Stopwatch.StartNew();
            PlayFabResult<SetObjectsResponse> setObjectsResponse = await dataAPI.SetObjectsAsync(setObjectsRequest);
            sw.Stop();

            if (setObjectsResponse.Error != null)
            {
                throw new InvalidOperationException($"SetObjectsAsync failed: {setObjectsResponse.Error.GenerateErrorReport()}");
            }
            else
            {
                log.LogInformation($"SetObjectsAsync succeeded in {sw.ElapsedMilliseconds}ms");
                log.LogInformation($"SetObjectsAsync returned. ProfileVersion: {setObjectsResponse.Result.ProfileVersion}. NumResults: {setObjectsResponse.Result.SetResults.Count}");
                return setObjectsResponse.Result;
            }
        }
    }
}