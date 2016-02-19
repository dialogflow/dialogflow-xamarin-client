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

using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;

using ApiAiSDK;
using ApiAi.Common;
using ApiAi.Android;
using Newtonsoft.Json;
using Android.Util;
using System.Threading.Tasks;

namespace AndroidSample
{
    [Activity(Label = "API.AI Sample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private readonly string TAG = typeof(MainActivity).Name;

        private ProgressBar progressBar;
        private ImageView recIndicator;
        private TextView resultTextView;

        private EditText contextTextView;
        private Spinner selectLanguageSpinner;

        private AIService aiService;

        LanguageConfig[] languages;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            recIndicator = FindViewById<ImageView>(Resource.Id.recIndicator);
            recIndicator.Visibility = ViewStates.Invisible;
		
            resultTextView = FindViewById<TextView>(Resource.Id.resultTextView);
            contextTextView = FindViewById<EditText>(Resource.Id.contextTextView);
            selectLanguageSpinner = FindViewById<Spinner>(Resource.Id.selectLanguageSpinner);

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
            
            var languagesAdapter = new ArrayAdapter<LanguageConfig>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, languages);
            selectLanguageSpinner.Adapter = languagesAdapter;
            selectLanguageSpinner.ItemSelected += SelectLanguageSpinner_ItemSelected;

            FindViewById<Button>(Resource.Id.buttonListen).Click += buttonListen_Click;
            FindViewById<Button>(Resource.Id.buttonCancel).Click += buttonCancel_Click;
            FindViewById<Button>(Resource.Id.buttonStopListen).Click += buttonStopListen_Click;
        }

        void buttonListen_Click(object sender, EventArgs e)
        {
			StartParallel(() => aiService.StartListening());
        }

        void buttonStopListen_Click(object sender, EventArgs e)
        {
            StartParallel(aiService.StopListening);
        }

        void buttonCancel_Click(object sender, EventArgs e)
        {
            StartParallel(aiService.Cancel);
        }

        protected override void OnPause()
        {
            Log.Debug(TAG, "OnPause");
            if (aiService != null)
            {
                aiService.Pause();    
            }

            base.OnPause();
        }

        protected override void OnResume()
        {
            Log.Debug(TAG, "OnResume");

            base.OnResume();

            if (aiService != null)
            {
                aiService.Resume();    
            }
        }

        void SelectLanguageSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            
            var selectedLanguage = languages[e.Position];

            var lang = SupportedLanguage.FromLanguageTag(selectedLanguage.LanguageCode);
            var config = new AIConfiguration("62f2522a-7404-4c28-b9ac-097ca5d8b32d",
                             selectedLanguage.AccessToken, lang);
            
			//TODO: Option for verbose logging. Remove this line in production.
			config.DebugLog = true;

            if (aiService != null)
            {
                aiService.Cancel();
            }

			aiService = AIService.CreateService(this, config, RecognitionEngine.ApiAi);

            aiService.OnResult += AiService_OnResult;
            aiService.OnError += AiService_OnError;
            aiService.ListeningStarted += AiService_ListeningStarted;
            aiService.ListeningFinished += AiService_ListeningFinished;
            aiService.AudioLevelChanged += AiService_AudioLevelChanged;
        }

        void AiService_OnResult(ApiAiSDK.Model.AIResponse response)
        {
            Log.Debug(TAG, "AiService_OnResult");
            RunOnUiThread(() =>
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

        void AiService_OnError(AIServiceException exception)
        {
            Log.Debug(TAG, "AiService_OnError", exception.ToString());
            RunOnUiThread(() =>
                {
                    resultTextView.Text = exception.ToString();
                }
            );
        }

        void AiService_AudioLevelChanged(float level)
        {
            Log.Debug(TAG, "AiService_AudioLevelChanged " + level);
            RunOnUiThread(() =>
                {
                    float positiveLevel = Math.Abs(level);
                    if (positiveLevel > 100)
                    {
                        positiveLevel = 100;
                    }

                    progressBar.Progress = (int)positiveLevel;

                }
            );
        }

        void AiService_ListeningFinished()
        {
            Log.Debug(TAG, "AiService_ListeningFinished");
            RunOnUiThread(() =>
                {
                    recIndicator.Visibility = ViewStates.Invisible;
                }
            );
        }

        void AiService_ListeningStarted()
        {
            Log.Debug(TAG, "AiService_ListeningStarted");
            RunOnUiThread(() =>
                {
                    recIndicator.Visibility = ViewStates.Visible;
                }
            );
        }

        private void StartParallel(Action a)
        {
            new Task(a).Start();
        }

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


