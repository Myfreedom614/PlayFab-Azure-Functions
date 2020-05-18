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
    using PlayFab.Samples;

    public static class GetSecureObjectPlayStream
    {
        [FunctionName("GetSecureObjectPlayStream")]
        public static async Task<GetObjectsResponse> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] PlayerPlayStreamFunctionExecutionContext req,
            HttpRequest httpRequest,
            ILogger log)
        {
            string body = await httpRequest.ReadAsStringAsync();
            log.LogInformation($"HTTP POST Body: {body}");

            if(req.PlayStreamEventEnvelope != null)
            {
                log.LogInformation($"eventEnvelope: {JsonConvert.SerializeObject(req.PlayStreamEventEnvelope)}");
            }
            else
            {
                log.LogInformation("PlayStreamEventEnvelope was null");
            }

            if(req.PlayerProfile != null)
            {
                log.LogInformation($"playerProfile: {JsonConvert.SerializeObject(req.PlayerProfile)}");
            }
            else
            {
                log.LogInformation("PlayerProfile was null");
            }

            var titleSettings = new PlayFabApiSettings
            {
                TitleId = req.TitleAuthenticationContext.Id,
                VerticalName = Settings.Cloud
            };

            var titleAuthContext = new PlayFabAuthenticationContext();
            titleAuthContext.EntityToken = req.TitleAuthenticationContext.EntityToken;

            var api = new PlayFabDataInstanceAPI(titleSettings, titleAuthContext);

            var getObjectsRequest = new GetObjectsRequest
            {
                Entity = new DataModels.EntityKey
                {
                    Id = req.PlayerProfile.PlayerId,
                    Type = "master_player_account"
                }
            };

            Stopwatch sw = Stopwatch.StartNew();
            PlayFabResult<GetObjectsResponse> getObjectsResponse = await api.GetObjectsAsync(getObjectsRequest);
            sw.Stop();

            if (getObjectsResponse.Error != null)
            {
                throw new InvalidOperationException($"GetObjectsAsync failed: {getObjectsResponse.Error.GenerateErrorReport()}");
            }
            else
            {
                log.LogInformation($"GetObjectsAsync succeeded in {sw.ElapsedMilliseconds}ms");
                log.LogInformation($"GetObjectsAsync returned. ProfileVersion: {getObjectsResponse.Result.ProfileVersion}. Entity: {getObjectsResponse.Result.Entity.Id}/{getObjectsResponse.Result.Entity.Type}. NumObjects: {getObjectsResponse.Result.Objects.Count}.");
                return getObjectsResponse.Result;
            }
        }
    }
}
