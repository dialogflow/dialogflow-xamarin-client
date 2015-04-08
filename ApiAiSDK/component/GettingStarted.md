# Getting Started with API.AI Xamarin SDK

## Quick Integration Instruction

First add access rights to the **Internet** and **Audio recording** for your app. 

In Android it can be done using AndroidManifest.xml
```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
```

In iOS no extra actions required.

1. Connect API.AI SDK Component to your solution.
2. Get two access keys from API.AI developer console.
3. In your code create instance of the `AIConfiguration` class. You should specify **subscription key**, **client access token** and one of the `SupportedLanguage` enumeration cases.
    
    ```csharp
    var config = new AIConfiguration("subscriptionKey", "accessToken", 
                                                SupportedLanguage.English);
    ```

4.  Then create AIService instance using `AIService.CreateService` method.
    * On iOS platform you need only AIConfiguration instance
        
        ```csharp
        aiService = AIService.CreateService(config);
        ```

    * On Android platform you need also context and optionally can specify recognition engine option

        ```csharp
        aiService = AIService.CreateService(context, config);
        ```

5. Now you need to specify event handlers for API.AI results processing
    ```csharp
    aiService.OnResult += AiService_OnResult;
    aiService.OnError += AiService_OnError;
    ```
6. Sample `OnResult` handler. Make sure you interact with the UI in the UI thread
    
    ```csharp
    void AiService_OnResult(AIResponse response)
    {
        RunOnUiThread(() =>
            {
                if (!response.IsError)
                {
                    if (response.Result != null)
                    {
                        resultTextView.Text = response.Result.Action;    
                    }
                }
                else
                {
                    resultTextView.Text = response.Status.ErrorDetails;
                }
            }
        );
    }
    ```

7. Sample `OnError` handler
    ```csharp
    void AiService_OnError(AIServiceException exception)
    {
        Log.Debug(TAG, "AIService Error: ", exception.ToString());
    }
    ```

8. Also you can add additional listeners for another recognition events:

    ```csharp
    aiService.ListeningStarted += AiService_ListeningStarted;
    aiService.ListeningFinished += AiService_ListeningFinished;
    aiService.AudioLevelChanged += AiService_AudioLevelChanged;
    ```
    
9. Now for start listening call `StartListening` method

    ```csharp
    aiService.StartListening();
    ```


