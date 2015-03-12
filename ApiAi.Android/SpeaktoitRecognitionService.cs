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

using ApiAiSDK;
using ApiAi.Common;

namespace ApiAi.Android
{
    /// <summary>
    /// Record sound and makes requests to the API.AI recognition and processing service
    /// </summary>
    public class SpeaktoitRecognitionService : BaseSpeaktoitRecognitionService
    {
        private const int SAMPLE_RATE_IN_HZ = 16000;
        private const ChannelIn CHANNEL_CONFIG = ChannelIn.Mono;
        private const Encoding ENCODING = Encoding.Pcm16bit;

        private AudioRecord audioRecord;

        public SpeaktoitRecognitionService(AIConfiguration config) : base(config)
        {
        }

        private void InitRecorder()
        {
            var minBufferSize = AudioRecord.GetMinBufferSize(SAMPLE_RATE_IN_HZ, CHANNEL_CONFIG, ENCODING);
            audioRecord = new AudioRecord(AudioSource.Mic, SAMPLE_RATE_IN_HZ, CHANNEL_CONFIG, ENCODING, minBufferSize);
        }

        #region implemented abstract members of BaseSpeaktoitRecognitionService

        protected override void StartRecording()
        {
            audioRecord.StartRecording();
        }

        protected override void StopRecording()
        {
            audioRecord.Stop();
        }

        #endregion

        /// <summary>
        /// To pause service when app goes to background
        /// </summary>
        public override void Pause()
        {
            StopRecording();

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
            if (audioRecord == null) 
            {
                InitRecorder();
            }
        }

        protected override void StartVoiceRequest()
        {
            try
            {
                var audioStream = new AudioStream(audioRecord);
                DoServiceRequest(audioStream);
            }
            catch(OperationCanceledException)
            {
                // Do nothing, because of request was cancelled in standard way
            }
            catch(System.Exception e)
            {
                FireOnError(new AIServiceException(e));
            }

        }

    }

}

