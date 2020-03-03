# HelloWorld

## Sample Request

Post Request - ExecuteFunction
Body:

```Json
{
  "FunctionName": "HelloWorld",
  "FunctionParameter": {
    "name": "Franklin"
  },
  "GeneratePlayStreamEvent": true,
  "Entity": {
    "Id": "{{PlayerEntityId}}",
    "Type": "title_player_account",
    "TypeString": "title_player_account"
  }
}
```