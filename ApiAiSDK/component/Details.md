# API.AI Xamarin SDK

Xamarin SDK for [Api.ai](http://api.ai) natural language processing service makes it easy to integrate speech interfaces into your Xamarin apps.

Please refer to the detailed docs at [api.ai](http://api.ai/docs/).

## <a name="pre-conditions"/> Before you start coding:
* To use the SDK, you'd need to [create an Api.ai account](https://console.api.ai/api-client/#/signup).
* [Create an agent](https://console.api.ai/api-client/#/newAgent) to get two API keys: subscription key and client access token.

## <a name="integration"/> Integration
* [Initialize the SDK](#initialize-sdk)
* [Process results](#process-results)
* [Start voice input](#start-voice-input)
* [Sample app](#sample-app)

### <a name="initialize-sdk">Initialize the SDK

```csharp
var config = new AIConfiguration("subscriptionKey", "accessToken", 
                                                SupportedLanguage.English);
aiService = AIService.CreateService(config);

aiService.OnResult += AiService_OnResult;
aiService.OnError += AiService_OnError;
```

_(Note: In Android you also should pass context to the `CreateService` method)_

### <a name="process-results" />Process results
And this code snippet for processing results

```csharp
void AiService_OnResult(AIResponse response)
{
    //TODO: Implement DoInUiThread function according to your platform
    DoInUiThread(() =>
        {
            if (!response.IsError)
            {
                // TODO: Do some response processing
            }
            else
            {
                // TODO: Do some server error processing
            }
        }
    );
}

void AiService_OnError(AIServiceException exception)
{
    Log.Debug(TAG, "AIService Error: ", exception.ToString());
    // TODO: Do some error processing
}
```

### <a name="start-voice-input" />Start voice input
Start listening with `aiService.StartListening();` method.

### <a name="sample-app" />Sample app
For more details see GettingStarted section in Xamarin Components Store and the [example app](https://github.com/api-ai/api-ai-xamarin-sample).
