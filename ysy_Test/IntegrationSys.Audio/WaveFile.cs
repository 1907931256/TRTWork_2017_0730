using IntegrationSys.LogUtil;
using System;
using System.IO;
using System.Text;

namespace IntegrationSys.Audio
{
	internal class WaveFile : IDisposable
	{
		private WaveHead waveHead_;

		private BinaryReader reader_;

		private long dataPosition_;

		public short FormatTag
		{
			get
			{
				return this.waveHead_.FormatTag;
			}
		}

		public short Channels
		{
			get
			{
				return this.waveHead_.Channels;
			}
		}

		public int SamplesPerSecond
		{
			get
			{
				return this.waveHead_.SamplesPerSecond;
			}
		}

		public int AverageBytesPerSecond
		{
			get
			{
				return this.waveHead_.AverageBytesPerSecond;
			}
		}

		public short BlockAlign
		{
			get
			{
				return this.waveHead_.BlockAlign;
			}
		}

		public short BitsPerSample
		{
			get
			{
				return this.waveHead_.BitsPerSample;
			}
		}

		public int DataSize
		{
			get;
			set;
		}

		public WaveFile(string filename)
		{
			try
			{
				this.waveHead_ = new WaveHead();
				this.reader_ = new BinaryReader(File.OpenRead(filename));
				this.reader_.BaseStream.Seek(16L, SeekOrigin.Begin);
				int num = this.reader_.ReadInt32();
				this.waveHead_.FormatTag = this.reader_.ReadInt16();
				this.waveHead_.Channels = this.reader_.ReadInt16();
				this.waveHead_.SamplesPerSecond = this.reader_.ReadInt32();
				this.waveHead_.AverageBytesPerSecond = this.reader_.ReadInt32();
				this.waveHead_.BlockAlign = this.reader_.ReadInt16();
				this.waveHead_.BitsPerSample = this.reader_.ReadInt16();
				if (num > 16)
				{
					this.reader_.BaseStream.Seek((long)(num - 16), SeekOrigin.Current);
				}
				string @string = Encoding.ASCII.GetString(this.reader_.ReadBytes(4));
				if (@string == "fact")
				{
					int num2 = this.reader_.ReadInt32();
					this.reader_.BaseStream.Seek((long)num2, SeekOrigin.Current);
					@string = Encoding.ASCII.GetString(this.reader_.ReadBytes(4));
					if (@string != "data")
					{
						throw new FormatException(filename + " is not wav file");
					}
				}
				else if (@string != "data")
				{
					throw new FormatException(filename + " is not wav file");
				}
				this.DataSize = this.reader_.ReadInt32();
				this.dataPosition_ = this.reader_.BaseStream.Position;
			}
			catch (Exception ex)
			{
				Log.Debug(ex.Message, ex);
			}
		}

		~WaveFile()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.reader_ != null)
			{
				this.reader_.Dispose();
			}
		}

		public byte[] GetData()
		{
			return this.GetData(0, 2147483647);
		}

		public byte[] GetData(int offset, int count)
		{
			if (offset < 0 || count <= 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (offset >= this.DataSize || offset + count > this.DataSize)
			{
				throw new ArgumentException();
			}
			this.reader_.BaseStream.Seek(this.dataPosition_ + (long)offset, SeekOrigin.Begin);
			return this.reader_.ReadBytes(count);
		}
	}
}
