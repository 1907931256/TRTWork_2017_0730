using Microsoft.DirectX.DirectSound;
using System;
using System.IO;
using System.Threading;

namespace IntegrationSys.Audio
{
	internal class SoundRecorder
	{
		private const int SAMPLE_RATE = 44100;

		private const int BITS_PER_SAMPLE = 16;

		private const int CHANNELS = 2;

		private const int NOTIFY_NUM = 16;

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
			CaptureDevicesCollection captureDevicesCollection = new CaptureDevicesCollection();
			if (captureDevicesCollection.Count > 0)
			{
				this.capture_ = new Capture(captureDevicesCollection[0].DriverGuid);
			}
			else
			{
				Console.WriteLine("No Capture Device");
			}
			this.waveFormat_ = this.CreateWaveFormat();
			this.notifyEvent_ = new AutoResetEvent(false);
		}

		public bool Start(string filename)
		{
			if (this.capture_ == null)
			{
				return false;
			}
			this.waveFormat_ = this.CreateWaveFormat();
			this.InitCaptureBuffer();
			this.InitNotifications();
			this.InitWaveFile(filename);
			this.captureExit_ = false;
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.ThreadCaptureData));
			this.captureBuffer_.Start(true);
			return true;
		}

		public void Stop()
		{
			this.captureBuffer_.Stop();
			this.captureExit_ = true;
			this.notifyEvent_.Set();
		}

		private WaveFormat CreateWaveFormat()
		{
			WaveFormat result = new WaveFormat();
			result.FormatTag = WaveFormatTag.Pcm;
			result.SamplesPerSecond = 44100;
			result.BitsPerSample = 16;
			result.Channels = 2;
			//result.BlockAlign = result.Channels * (result.BitsPerSample / 8);
			result.AverageBytesPerSecond = (int)result.BlockAlign * result.SamplesPerSecond;
			return result;
		}

		private void InitCaptureBuffer()
		{
			if (this.captureBuffer_ != null)
			{
				this.captureBuffer_.Dispose();
				this.captureBuffer_ = null;
			}
			this.notifySize_ = ((1024 > this.waveFormat_.AverageBytesPerSecond / 8) ? 1024 : (this.waveFormat_.AverageBytesPerSecond / 8));
			this.notifySize_ -= this.notifySize_ % (int)this.waveFormat_.BlockAlign;
			this.captureBufferSize_ = this.notifySize_ * 16;
			this.captureBuffer_ = new CaptureBuffer(new CaptureBufferDescription
			{
				BufferBytes = this.captureBufferSize_,
				Format = this.waveFormat_
			}, this.capture_);
			this.captureOffset_ = 0;
			this.captureDataLength_ = 0;
		}

		private void InitNotifications()
		{
			if (this.notify_ != null)
			{
				this.notify_.Dispose();
				this.notify_ = null;
			}
			BufferPositionNotify[] array = new BufferPositionNotify[17];
			for (int i = 0; i < 16; i++)
			{
				array[i].Offset = this.notifySize_ * i + this.notifySize_ - 1;
				array[i].EventNotifyHandle = this.notifyEvent_.SafeWaitHandle.DangerousGetHandle();
			}
			this.notify_ = new Notify(this.captureBuffer_);
			this.notify_.SetNotificationPositions(array, 16);
		}

		private void InitWaveFile(string filename)
		{
			this.writer_ = new BinaryWriter(new FileStream(filename, FileMode.Create));
			char[] chars = new char[]
			{
				'R',
				'I',
				'F',
				'F'
			};
			char[] chars2 = new char[]
			{
				'W',
				'A',
				'V',
				'E'
			};
			char[] chars3 = new char[]
			{
				'f',
				'm',
				't',
				' '
			};
			char[] chars4 = new char[]
			{
				'd',
				'a',
				't',
				'a'
			};
			int value = 16;
			int value2 = 0;
			this.writer_.Write(chars);
			this.writer_.Write(value2);
			this.writer_.Write(chars2);
			this.writer_.Write(chars3);
			this.writer_.Write(value);
			this.writer_.Write((short)this.waveFormat_.FormatTag);
			this.writer_.Write(this.waveFormat_.Channels);
			this.writer_.Write(this.waveFormat_.SamplesPerSecond);
			this.writer_.Write(this.waveFormat_.AverageBytesPerSecond);
			this.writer_.Write(this.waveFormat_.BlockAlign);
			this.writer_.Write(this.waveFormat_.BitsPerSample);
			this.writer_.Write(chars4);
			this.writer_.Write(0);
		}

		private void CaptureData()
		{
			int num = 0;
			int num2 = 0;
			this.captureBuffer_.GetCurrentPosition(out num2, out num);
			int num3 = num - this.captureOffset_;
			if (num3 < 0)
			{
				num3 += this.captureBufferSize_;
			}
			num3 -= num3 % this.notifySize_;
			if (num3 == 0)
			{
				return;
			}
			byte[] array = (byte[])this.captureBuffer_.Read(this.captureOffset_, typeof(byte), LockFlag.None, new int[]
			{
				num3
			});
			this.writer_.Write(array, 0, array.Length);
			this.captureDataLength_ += array.Length;
			this.captureOffset_ += array.Length;
			this.captureOffset_ %= this.captureBufferSize_;
		}

		private void ThreadCaptureData(object stateInfo)
		{
			while (!this.captureExit_)
			{
				this.notifyEvent_.WaitOne(-1, true);
				this.CaptureData();
			}
			this.writer_.Seek(4, SeekOrigin.Begin);
			this.writer_.Write(this.captureDataLength_ + 36);
			this.writer_.Seek(40, SeekOrigin.Begin);
			this.writer_.Write(this.captureDataLength_);
			this.writer_.Close();
			this.writer_ = null;
		}
	}
}
