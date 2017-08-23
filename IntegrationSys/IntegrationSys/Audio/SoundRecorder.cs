using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using System.Threading;
using System.IO;

namespace IntegrationSys.Audio
{
    class SoundRecorder
    {
        const int SAMPLE_RATE = 44100;
        const int BITS_PER_SAMPLE = 16;
        const int CHANNELS = 2;

        const int NOTIFY_NUM = 16;

        private Capture capture_;
        private CaptureBuffer captureBuffer_;
        private int captureBufferSize_;
        private int captureOffset_;
        private int captureDataLength_;

        private bool captureExit_;

        private int notifySize_;
        private Notify notify_;
        private AutoResetEvent notifyEvent_;

        private WaveFormat waveFormat_;

        private BinaryWriter writer_;

        public SoundRecorder()
        {
            CaptureDevicesCollection devices = new CaptureDevicesCollection();

            if (devices.Count > 0)
            {
                capture_ = new Capture(devices[0].DriverGuid);
            }
            else
            {
                Console.WriteLine("No Capture Device");
            }

            waveFormat_ = CreateWaveFormat();

            notifyEvent_ = new AutoResetEvent(false);
        }

        public bool Start(string filename)
        {
            if (capture_ == null) return false;
            waveFormat_ = CreateWaveFormat();
            InitCaptureBuffer();
            InitNotifications();
            InitWaveFile(filename);

            captureExit_ = false;
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadCaptureData));
            captureBuffer_.Start(true);

            return true;
        }

        public void Stop()
        {
            captureBuffer_.Stop();
            captureExit_ = true;
            notifyEvent_.Set();

            // 写WAV文件尾  
            //writer_.Seek(4, SeekOrigin.Begin);
            //writer_.Write((int)(captureDataLength_ + 36));   // 写文件长度  
            //writer_.Seek(40, SeekOrigin.Begin);
            //writer_.Write(captureDataLength_);                // 写数据长度  
            //writer_.Close();
            //writer_ = null;
        }

        private WaveFormat CreateWaveFormat()
        {
            WaveFormat waveFormat = new WaveFormat();
            waveFormat.FormatTag = WaveFormatTag.Pcm;
            waveFormat.SamplesPerSecond = SAMPLE_RATE;
            waveFormat.BitsPerSample = BITS_PER_SAMPLE;
            waveFormat.Channels = CHANNELS;
            waveFormat.BlockAlign = (short)(waveFormat.Channels * (waveFormat.BitsPerSample / 8));
            waveFormat.AverageBytesPerSecond = waveFormat.BlockAlign * waveFormat.SamplesPerSecond;

            return waveFormat;
        }

        private void InitCaptureBuffer()
        {
            if (captureBuffer_ != null)
            {
                captureBuffer_.Dispose();
                captureBuffer_ = null;
            }

            notifySize_ = (1024 > waveFormat_.AverageBytesPerSecond / 8) ? 1024 : (waveFormat_.AverageBytesPerSecond / 8);
            notifySize_ -= notifySize_ % waveFormat_.BlockAlign; 
            captureBufferSize_ = notifySize_ * NOTIFY_NUM;

            CaptureBufferDescription bufferDescription = new CaptureBufferDescription();
            bufferDescription.BufferBytes = captureBufferSize_;
            bufferDescription.Format = waveFormat_;

            captureBuffer_ = new CaptureBuffer(bufferDescription, capture_);

            captureOffset_ = 0;
            captureDataLength_ = 0;
        }

        private void InitNotifications()
        {
            if (notify_ != null)
            {
                notify_.Dispose();
                notify_ = null;
            }

            BufferPositionNotify[] positionNotify = new BufferPositionNotify[NOTIFY_NUM + 1];
            for (int i = 0; i < NOTIFY_NUM; i++)
            {
                positionNotify[i].Offset = (notifySize_ * i) + notifySize_ - 1;
                positionNotify[i].EventNotifyHandle = notifyEvent_.SafeWaitHandle.DangerousGetHandle();
            }

            notify_ = new Notify(captureBuffer_);
            notify_.SetNotificationPositions(positionNotify, NOTIFY_NUM);
        }

        private void InitWaveFile(string filename)
        {
            writer_ = new BinaryWriter(new FileStream(filename, FileMode.Create));
            /**************************************************************************
                Here is where the file will be created. A
                wave file is a RIFF file, which has chunks
                of data that describe what the file contains.
                A wave RIFF file is put together like this:
                The 12 byte RIFF chunk is constructed like this:
                Bytes 0 - 3 :  'R' 'I' 'F' 'F'
                Bytes 4 - 7 :  Length of file, minus the first 8 bytes of the RIFF description.
                                  (4 bytes for "WAVE" + 24 bytes for format chunk length +
                                  8 bytes for data chunk description + actual sample data size.)
                 Bytes 8 - 11: 'W' 'A' 'V' 'E'
                 The 24 byte FORMAT chunk is constructed like this:
                 Bytes 0 - 3 : 'f' 'm' 't' ' '
                 Bytes 4 - 7 : The format chunk length. This is always 16.
                 Bytes 8 - 9 : File padding. Always 1.
                 Bytes 10- 11: Number of channels. Either 1 for mono,  or 2 for stereo.
                 Bytes 12- 15: Sample rate.
                 Bytes 16- 19: Number of bytes per second.
                 Bytes 20- 21: Bytes per sample. 1 for 8 bit mono, 2 for 8 bit stereo or
                                 16 bit mono, 4 for 16 bit stereo.
                 Bytes 22- 23: Number of bits per sample.
                 The DATA chunk is constructed like this:
                 Bytes 0 - 3 : 'd' 'a' 't' 'a'
                 Bytes 4 - 7 : Length of data, in bytes.
                 Bytes 8 -: Actual sample data.
               ***************************************************************************/
            char[] chunkRiff = { 'R', 'I', 'F', 'F' };
            char[] chunkType = { 'W', 'A', 'V', 'E' };
            char[] chunkFmt = { 'f', 'm', 't', ' ' };
            char[] chunkData = { 'd', 'a', 't', 'a' };
            int formatChunkLength = 0x10;  // Format chunk length.
            int length = 0;                // File length, minus first 8 bytes of RIFF description. This will be filled in later.
            // RIFF 块
            writer_.Write(chunkRiff);
            writer_.Write(length);
            writer_.Write(chunkType);
            // WAVE块
            writer_.Write(chunkFmt);
            writer_.Write(formatChunkLength);
            writer_.Write((short)waveFormat_.FormatTag);
            writer_.Write(waveFormat_.Channels);
            writer_.Write(waveFormat_.SamplesPerSecond);
            writer_.Write(waveFormat_.AverageBytesPerSecond);
            writer_.Write(waveFormat_.BlockAlign);
            writer_.Write(waveFormat_.BitsPerSample);
            // 数据块
            writer_.Write(chunkData);
            writer_.Write((int)0);   // The sample length will be written in later.
        }

        private void CaptureData()
        {
            int readPos = 0, capturePos = 0, lockSize = 0;
            captureBuffer_.GetCurrentPosition(out capturePos, out readPos);
            lockSize = readPos - captureOffset_;
            if (lockSize < 0)       // 因为是循环的使用缓冲区，所以有一种情况下为负：当文以载读指针回到第一个通知点，而Ibuffeoffset还在最后一个通知处  
                lockSize += captureBufferSize_;
            lockSize -= (lockSize % notifySize_);   // 对齐缓冲区边界,实际上由于开始设定完整,这个操作是多余的.  
            if (0 == lockSize)
                return;

            // 读取缓冲区内的数据  
            byte[] captureData = (byte[])captureBuffer_.Read(captureOffset_, typeof(byte), LockFlag.None, lockSize);
            // 写入Wav文件  
            writer_.Write(captureData, 0, captureData.Length);
            // 更新已经录制的数据长度.  
            captureDataLength_ += captureData.Length;
            // 移动录制数据的起始点,通知消息只负责指示产生消息的位置,并不记录上次录制的位置  
            captureOffset_ += captureData.Length;
            captureOffset_ %= captureBufferSize_; // Circular buffer  
        }

        private void ThreadCaptureData(Object stateInfo)
        {
            while (!captureExit_)
            {
                // 等待缓冲区的通知消息  
                notifyEvent_.WaitOne(Timeout.Infinite, true);
                // 录制数据  
                CaptureData();
            }

            // 写WAV文件尾  
            writer_.Seek(4, SeekOrigin.Begin);
            writer_.Write((int)(captureDataLength_ + 36));   // 写文件长度  
            writer_.Seek(40, SeekOrigin.Begin);
            writer_.Write(captureDataLength_);                // 写数据长度  
            writer_.Close();
            writer_ = null;
        }

    }
}
