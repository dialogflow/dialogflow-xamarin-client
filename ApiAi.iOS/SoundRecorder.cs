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
using Foundation;
using AVFoundation;

using ApiAi.Common;
using AudioToolbox;

namespace ApiAi.iOS
{
    /// <summary>
    /// Object for recording PCM sound
    /// </summary>
    public class SoundRecorder
    {
     
        private const int CountAudioBuffers = 3;
        private const int AudioBufferLength = 32000;
        private const int SampleRate = 16000;
        private const int BitsPerChannel = 16;
        private const int Channels = 1;

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

        public SoundRecorder()
        {
            if (AVAudioSession.SharedInstance().Category == "RECORD"
               || AVAudioSession.SharedInstance().Category == "PLAY_RECORD")
            {
            }

            audioStreamDescription = new AudioStreamBasicDescription
            {
                    Format = AudioFormatType.LinearPCM,
                    FormatFlags = 
                        AudioFormatFlags.LinearPCMIsSignedInteger |
                        AudioFormatFlags.LinearPCMIsPacked,

                    SampleRate = SampleRate,
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
            var status = inputQueue.Start();
            if (status == AudioQueueStatus.Ok)
            {
                outputAudioStream.StartRecording();
            }
            else
            {
                throw new AudioRecorderException("Sound recording initialization failure: " + status);
            }
        }

        public void StopRecording()
        {
            if (inputQueue.IsRunning)
            {
                inputQueue.Stop(true);
                outputAudioStream.EndRecording();

            }
        }

        private void HandleInputCompleted(object sender, InputCompletedEventArgs e)
        {
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
            var soundData = new byte[buffer.AudioDataByteSize];
            System.Runtime.InteropServices.Marshal.Copy(buffer.AudioData, soundData, 0, (int)buffer.AudioDataByteSize);

            if (outputAudioStream != null)
            {
                outputAudioStream.Write(soundData, 0, soundData.Length);
            }
        }
    }
}

