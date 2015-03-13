//
//  API.AI Xamarin SDK - client-side libraries for API.AI
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
using ApiAiSDK;
using ApiAiSDK.Model;

namespace ApiAi.Common
{
    public abstract class AIService
    {
        protected readonly AIConfiguration config;
        protected readonly AIDataService dataService;

        public event Action SpeechBegin;
        public event Action SpeechEnd;
        public event Action<float> AudioLevelChanged;

        public event Action ListeningStarted;
        public event Action ListeningFinished;

        public event Action<AIResponse> OnResult;
        public event Action<AIServiceException> OnError;

//        public static AIService Create(AIConfiguration config)
//        {
//
//        }

        protected AIService(AIConfiguration config)
        {
            this.config = config;
            dataService = new AIDataService(config);
        }

        /// <summary>
        /// Starts the voice recording
        /// </summary>
        public abstract void StartListening();

        /// <summary>
        /// Stops the voice recording
        /// </summary>
        public abstract void StopListening();

        /// <summary>
        /// Cancel voce recording and web requests
        /// </summary>
        public abstract void Cancel();

        /// <summary>
        /// Call this method while application goes to background to release audio resources
        /// </summary>
        public virtual void Pause()
        {
        }

        /// <summary>
        /// Call this method while application goes from background to lock audio resources
        /// </summary>
        public virtual void Resume()
        {
        }

        protected virtual void OnSpeechBegin()
        {
            SpeechBegin.InvokeSafely();
        }

        protected virtual void OnSpeechEnd()
        {
            var handler = SpeechEnd;
            if (handler != null)
                handler();
        }

        protected virtual void OnAudioLevelChanged(float level)
        {
            var handler = AudioLevelChanged;
            if (handler != null)
                handler(level);
        }

        protected virtual void OnListeningStarted()
        {
            var handler = ListeningStarted;
            if (handler != null)
                handler();
        }

        protected virtual void OnListeningFinished()
        {
            var handler = ListeningFinished;
            if (handler != null)
                handler();
        }

        protected virtual void FireOnResult(AIResponse response)
        {
            OnResult.InvokeSafely(response);
        }

        protected virtual void FireOnError(AIServiceException aiException)
        {
            var handler = OnError;
            if (handler != null)
            {
                handler(aiException);
            }
        }
    }
}

