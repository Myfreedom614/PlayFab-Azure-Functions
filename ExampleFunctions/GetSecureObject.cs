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

    public static class GetSecureObject
    {
        [FunctionName("GetSecureObject")]
        public static async Task<GetObjectsResponse> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] FunctionExecutionContext req,
            HttpRequest httpRequest,
            ILogger log)
        {
            string body = await httpRequest.ReadAsStringAsync();
            log.LogInformation($"HTTP POST Body: {body}");

            log.LogInformation($"callingEntityKey: {JsonConvert.SerializeObject(req.CallerEntityProfile.Entity)}");
            log.LogInformation($"currentEntity: {JsonConvert.SerializeObject(req.CallerEntityProfile)}");

            var titleSettings = new PlayFabApiSettings
            {
                TitleId = req.TitleAuthenticationContext.Id,
                //VerticalName = Settings.Cloud
                DeveloperSecretKey = Settings.TitleSecret
            };

            var titleAuthContext = new PlayFabAuthenticationContext();
            titleAuthContext.EntityToken = req.TitleAuthenticationContext.EntityToken;

            var api = new PlayFabDataInstanceAPI(titleSettings, titleAuthContext);

            var getObjectsRequest = new GetObjectsRequest
            {
                Entity = new DataModels.EntityKey
                {
                    Id = req.CallerEntityProfile.Entity.Id,
                    Type = req.CallerEntityProfile.Entity.Type
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
