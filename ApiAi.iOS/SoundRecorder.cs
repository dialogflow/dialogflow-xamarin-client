/**
 * Copyright 2017 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using Foundation;
using AVFoundation;

using ApiAi.Common;
using AudioToolbox;
using ApiAiSDK.Util;
using ApiAi.Common.Logging;
using ApiAiSDK;

namespace ApiAi.iOS
{
    /// <summary>
    /// Object for recording PCM sound
    /// </summary>
    public class SoundRecorder
    {
        private readonly string TAG = typeof(SoundRecorder).Name;
     
        private const int CountAudioBuffers = 3;
        private const int AudioBufferLength = 3200;
        private const int BitsPerChannel = 16;
        private const int Channels = 1;

        private readonly VoiceActivityDetector vad;

        private AudioStreamBasicDescription audioStreamDescription;
        private InputAudioQueue inputQueue;
       
        private AudioStream outputAudioStream;

        public bool Active
        {
            get
            {
                return inputQueue.IsRunning;
            }
        }

        public SoundRecorder(int sampleRate, VoiceActivityDetector vad)
        {
            this.vad = vad;

            if (AVAudioSession.SharedInstance().Category != AVAudioSession.CategoryRecord
                && AVAudioSession.SharedInstance().Category != AVAudioSession.CategoryPlayAndRecord)
            {
                throw new AIServiceException("AVAudioCategory not set to RECORD or PLAY_AND_RECORD. Please set it using AVAudioSession class.");
            }

            audioStreamDescription = new AudioStreamBasicDescription
            {
                    Format = AudioFormatType.LinearPCM,
                    FormatFlags = 
                        AudioFormatFlags.LinearPCMIsSignedInteger |
                        AudioFormatFlags.LinearPCMIsPacked,

                    SampleRate = sampleRate,
                    BitsPerChannel = BitsPerChannel,
                    ChannelsPerFrame = Channels,
                    BytesPerFrame = (BitsPerChannel / 8) * Channels,
                    FramesPerPacket = 1,
                    Reserved = 0, 
            };

            audioStreamDescription.BytesPerPacket = audioStreamDescription.BytesPerFrame * audioStreamDescription.FramesPerPacket;

            inputQueue = CreateInputQueue(audioStreamDescription);
        }

        private InputAudioQueue CreateInputQueue(AudioStreamBasicDescription streamDescription)
        {
            var queue = new InputAudioQueue(streamDescription);

            for (int count = 0; count < CountAudioBuffers; count++)
            {
                IntPtr bufferPointer;
                queue.AllocateBuffer(AudioBufferLength, out bufferPointer);
                queue.EnqueueBuffer(bufferPointer, AudioBufferLength, null);
            }
            queue.InputCompleted += HandleInputCompleted;
            return queue;
        }
            
        public AudioStream CreateAudioStream()
        {
            outputAudioStream = new AudioStream();
            return outputAudioStream;
        }

        public void StartRecording()
        {
            Log.Debug(TAG, "StartRecording");

            var status = inputQueue.Start();
            Log.Debug(TAG, "Start status: " + status);

            if (status == AudioQueueStatus.Ok)
            {
                outputAudioStream.StartRecording();
                Log.Debug(TAG, "Started");
            }
            else
            {
                throw new AudioRecorderException("Sound recording initialization failure: " + status);
            }
        }

        public void StopRecording()
        {
            Log.Debug(TAG, "StopRecording");

            if (inputQueue.IsRunning)
            {
                var stopStatus = inputQueue.Pause();
                //var stopStatus = inputQueue.Stop();

                Log.Debug(TAG, "Stop status " + stopStatus);

                outputAudioStream.EndRecording();
            }
        }

        private void HandleInputCompleted(object sender, InputCompletedEventArgs e)
        {
            Log.Debug(TAG, "HandleInputCompleted");

            if (!Active)
            {
                return;
            }

            var buffer = (AudioQueueBuffer)System.Runtime.InteropServices.Marshal.PtrToStructure(e.IntPtrBuffer, typeof(AudioQueueBuffer));

            ProcessBuffer(buffer);

            var status = inputQueue.EnqueueBuffer(e.IntPtrBuffer, AudioBufferLength, e.PacketDescriptions);  
        }

        private void ProcessBuffer(AudioQueueBuffer buffer)
        {
            Log.Debug(TAG, "AudioQueueBuffer size:" + buffer.AudioDataByteSize);

            if (buffer.AudioDataByteSize > 0)
            {
                var soundData = new byte[buffer.AudioDataByteSize];
                System.Runtime.InteropServices.Marshal.Copy(buffer.AudioData, soundData, 0, (int)buffer.AudioDataByteSize);

                if (outputAudioStream != null)
                {
                    outputAudioStream.Write(soundData, 0, soundData.Length);
                    vad.ProcessBufferEx(soundData, soundData.Length);
                }    
            }
        }
    }
}

