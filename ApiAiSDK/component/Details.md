# API.AI Xamarin SDK

Xamarin SDK for [Api.ai](http://api.ai) natural language processing service makes it easy to integrate speech interfaces into your Xamarin apps.

Please refer to the detailed docs at [api.ai](http://api.ai/docs/).

Preconditions:
* To use the SDK, you'd need to [register](https://console.api.ai/api-client/#/signup) with Api.ai 
* [Create an agent](https://console.api.ai/api-client/#/newAgent) to get two API keys: subscription key and client access token.

Here's the initialization code snippet:

```csharp
var config = new AIConfiguration("subscriptionKey", "accessToken", 
                                                SupportedLanguage.English);
aiService = AIService.CreateService(config);

aiService.OnResult += AiService_OnResult;
aiService.OnError += AiService_OnError;
```

_(Note: In Android you also should pass context to the `CreateService` method)_

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

Start listening with `aiService.StartListening();` method.

For more details see GettingStarted section in Xamarin Components Store and sample applications.
