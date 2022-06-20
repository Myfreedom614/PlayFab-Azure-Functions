# FuncAppV6

## .csproj
```xml
<TargetFramework>net6.0</TargetFramework>
<AzureFunctionsVersion>v4</AzureFunctionsVersion>
```

## HelloWorld Sample Request

Post Request - ExecuteFunction
Body:

```Json
{
  "FunctionName": "HelloWorldV6",
  "FunctionParameter": {
    "inputValue": "Franklin"
  },
  "GeneratePlayStreamEvent": true,
  "Entity": {
    "Id": "{{PlayerEntityId}}",
    "Type": "title_player_account",
    "TypeString": "title_player_account"
  }
}
```