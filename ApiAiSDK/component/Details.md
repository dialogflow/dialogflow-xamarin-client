# API.AI Xamarin SDK

Xamarin SDK for [API.AI](http://api.ai) natural language processing service makes it easy to integrate speech recognition and processing features to your Xamarin apps.

See detailed documentation on the [api.ai](http://api.ai/docs/) website.

To use API.AI service you should get two keys: subscription key and client access token.

Use this code snippet for initialization 

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

## Source Code
* GitHub: https://github.com/api-ai/api-ai-xamarin
