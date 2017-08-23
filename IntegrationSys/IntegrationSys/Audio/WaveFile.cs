using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IntegrationSys.LogUtil;

namespace IntegrationSys.Audio
{
    class WaveHead
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

    class WaveFile : IDisposable
    {
        private WaveHead waveHead_;
        private BinaryReader reader_;

        private long dataPosition_;

        public WaveFile(string filename)
        {
            try
            {
                waveHead_ = new WaveHead();
                reader_ = new BinaryReader(File.OpenRead(filename));

                reader_.BaseStream.Seek(16, SeekOrigin.Begin);
                int fmtSize = reader_.ReadInt32();
                waveHead_.FormatTag = reader_.ReadInt16();
                waveHead_.Channels = reader_.ReadInt16();
                waveHead_.SamplesPerSecond = reader_.ReadInt32();
                waveHead_.AverageBytesPerSecond = reader_.ReadInt32();
                waveHead_.BlockAlign = reader_.ReadInt16();
                waveHead_.BitsPerSample = reader_.ReadInt16();
                if (fmtSize > 16)
                {
                    reader_.BaseStream.Seek(fmtSize - 16, SeekOrigin.Current);
                }
                string chunkId = System.Text.Encoding.ASCII.GetString(reader_.ReadBytes(4));
                if (chunkId == "fact")
                {
                    int factSize = reader_.ReadInt32();
                    reader_.BaseStream.Seek(factSize, SeekOrigin.Current);

                    chunkId = System.Text.Encoding.ASCII.GetString(reader_.ReadBytes(4));
                    if (chunkId != "data")
                    {
                        throw new FormatException(filename + " is not wav file");
                    }
                }
                else if (chunkId != "data")
                {
                    throw new FormatException(filename + " is not wav file");
                }

                DataSize = reader_.ReadInt32();
                dataPosition_ = reader_.BaseStream.Position;
            }
            catch (Exception e)
            {
                Log.Debug(e.Message, e);
            }
        }

        ~WaveFile()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (reader_ != null)
                {
                    reader_.Dispose();
                }
            }
        }

        public short FormatTag
        {
            get
            {
                return waveHead_.FormatTag;
            }
        }

        public short Channels
        {
            get
            {
                return waveHead_.Channels;
            }
        }

        public int SamplesPerSecond
        {
            get
            {
                return waveHead_.SamplesPerSecond;
            }
        }

        public int AverageBytesPerSecond
        {
            get
            {
                return waveHead_.AverageBytesPerSecond;
            }
        }

        public short BlockAlign
        {
            get
            {
                return waveHead_.BlockAlign;
            }
        }

        public short BitsPerSample
        {
            get
            {
                return waveHead_.BitsPerSample;
            }
        }

        public int DataSize
        {
            get;
            set;
        }

        public byte[] GetData()
        {
            return GetData(0, Int32.MaxValue);
        }

        public byte[] GetData(int offset, int count)
        {
            if (offset < 0 || count <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (offset >= DataSize || offset + count > DataSize)
            {
                throw new ArgumentException();
            }

            reader_.BaseStream.Seek(dataPosition_ + offset, SeekOrigin.Begin);
            return reader_.ReadBytes(count);
        }
    }
}
