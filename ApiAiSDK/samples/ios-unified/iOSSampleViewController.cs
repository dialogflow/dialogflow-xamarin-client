//
//  API.AI Xamarin Samples 
//  =================================================
//
//  Copyright (C) 2015 by Speaktoit, Inc. (https://www.speaktoit.com)
//  https://www.api.ai
//
//  ***********************************************************************************************************************
//
//  Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with
//  the License. You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on
//  an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the
//  specific language governing permissions and limitations under the License.
//
//  ***********************************************************************************************************************
//

using System;
using System.Drawing;

using Foundation;
using UIKit;
using ApiAi;
using ApiAiSDK;
using ApiAi.Common;
using ApiAi.iOS;
using Newtonsoft.Json;
using ApiAi.Common.Logging;

namespace iOSSample
{
	public partial class iOSSampleViewController : UIViewController
	{
        private AIService aiService;

        LanguageConfig[] languages;


		public iOSSampleViewController (IntPtr handle) : base (handle)
		{
            languages = new []
                {
                    new LanguageConfig("en", "327bf2eb54904e508362f6fb528ce00a"),
                    new LanguageConfig("ru", "adcb900f02594f4186420c082e44173e"),
                    new LanguageConfig("de", "96807aac0e98426eaf684f4081b7e431"),
                    new LanguageConfig("pt", "4c4a2277516041f6a1c909163ebfed39"),
                    new LanguageConfig("pt-BR", "6076377eea9e4291854204795b55eee9"),
                    new LanguageConfig("es", "430d461ea8e64c09a4459560353a5b1d"),
                    new LanguageConfig("fr", "d6434b3bf49d4a93a25679782619f39d"),
                    new LanguageConfig("it", "4319f7aa1765468194d9761432e4db36"),
                    new LanguageConfig("ja", "6cab6813dc8c416f92c3c2e2b4a7bc27"),
                    new LanguageConfig("ko", "b0219c024ee848eaa7cfb17dceb9934a"),
                    new LanguageConfig("zh-CN", "2b575c06deb246d2abe4bf769eb3200b"),
                    new LanguageConfig("zh-HK", "00ef32d3e35f43178405c046a16f3843"),
                    new LanguageConfig("zh-TW", "dd7ebc8a02974155aeffec26b21b55cf"),
                };

            InitializeService(languages[0]);
		}

        partial void selectLang_TouchUpInside (UIButton sender)
        {
            var actionSheet = new UIActionSheet("Select Language");

            foreach (var lang in languages) {
                actionSheet.AddButton(lang.LanguageCode);
            }
            actionSheet.Clicked += languageSelected;
            actionSheet.ShowInView(this.View);
        }

        private void languageSelected(object a, UIButtonEventArgs b)
        {
            var selectedConfig = languages[b.ButtonIndex];
            InitializeService(selectedConfig);
        }

        private void InitializeService(LanguageConfig conf)
        {
            var lang = SupportedLanguage.FromLanguageTag(conf.LanguageCode);
            var config = new AIConfiguration("62f2522a-7404-4c28-b9ac-097ca5d8b32d",
                conf.AccessToken, lang);

			//TODO: Option for verbose logging. Remove this line in production.
            config.DebugLog = true;

            if (aiService != null)
            {
                aiService.Cancel();
            }

			aiService = AIService.CreateService(config);

            aiService.OnResult += AiService_OnResult;
            aiService.OnError += AiService_OnError;
            aiService.ListeningStarted += AiService_ListeningStarted;
            aiService.ListeningFinished += AiService_ListeningFinished;
			aiService.AudioLevelChanged += AiService_AudioLevelChanged;
        }

        partial void listenButton_TouchUpInside (UIButton sender)
        {
			InvokeInBackground(() => aiService.StartListening());
        }

        partial void stopButton_TouchUpInside (UIButton sender)
        {
            InvokeInBackground(aiService.StopListening);
        }

        partial void cancelButton_TouchUpInside (UIButton sender)
        {
            InvokeInBackground(aiService.Cancel);
        }

        void AiService_OnResult(ApiAiSDK.Model.AIResponse response)
        {
            InvokeOnMainThread(() =>
                {
                    if (!response.IsError)
                    {

						var jsonSettings = new JsonSerializerSettings
                        { 
							NullValueHandling = NullValueHandling.Ignore,
                        };

						var responseString = JsonConvert.SerializeObject(response, Formatting.Indented, jsonSettings);
                        resultTextView.Text = responseString;
                    }
                    else
                    {
                        resultTextView.Text = response.Status.ErrorDetails;
                    }
                }
            );
        }

        void AiService_OnError(AIServiceException e)
        {
            Log.Debug("iOSSampleViewController", e.ToString());

            InvokeOnMainThread(() =>
                {
                    resultTextView.Text = e.ToString();
                }
            );
        }

        void AiService_ListeningStarted()
        {
			Log.Debug("iOSSampleViewController", "AiService_ListeningStarted");

            InvokeOnMainThread(()=> { soundLevelView.Hidden = false; });
        }

        void AiService_ListeningFinished()
        {
			Log.Debug("iOSSampleViewController", "AiService_ListeningFinished");
            InvokeOnMainThread(()=> { soundLevelView.Hidden = true; });
        }

        void AiService_AudioLevelChanged(float level)
        {
            //Log.Debug("iOSSampleViewController", "AudioLevel " + level);
            InvokeOnMainThread(() =>
                soundLevelView.SetProgress(level, true));
        }

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
            aiService.Resume();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
            aiService.Pause();
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion
	}

    class LanguageConfig
    {
        public string LanguageCode { get; set; }

        public string AccessToken { get; set; }

        public LanguageConfig(string languageCode, string accessToken)
        {
            this.LanguageCode = languageCode;
            this.AccessToken = accessToken;
        }

        public override string ToString()
        {
            return LanguageCode;
        }
    }
}

