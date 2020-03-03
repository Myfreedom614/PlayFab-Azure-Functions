// Copyright (C) Microsoft Corporation. All rights reserved.

namespace PlayFab.IntegrationTests
{
    using System;
    
    public static class Settings
    {
        private const string _titleId = "PlayFab.TitleId";
        private const string _titleSecret = "PlayFab.TitleSecret";
        private const string _cloudSetting = "PlayFab.Cloud";

        public static string Cloud => Environment.GetEnvironmentVariable(_cloudSetting, EnvironmentVariableTarget.Process);
        public static string TitleId => Environment.GetEnvironmentVariable(_titleId, EnvironmentVariableTarget.Process);
        public static string TitleSecret => Environment.GetEnvironmentVariable(_titleSecret, EnvironmentVariableTarget.Process);
    }    
}