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
using Android.Media;
using Android.Util;
using ApiAiSDK.Util;

namespace ApiAi.Android
{
    internal class AudioStream : System.IO.Stream
    {
        private readonly string TAG = typeof(AudioStream).Name;

        private readonly AudioRecord audioRecord;

        private VoiceActivityDetector vad;

        internal AudioStream(AudioRecord record)
        {
            audioRecord = record;
        }

        internal AudioStream(AudioRecord record, VoiceActivityDetector vad)
        {
            this.vad = vad;
            audioRecord = record;
        }

        #region implemented abstract members of Stream

        public override void Flush()
        {

        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytesRead = audioRecord.Read(buffer, offset, count);

            if (bytesRead > 0)
            {
                vad.ProcessBufferEx(buffer, bytesRead);
                return bytesRead;
            }

            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException();
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException();
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override long Length
        {
            get
            {
                throw new InvalidOperationException();
            }
        }

        public override long Position
        {
            get
            {
                throw new InvalidOperationException();
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        #endregion
    }
}

