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
using System.IO;

using System.Threading;
using System.Threading.Tasks;

using Java.Lang;
using Android;
using Android.Media;
using Android.Util;

using ApiAiSDK;
using ApiAi.Common;
using ApiAiSDK.Util;
using Android.Content;

namespace ApiAi.Android
{
    /// <summary>
    /// Record sound and makes requests to the API.AI recognition and processing service
    /// </summary>
    public class SpeaktoitRecognitionService : BaseSpeaktoitRecognitionService
    {
        private readonly string TAG = typeof(SpeaktoitRecognitionService).Name;
        
        private const int SAMPLE_RATE_IN_HZ = 16000;
        private const ChannelIn CHANNEL_CONFIG = ChannelIn.Mono;
        private const Encoding ENCODING = Encoding.Pcm16bit;

        private AudioRecord audioRecord;

        private VoiceActivityDetector vad = new VoiceActivityDetector(SAMPLE_RATE_IN_HZ);

        public SpeaktoitRecognitionService(Context context, AIConfiguration config)
            : base(config)
        {
            InitRecorder();
        }

        private void InitRecorder()
        {
            var minBufferSize = AudioRecord.GetMinBufferSize(SAMPLE_RATE_IN_HZ, CHANNEL_CONFIG, ENCODING);
            audioRecord = new AudioRecord(AudioSource.Mic, SAMPLE_RATE_IN_HZ, CHANNEL_CONFIG, ENCODING, minBufferSize);

            vad.Enabled = config.VoiceActivityDetectionEnabled;

            vad.SpeechBegin += Vad_SpeechBegin;
            vad.SpeechEnd += Vad_SpeechEnd;
            vad.AudioLevelChange += Vad_AudioLevelChange;
        }

        void Vad_AudioLevelChange(float level)
        {
            OnAudioLevelChanged(level);
        }

        void Vad_SpeechBegin()
        {
            Log.Debug(TAG, "Vad_SpeechBegin");
            new Task(OnSpeechBegin).Start();
        }

        void Vad_SpeechEnd()
        {
            Log.Debug(TAG, "Vad_SpeechEnd");
            new Task(OnSpeechEnd).Start();
            new Task(StopListening).Start();
        }

        #region implemented abstract members of BaseSpeaktoitRecognitionService

        protected override void StartRecording()
        {
            Log.Debug(TAG, "StartRecording");
            vad.Reset();
            audioRecord.StartRecording();
        }

        protected override void StopRecording()
        {
            Log.Debug(TAG, "StopRecording");
            audioRecord.Stop();
        }

        #endregion

        /// <summary>
        /// To pause service when app goes to background
        /// </summary>
        public override void Pause()
        {
            Log.Debug(TAG, "Pause");
            StopListening();

            if (audioRecord != null)
            {
                audioRecord.Release();
                audioRecord = null;
            }

        }

        /// <summary>
        /// To resume service on resuming app
        /// </summary>
        public override void Resume()
        {
            Log.Debug(TAG, "Resume");
            if (audioRecord == null)
            {
                InitRecorder();
            }
        }

        protected override void StartVoiceRequest()
        {
            Log.Debug(TAG, "StartVoiceRequest");
            try
            {
                var audioStream = new AudioStream(audioRecord, vad);
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

    }

}

