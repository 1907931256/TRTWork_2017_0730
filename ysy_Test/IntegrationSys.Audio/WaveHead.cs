using System;

namespace IntegrationSys.Audio
{
	internal class WaveHead
	{
		public short FormatTag
		{
			get;
			set;
		}

		public short Channels
		{
			get;
			set;
		}

		public int SamplesPerSecond
		{
			get;
			set;
		}

		public int AverageBytesPerSecond
		{
			get;
			set;
		}

		public short BlockAlign
		{
			get;
			set;
		}

		public short BitsPerSample
		{
			get;
			set;
		}
	}
}
