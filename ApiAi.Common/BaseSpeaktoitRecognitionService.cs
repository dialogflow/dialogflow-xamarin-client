﻿//
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
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace ApiAi.Common
{
    public abstract class BaseSpeaktoitRecognitionService : AIService
    {
        private readonly object audioRecordLock = new object();
        private volatile bool isRecording = false;

        private CancellationTokenSource cancellationTokenSource;


        protected BaseSpeaktoitRecognitionService(AIConfiguration config) : base(config)
        {
        }

        protected abstract void StartRecording();

        protected abstract void StartVoiceRequest();

        protected abstract void StopRecording();


        public override void StartListening()
        {
            lock(audioRecordLock)
            {
                if(!isRecording)
                {
                    try
                    {
                        StartRecording();
                        isRecording = true;

                        OnListeningStarted();

                        cancellationTokenSource = new CancellationTokenSource();
                        var parallelOptions = new ParallelOptions();
                        parallelOptions.CancellationToken = cancellationTokenSource.Token;

                        Parallel.Invoke(parallelOptions, () => {
                            StartVoiceRequest();
                        });    
                    }
                    catch (Exception ex)
                    {
                        FireOnError(new AIServiceException(ex));
                    }

                }
            }
        }


        public override void StopListening()
        {
            lock (audioRecordLock)
            {
                if (isRecording)
                {
                    try
                    {
                        StopRecording();
                        isRecording = false;

                        OnListeningFinished();
                    }
                    catch (Exception ex)
                    {
                        FireOnError(new AIServiceException(ex));
                    }
                }
            }
        }

        public override void Cancel()
        {
            lock (audioRecordLock)
            {
                if (isRecording)
                {
                    StopRecording();
                    isRecording = false;

                    OnListeningFinished();
                }
                else
                {
                    var cts = cancellationTokenSource;
                    if (cts != null && !cts.IsCancellationRequested)
                    {
                        cts.Cancel();
                        cts.Dispose();
                        cancellationTokenSource = null;
                    }
                }
            }
        }

        protected void DoServiceRequest(Stream audioStream)
        {
            var response = dataService.VoiceRequest(audioStream);
            FireOnResult(response); 
        }
    }
}

