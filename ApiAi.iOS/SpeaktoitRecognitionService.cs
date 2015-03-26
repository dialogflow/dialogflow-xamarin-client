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
using ApiAi.Common;
using AVFoundation;

namespace ApiAi.iOS
{
    public class SpeaktoitRecognitionService : BaseSpeaktoitRecognitionService
    {
        private readonly string TAG = typeof(SpeaktoitRecognitionService).Name;

        private SoundRecorder soundRecorder;

        private AudioStream audioStream;


        IDisposable anotherSessionNotificationToken;

        public SpeaktoitRecognitionService(AIConfiguration config) : base(config)
        {
            soundRecorder = new SoundRecorder();
        }


        #region implemented abstract members of BaseSpeaktoitRecognitionService
        protected override void StartRecording()
        {
            audioStream = soundRecorder.CreateAudioStream();
            soundRecorder.StartRecording();

            anotherSessionNotificationToken = AVAudioSession.Notifications.ObserveInterruption(OnAnotherSessionStarts);
        }

        protected override void StopRecording()
        {
            soundRecorder.StopRecording();
            if (anotherSessionNotificationToken != null)
            {
                anotherSessionNotificationToken.Dispose();
            }
        }

        protected override void StartVoiceRequest()
        {
            Log.Debug(TAG, "StartVoiceRequest");
            try
            {
                DoServiceRequest(audioStream);
            }
            catch (OperationCanceledException)
            {
                // Do nothing, because of request was cancelled in standard way
                Log.Debug(TAG, "StartVoiceRequest - OperationCancelled");
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "StartVoiceRequest - Exception", e);
                FireOnError(new AIServiceException(e));
            }
        }
     
        #endregion


        private void OnAnotherSessionStarts(object sender, AVAudioSessionInterruptionEventArgs args)
        {
            if (soundRecorder != null && soundRecorder.Active)
            {
                if (args.InterruptionType == AVAudioSessionInterruptionType.Began)
                {
                    // Cancel process, and do not start it again because of it means user start do something another
                    // and will restart process if necessary
                    Cancel();
                }    
            }
        }

    }
}

