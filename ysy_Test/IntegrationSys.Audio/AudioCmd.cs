using FFTWSharp;
using IntegrationSys.Flow;
using IntegrationSys.LogUtil;
using System;
using System.IO;
using System.Media;
using System.Numerics;
using System.Runtime.InteropServices;

namespace IntegrationSys.Audio
{
	internal class AudioCmd : IExecutable
	{
		private const string ACTION_RECORD_START = "录音";

		private const string ACTION_RECORD_STOP = "停止录音";

		private const string ACTION_PLAY = "播放声音";

		private const string ACTION_STOP = "停止播放";

		private const string ACTION_ANALYSIS = "分析";

		private const string ACTION_VIBRATE_ANALYSIS = "振子分析";

		private SoundPlayer soundPlayer_;

		private SoundRecorder soundRecorder_;

		private static AudioCmd instance_;

		public static AudioCmd Instance
		{
			get
			{
				if (AudioCmd.instance_ == null)
				{
					AudioCmd.instance_ = new AudioCmd();
				}
				return AudioCmd.instance_;
			}
		}

		private AudioCmd()
		{
			this.soundRecorder_ = new SoundRecorder();
		}

		public void ExecuteCmd(string action, string param, out string retValue)
		{
			if ("播放声音" == action)
			{
				this.ExecutePlay(param, out retValue);
				return;
			}
			if ("停止播放" == action)
			{
				this.ExecuteStop(param, out retValue);
				return;
			}
			if ("录音" == action)
			{
				this.ExecuteRecordStart(param, out retValue);
				return;
			}
			if ("停止录音" == action)
			{
				this.ExecuteRecordStop(param, out retValue);
				return;
			}
			if ("分析" == action)
			{
				this.ExecuteAnalysis(param, out retValue);
				return;
			}
			if ("振子分析" == action)
			{
				this.ExecuteVibrateAnalysis(param, out retValue);
				return;
			}
			retValue = "Res=CmdNotSupport";
		}

		private void ExecuteRecordStart(string param, out string retValue)
		{
			this.soundRecorder_.Start(param);
			retValue = "Res=Pass";
		}

		private void ExecuteRecordStop(string param, out string retValue)
		{
			this.soundRecorder_.Stop();
			retValue = "Res=Pass";
		}

		private void ExecutePlay(string param, out string retValue)
		{
			try
			{
				this.soundPlayer_ = new SoundPlayer(param);
				this.soundPlayer_.LoadAsync();
				this.soundPlayer_.Play();
				retValue = "Res=Pass";
			}
			catch (UriFormatException)
			{
				retValue = "Res=ArgumentException";
			}
			catch (FileNotFoundException)
			{
				retValue = "Res=FileNotFound";
			}
		}

		private void ExecuteStop(string param, out string retValue)
		{
			this.soundPlayer_.Stop();
			retValue = "Res=Pass";
		}

		private void ExecuteAnalysis(string param, out string retValue)
		{
			string[] array = param.Split(new char[]
			{
				' '
			});
			string filename = array[0];
			int num = int.Parse(array[1]);
			int length = int.Parse(array[2]);
			int baseFreq = int.Parse(array[3]);
			int freqShift = int.Parse(array[4]);
			retValue = "Res=Fail";
			try
			{
				using (WaveFile waveFile = new WaveFile(filename))
				{
					int num2 = this.CalcFFTPoints(waveFile.SamplesPerSecond, length);
					if (waveFile.Channels == 2)
					{
						int num3 = num * waveFile.SamplesPerSecond / 1000;
						int offset = num3 * (int)waveFile.BlockAlign;
						int count = num2 * (int)waveFile.BlockAlign;
						byte[] data = waveFile.GetData(offset, count);
						using (MemoryStream memoryStream = new MemoryStream(data))
						{
							using (BinaryReader binaryReader = new BinaryReader(memoryStream))
							{
								double[] array2 = new double[num2];
								double[] array3 = new double[num2];
								for (int i = 0; i < num2; i++)
								{
									array2[i] = (double)binaryReader.ReadInt16();
									array3[i] = (double)binaryReader.ReadInt16();
								}
								Complex[] cdata;
								this.fft_r2c(array2, out cdata);
								Complex[] cdata2;
								this.fft_r2c(array3, out cdata2);
								double num4;
								double num5;
								this.CalcFreqAndAmpl(num2, waveFile.SamplesPerSecond, cdata, out num4, out num5);
								double num6;
								double num7;
								this.CalcFreqAndAmpl(num2, waveFile.SamplesPerSecond, cdata2, out num6, out num7);
								double num8;
								this.CalcThd(num2, waveFile.SamplesPerSecond, baseFreq, cdata, out num8);
								double num9;
								this.CalcThd(num2, waveFile.SamplesPerSecond, baseFreq, cdata2, out num9);
								double num10;
								this.CalcNoise(num2, waveFile.SamplesPerSecond, baseFreq, freqShift, cdata, out num10);
								double num11;
								this.CalcNoise(num2, waveFile.SamplesPerSecond, baseFreq, freqShift, cdata2, out num11);
								retValue = string.Concat(new string[]
								{
									"Freq1=",
									num4.ToString("F"),
									";Ampl1=",
									num5.ToString("F"),
									";Thd1=",
									num8.ToString("F"),
									";Noise1=",
									num10.ToString("F")
								});
								string text = retValue;
								retValue = string.Concat(new string[]
								{
									text,
									";Freq2=",
									num6.ToString("F"),
									";Ampl2=",
									num7.ToString("F"),
									";Thd2=",
									num9.ToString("F"),
									";Noise2=",
									num11.ToString("F")
								});
							}
							goto IL_3E7;
						}
					}
					int num12 = num * waveFile.SamplesPerSecond / 1000;
					int offset2 = num12 * (int)waveFile.BlockAlign;
					int count2 = num2 * (int)waveFile.BlockAlign;
					byte[] data2 = waveFile.GetData(offset2, count2);
					using (MemoryStream memoryStream2 = new MemoryStream(data2))
					{
						using (BinaryReader binaryReader2 = new BinaryReader(memoryStream2))
						{
							double[] array4 = new double[num2];
							for (int j = 0; j < num2; j++)
							{
								array4[j] = (double)binaryReader2.ReadInt16();
							}
							Complex[] cdata3;
							this.fft_r2c(array4, out cdata3);
							double num13;
							double num14;
							this.CalcFreqAndAmpl(num2, waveFile.SamplesPerSecond, cdata3, out num13, out num14);
							double num15;
							this.CalcThd(num2, waveFile.SamplesPerSecond, baseFreq, cdata3, out num15);
							double num16;
							this.CalcNoise(num2, waveFile.SamplesPerSecond, baseFreq, freqShift, cdata3, out num16);
							retValue = string.Concat(new string[]
							{
								"Freq1=",
								num13.ToString("F"),
								";Ampl1=",
								num14.ToString("F"),
								";Thd1=",
								num15.ToString("F"),
								";Noise1=",
								num16.ToString("F")
							});
						}
					}
					IL_3E7:;
				}
			}
			catch (Exception ex)
			{
				Log.Debug(ex.Message, ex);
			}
		}

		private void ExecuteVibrateAnalysis(string param, out string retValue)
		{
			retValue = "Res=Fail";
			string[] array = param.Split(new char[]
			{
				' '
			});
			string filename = array[0];
			int num = int.Parse(array[1]);
			int length = int.Parse(array[2]);
			int num2 = int.Parse(array[3]);
			int num3 = (array.Length - 4) / 2;
			try
			{
				using (WaveFile waveFile = new WaveFile(filename))
				{
					int num4 = this.CalcFFTPoints(waveFile.SamplesPerSecond, length);
					double[] array2 = new double[num4];
					if (waveFile.Channels == 2)
					{
						int num5 = num * waveFile.SamplesPerSecond / 1000;
						int offset = num5 * (int)waveFile.BlockAlign;
						int count = num4 * (int)waveFile.BlockAlign;
						byte[] data = waveFile.GetData(offset, count);
						using (MemoryStream memoryStream = new MemoryStream(data))
						{
							using (BinaryReader binaryReader = new BinaryReader(memoryStream))
							{
								for (int i = 0; i < num4; i++)
								{
									array2[i] = (double)binaryReader.ReadInt16();
									binaryReader.ReadInt16();
								}
							}
							goto IL_184;
						}
					}
					int num6 = num * waveFile.SamplesPerSecond / 1000;
					int offset2 = num6 * (int)waveFile.BlockAlign;
					int count2 = num4 * (int)waveFile.BlockAlign;
					byte[] data2 = waveFile.GetData(offset2, count2);
					using (MemoryStream memoryStream2 = new MemoryStream(data2))
					{
						using (BinaryReader binaryReader2 = new BinaryReader(memoryStream2))
						{
							for (int j = 0; j < num4; j++)
							{
								array2[j] = (double)binaryReader2.ReadInt16();
							}
						}
					}
					IL_184:
					Complex[] array3;
					this.fft_r2c(array2, out array3);
					double[] array4 = new double[array3.Length];
					array4[0] = array3[0].Magnitude / (double)num4;
					for (int k = 1; k < array3.Length; k++)
					{
						array4[k] = array3[k].Magnitude * 2.0 / (double)num4;
					}
					double num7 = (double)waveFile.SamplesPerSecond / (double)num4;
					double num8 = 0.0;
					double num9 = 0.0;
					for (int l = 0; l < array4.Length; l++)
					{
						double num10 = (double)l * num7;
						bool flag = false;
						for (int m = 0; m < num3; m++)
						{
							int num11 = int.Parse(array[4 + 2 * m]);
							int num12 = int.Parse(array[4 + 2 * m + 1]);
							double num13 = (double)(num11 - num12);
							double num14 = (double)(num11 + num12);
							if (num13 <= num10 && num10 <= num14)
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							num9 += array4[l];
						}
						num8 += array4[l];
					}
					double num15 = num9 / num8 * 100.0;
					retValue = "Ampl=" + (num8 / (double)num2 / 32.7).ToString("F") + ";Noise=" + num15.ToString("F");
				}
			}
			catch (Exception ex)
			{
				Log.Debug(ex.Message, ex);
			}
		}

		private int CalcFFTPoints(int sampleRate, int length)
		{
			int i = 1;
			int num = length * sampleRate / 1000;
			while (i <= num)
			{
				i *= 2;
			}
			return i / 2;
		}

		private void fft_r2c(double[] inData, out Complex[] output)
		{
			int num = inData.Length;
			IntPtr intPtr = fftw.malloc(num * 8);
			IntPtr intPtr2 = fftw.malloc((num / 2 + 1) * 8 * 2);
			Marshal.Copy(inData, 0, intPtr, num);
			IntPtr plan = fftw.dft_r2c_1d(num, intPtr, intPtr2, fftw_flags.Estimate);
			fftw.execute(plan);
			double[] array = new double[(num / 2 + 1) * 2];
			Marshal.Copy(intPtr2, array, 0, (num / 2 + 1) * 2);
			output = new Complex[num / 2 + 1];
			for (int i = 0; i < num / 2 + 1; i++)
			{
				output[i] = new Complex(array[2 * i], array[2 * i + 1]);
			}
			fftw.destroy_plan(plan);
			fftw.free(intPtr);
			fftw.free(intPtr2);
		}

		private void GetMax(double[] data, out double max, out int i)
		{
			int num = 0;
			for (int j = 0; j < data.Length; j++)
			{
				if (data[j] > data[num])
				{
					num = j;
				}
			}
			i = num;
			max = data[i];
		}

		private void CalcFreqAndAmpl(int n, int sampleRate, Complex[] cdata, out double freq, out double ampl)
		{
			double[] array = new double[cdata.Length];
			array[0] = cdata[0].Magnitude / (double)n / 32.7;
			for (int i = 1; i < cdata.Length; i++)
			{
				array[i] = cdata[i].Magnitude * 2.0 / (double)n / 32.7;
			}
			double num;
			int num2;
			this.GetMax(array, out num, out num2);
			double num3 = (double)sampleRate / (double)n;
			freq = num3 * (double)num2;
			ampl = num;
		}

		private void CalcThd(int n, int sampleRate, int baseFreq, Complex[] cdata, out double thd)
		{
			double[] array = new double[cdata.Length];
			array[0] = cdata[0].Magnitude / (double)n;
			for (int i = 1; i < cdata.Length; i++)
			{
				array[i] = cdata[i].Magnitude * 2.0 / (double)n;
			}
			double num = (double)sampleRate / (double)n;
			int num2 = (int)Math.Round((double)baseFreq / num);
			double num3 = array[num2] * array[num2];
			double num4 = 0.0;
			for (int j = 2; j <= 5; j++)
			{
				num4 += array[j * num2] * array[j * num2];
			}
			thd = Math.Sqrt(num4 / num3) * 100.0;
		}

		private void CalcNoise(int n, int sampleRate, int baseFreq, int freqShift, Complex[] cdata, out double noise)
		{
			double[] array = new double[cdata.Length];
			array[0] = cdata[0].Magnitude / (double)n;
			for (int i = 1; i < cdata.Length; i++)
			{
				array[i] = cdata[i].Magnitude * 2.0 / (double)n;
			}
			double num = (double)sampleRate / (double)n;
			Math.Round((double)baseFreq / num);
			int num2 = 0;
			int num3 = 20000;
			int num4 = num3 / baseFreq;
			double num5 = 0.0;
			double num6 = 0.0;
			for (int j = 0; j < array.Length; j++)
			{
				double num7 = (double)j * num;
				if ((double)num2 <= num7 && num7 <= (double)num3)
				{
					bool flag = false;
					for (int k = 1; k <= num4; k++)
					{
						double num8 = (double)(baseFreq * k - freqShift);
						double num9 = (double)(baseFreq * k + freqShift);
						if (num8 <= num7 && num7 <= num9)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						num5 += array[j];
					}
					num6 += array[j];
				}
			}
			if (num6 != 0.0)
			{
				noise = (num6 - num5) / num6 * 100.0;
				return;
			}
			noise = 100.0;
		}
	}
}
