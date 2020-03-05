# FuncAppV3

## HelloWorld Sample Request

Post Request - ExecuteFunction
Body:

```Json
{
  "FunctionName": "HelloWorldV3",
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