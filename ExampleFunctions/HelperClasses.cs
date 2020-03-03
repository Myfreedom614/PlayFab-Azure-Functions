// Copyright (C) Microsoft Corporation. All rights reserved.

namespace PlayFab.IntegrationTests
{
    using System;
    using System.Collections.Generic;

    // Shared models
    public class TitleAuthenticationContext
    {
        public string Id { get; set; }
        public string EntityToken { get; set; }
    }

    public class FunctionExecutionError
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }

    public class ExecuteFunctionResult<T>
    {
        public int ExecutionTimeMilliseconds { get; set; }
        public string FunctionName { get; set; }
        public T FunctionResult { get; set; }
        public bool? FunctionResultTooLarge { get; set; }
        public FunctionExecutionError Error { get; set; }
    }

    public class ExecuteFunctionResult : ExecuteFunctionResult<object>
    {
    }

    // Models for execution via ExecuteFunction API
    public class FunctionExecutionContext<T>
    {
        public PlayFab.ProfilesModels.EntityProfileBody CallerEntityProfile { get; set; }
        public TitleAuthenticationContext TitleAuthenticationContext { get; set; }
        public bool? GeneratePlayStreamEvent { get; set; }
        public T FunctionArgument { get; set; }
    }

    public class FunctionExecutionContext : FunctionExecutionContext<object>
    {
    }

    public class PostFunctionResultForFunctionExecutionRequest<T>
    {
        public CloudScriptModels.EntityKey Entity { get; set; }
        public ExecuteFunctionResult<T> FunctionResult { get; set; }
    }

    public class PostFunctionResultForFunctionExecutionRequest : PostFunctionResultForFunctionExecutionRequest<object>
    {
    }

    // Models for execution via Player PlayStream event, entering or leaving a 
    // player segment or as part of a player segment based scheduled task.
    public class PlayerPlayStreamEventEnvelope
    {
        public string EntityId { get; set; }
        public string EntityType { get; set; }
        public string EventName { get; set; }
        public string EventNamespace { get; set; }
        public string EventData { get; set; }
        public string EventSettings { get; set; }
    }

    public class PlayerPlayStreamFunctionExecutionContext<T>
    {
        public PlayFab.ServerModels.PlayerProfile PlayerProfile { get; set; }
        public bool PlayerProfileTruncated { get; set; }
        public PlayerPlayStreamEventEnvelope PlayStreamEventEnvelope { get; set; }
        public TitleAuthenticationContext TitleAuthenticationContext { get; set; }
        public bool? GeneratePlayStreamEvent { get; set; }
        public T FunctionArgument { get; set; }
    }

    public class PlayerPlayStreamFunctionExecutionContext : PlayerPlayStreamFunctionExecutionContext<object>
    {
    }

    public class PostFunctionResultForPlayerTriggeredActionRequest<T>
    {
        public CloudScriptModels.EntityKey Entity { get; set; }
        public PlayerPlayStreamEventEnvelope PlayStreamEventEnvelope { get; set; }
        public PlayFab.ServerModels.PlayerProfile PlayerProfile { get; set; }
        public ExecuteFunctionResult<T> FunctionResult { get; set; }
    }

    public class PostFunctionResultForPlayerTriggeredActionRequest : PostFunctionResultForPlayerTriggeredActionRequest<object>
    {
    }

    // Models for execution via Scheduled task
    public class PlayStreamEventHistory
    {
        public string ParentTriggerId { get; set; }
        public string ParentEventId { get; set; }
        public bool TriggeredEvents { get; set; }
    }

    public class ScheduledTaskFunctionExecutionContext<T>
    {
        public PlayFab.AdminModels.NameIdentifier ScheduledTaskNameId { get; set; }
        public Stack<PlayStreamEventHistory> EventHistory { get; set; }
        public TitleAuthenticationContext TitleAuthenticationContext { get; set; }
        public bool? GeneratePlayStreamEvent { get; set; }
        public T FunctionArgument { get; set; }
    }

    public class ScheduledTaskFunctionExecutionContext : ScheduledTaskFunctionExecutionContext<object>
    {
    }

    public class PostFunctionResultForScheduledTaskRequest<T>
    {
        public CloudScriptModels.EntityKey Entity { get; set; }
        public PlayFab.AdminModels.NameIdentifier ScheduledTaskId { get; set; }
        public ExecuteFunctionResult<T> FunctionResult { get; set; }
    }

    public class PostFunctionResultForScheduledTaskRequest : PostFunctionResultForScheduledTaskRequest<object>
    {
    }

    // Models for execution via entity PlayStream event, entering or leaving an 
    // entity segment or as part of an entity segment based scheduled task.
    public class EventFullName
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
    }

    public class OriginInfo
    {
        public string Id { get; set; }
        public DateTime? Timestamp { get; set; }
    }

    public class EntityLineage
    {
        public string NamespaceId { get; set; }
        public string TitleId { get; set; }
        public string MasterPlayerAccountId { get; set; }
        public string TitlePlayerAccountId { get; set; }
        public string CharacterId { get; set; }
        public string GroupId { get; set; }
        public string CloudRootId { get; set; }
    }

    public class EntityPlayStreamEvent<T>
    {
        public string SchemaVersion { get; set; }
        public EventFullName FullName { get; set; }
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public CloudScriptModels.EntityKey Entity { get; set; }
        public CloudScriptModels.EntityKey Originator { get; set; }
        public OriginInfo OriginInfo { get; set; }
        public T Payload { get; set; }
        public EntityLineage EntityLineage { get; set; }
    }

    public class EntityPlayStreamEvent : EntityPlayStreamEvent<object>
    {
    }

    public class EntityPlayStreamFunctionExecutionContext<TPayload, TArg>
    {
        public PlayFab.ProfilesModels.EntityProfileBody CallerEntityProfile { get; set; }
        public EntityPlayStreamEvent<TPayload> PlayStreamEvent { get; set; }
        public TitleAuthenticationContext TitleAuthenticationContext { get; set; }
        public bool? GeneratePlayStreamEvent { get; set; }
        public TArg FunctionArgument { get; set; }
    }

    public class EntityPlayStreamFunctionExecutionContext : EntityPlayStreamFunctionExecutionContext<object, object>
    {
    }

    public class PostFunctionResultForEntityTriggeredActionRequest<T>
    {
        public CloudScriptModels.EntityKey Entity { get; set; }
        public PlayFab.ProfilesModels.EntityProfileBody CallerEntityProfile { get; set; }
        public EntityPlayStreamEvent<T> PlayStreamEvent { get; set; }
        public ExecuteFunctionResult FunctionResult { get; set; }
    }

    public class PostFunctionResultForEntityTriggeredActionRequest : PostFunctionResultForEntityTriggeredActionRequest<object>
    {
    }
}