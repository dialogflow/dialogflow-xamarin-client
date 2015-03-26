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
using System.Diagnostics;

namespace ApiAi.iOS
{
    public class AudioStream : Stream
    {
        private readonly string TAG = typeof(AudioStream).Name;

        private readonly MemoryStream innerStream;
        private readonly object innerStreamLock = new object();

        private long readPosition;
        private long writePosition;

        private AutoResetEvent dataAvailable = new AutoResetEvent(false);

        public AudioStream()
        {
            innerStream = new MemoryStream();
        }


        public override bool CanRead { get { return true; } }

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return true; } }

        public override void Flush()
        {
            lock (innerStreamLock)
            {
                innerStream.Flush();
            }
        }

        public override long Length
        {
            get
            {
                lock (innerStreamLock)
                {
                    return innerStream.Length;
                }
            }
        }

        public override long Position
        {
            get
            {
                lock (innerStreamLock)
                {
                    return innerStream.Position;
                }
            }

            set { throw new NotSupportedException(); }
        }

        private bool ProduceInProcess { get; set; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Log.Debug(TAG, "ProduceInProcess {0}, readPosition {1}, innerStream.Length {2}", ProduceInProcess, readPosition, innerStream.Length);
            if (ProduceInProcess && readPosition >= innerStream.Length)
            {
                dataAvailable.Reset();

                Log.Debug(TAG, "Waiting for Write...");
                dataAvailable.WaitOne();
                Log.Debug(TAG, "Wait complete");
            }

            lock (innerStreamLock)
            {
                innerStream.Position = readPosition;
                var red = innerStream.Read(buffer, offset, count);
                readPosition = innerStream.Position;

                Log.Debug(TAG, "Read " + count + " - " + red);

                return red;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Log.Debug(TAG, "Write " + count);
            lock (innerStreamLock)
            {
                innerStream.Position = writePosition;
                innerStream.Write(buffer, offset, count);
                writePosition = innerStream.Position;

                dataAvailable.Set();
            }
        }

        public void EndRecording()
        {
            Log.Debug(TAG, "EndRecording");
            ProduceInProcess = false;
            dataAvailable.Set();
        }

        public void StartRecording()
        {
            ProduceInProcess = true;
        }
    }
}

