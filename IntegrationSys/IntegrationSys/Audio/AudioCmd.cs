using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationSys.Flow;
using System.Media;
using System.IO;
using System.Numerics;
using FFTWSharp;
using System.Runtime.InteropServices;
using IntegrationSys.LogUtil;

namespace IntegrationSys.Audio
{
    class AudioCmd : IExecutable
    {
        const string ACTION_RECORD_START = "录音";
        const string ACTION_RECORD_STOP = "停止录音";
        const string ACTION_PLAY = "播放声音";
        const string ACTION_STOP = "停止播放";
        const string ACTION_ANALYSIS = "分析";
        const string ACTION_VIBRATE_ANALYSIS = "振子分析";

        private SoundPlayer soundPlayer_;
        private SoundRecorder soundRecorder_;

        private static AudioCmd instance_;

        public static AudioCmd Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new AudioCmd();
                }
                return instance_;
            }
        }

        private AudioCmd() 
        {
            soundRecorder_ = new SoundRecorder();
        }

        public void ExecuteCmd(string action, string param, out string retValue)
        {
            if (ACTION_PLAY == action)
            {
                ExecutePlay(param, out retValue);
            }
            else if (ACTION_STOP == action)
            {
                ExecuteStop(param, out retValue);
            }
            else if (ACTION_RECORD_START == action)
            {
                ExecuteRecordStart(param, out retValue);
            }
            else if (ACTION_RECORD_STOP == action)
            {
                ExecuteRecordStop(param, out retValue);
            }
            else if (ACTION_ANALYSIS == action)
            {
                ExecuteAnalysis(param, out retValue);
            }
            else if (ACTION_VIBRATE_ANALYSIS == action)
            {
                ExecuteVibrateAnalysis(param, out retValue);
            }
            else
            {
                retValue = "Res=CmdNotSupport";
            }
        }

        private void ExecuteRecordStart(string param, out string retValue)
        {
            soundRecorder_.Start(param);
            retValue = "Res=Pass";
        }

        private void ExecuteRecordStop(string param, out string retValue)
        {
            soundRecorder_.Stop();
            retValue = "Res=Pass";
        }

        private void ExecutePlay(string param, out string retValue)
        {
            try
            {
                soundPlayer_ = new SoundPlayer(param);
                soundPlayer_.LoadAsync();
                soundPlayer_.Play();

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
            soundPlayer_.Stop();

            retValue = "Res=Pass";
        }

        private void ExecuteAnalysis(string param, out string retValue)
        {
            string[] parameters = param.Split(' ');
            string filename = parameters[0];
            int start = Int32.Parse(parameters[1]);
            int len = Int32.Parse(parameters[2]);
            int baseFreq = Int32.Parse(parameters[3]);
            int  freqShift = Int32.Parse(parameters[4]);

            retValue = "Res=Fail";
            try
            {
                using (WaveFile waveFile = new WaveFile(filename))
                {
                    int n = CalcFFTPoints(waveFile.SamplesPerSecond, len);

                    if (waveFile.Channels == 2)
                    {
                        int sampleStartIndex = start * waveFile.SamplesPerSecond / 1000;
                        int offset = sampleStartIndex * waveFile.BlockAlign;
                        int count = n * waveFile.BlockAlign;

                        byte[] data = waveFile.GetData(offset, count);

                        using (MemoryStream memStream = new MemoryStream(data))
                        {
                            using (BinaryReader reader = new BinaryReader(memStream))
                            {
                                double[] lData = new double[n];
                                double[] rData = new double[n];

                                for (int i = 0; i < n; i++)
                                {
                                    lData[i] = (double)reader.ReadInt16();
                                    rData[i] = (double)reader.ReadInt16();
                                }

                                Complex[] lcData;
                                Complex[] rcData;
                                fft_r2c(lData, out lcData);
                                fft_r2c(rData, out rcData);

                                double freq1, freq2;
                                double ampl1, ampl2;
                                double thd1, thd2;
                                double noise1, noise2;

                                CalcFreqAndAmpl(n, waveFile.SamplesPerSecond, lcData, out freq1, out ampl1);
                                CalcFreqAndAmpl(n, waveFile.SamplesPerSecond, rcData, out freq2, out ampl2);

                                CalcThd(n, waveFile.SamplesPerSecond, baseFreq, lcData, out thd1);
                                CalcThd(n, waveFile.SamplesPerSecond, baseFreq, rcData, out thd2);

                                CalcNoise(n, waveFile.SamplesPerSecond, baseFreq, freqShift, lcData, out noise1);
                                CalcNoise(n, waveFile.SamplesPerSecond, baseFreq, freqShift, rcData, out noise2);

                                retValue = "Freq1=" + freq1.ToString("F") + ";Ampl1=" + ampl1.ToString("F") + ";Thd1=" + thd1.ToString("F") + ";Noise1=" + noise1.ToString("F");
                                retValue += ";Freq2=" + freq2.ToString("F") + ";Ampl2=" + ampl2.ToString("F") + ";Thd2=" + thd2.ToString("F") + ";Noise2=" + noise2.ToString("F");
                            }
                        }
                    }
                    else
                    {
                        int sampleStartIndex = start * waveFile.SamplesPerSecond / 1000;
                        int offset = sampleStartIndex * waveFile.BlockAlign;
                        int count = n * waveFile.BlockAlign;

                        byte[] data = waveFile.GetData(offset, count);

                        using (MemoryStream memStream = new MemoryStream(data))
                        {
                            using (BinaryReader reader = new BinaryReader(memStream))
                            {
                                double[] lData = new double[n];

                                for (int i = 0; i < n; i++)
                                {
                                    lData[i] = (double)reader.ReadInt16();
                                }

                                Complex[] lcData;
                                fft_r2c(lData, out lcData);

                                double freq1;
                                double ampl1;
                                double thd1;
                                double noise1;

                                CalcFreqAndAmpl(n, waveFile.SamplesPerSecond, lcData, out freq1, out ampl1);

                                CalcThd(n, waveFile.SamplesPerSecond, baseFreq, lcData, out thd1);

                                CalcNoise(n, waveFile.SamplesPerSecond, baseFreq, freqShift, lcData, out noise1);

                                retValue = "Freq1=" + freq1.ToString("F") + ";Ampl1=" + ampl1.ToString("F") + ";Thd1=" + thd1.ToString("F") + ";Noise1=" + noise1.ToString("F");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message, e);
            }

            
        }

        private void ExecuteVibrateAnalysis(string param, out string retValue)
        {
            retValue = "Res=Fail";

            string[] parameters = param.Split(' ');
            string filename = parameters[0];
            int start = Int32.Parse(parameters[1]);
            int len = Int32.Parse(parameters[2]);
            int divider = Int32.Parse(parameters[3]);

            int freqCount = (parameters.Length - 4) / 2;

            try 
	        {	        
		        using (WaveFile waveFile = new WaveFile(filename))
                {
                    int n = CalcFFTPoints(waveFile.SamplesPerSecond, len);
                    double[] lData = new double[n];
                    Complex[] cData;

                    if (waveFile.Channels == 2)
                    {
                        int sampleStartIndex = start * waveFile.SamplesPerSecond / 1000;
                        int offset = sampleStartIndex * waveFile.BlockAlign;
                        int count = n * waveFile.BlockAlign;

                        byte[] data = waveFile.GetData(offset, count);

                        using (MemoryStream memStream = new MemoryStream(data))
                        {
                            using (BinaryReader reader = new BinaryReader(memStream))
                            {
                                for (int i = 0; i < n; i++)
                                {
                                    lData[i] = (double)reader.ReadInt16();
                                    reader.ReadInt16();
                                }
                            }
                        }
                    }
                    else
                    {
                        int sampleStartIndex = start * waveFile.SamplesPerSecond / 1000;
                        int offset = sampleStartIndex * waveFile.BlockAlign;
                        int count = n * waveFile.BlockAlign;

                        byte[] data = waveFile.GetData(offset, count);

                        using (MemoryStream memStream = new MemoryStream(data))
                        {
                            using (BinaryReader reader = new BinaryReader(memStream))
                            {
                                for (int i = 0; i < n; i++)
                                {
                                    lData[i] = (double)reader.ReadInt16();
                                }
                            }
                        }
                    }

                    fft_r2c(lData, out cData);

                    double[] ddata = new double[cData.Length];

                    ddata[0] = cData[0].Magnitude / n;
                    for (int i = 1; i < cData.Length; i++)
                    {
                        ddata[i] = cData[i].Magnitude * 2 / n;
                    }

                    double freqInterval = (double)waveFile.SamplesPerSecond / n;
                    double total = 0;
                    double valid = 0;

                    for (int i = 0; i < ddata.Length; i++)
                    {
                        double freq = i * freqInterval;
                        bool inRange = false;
                        for (int j = 0; j < freqCount; j++)
                        {
                            int freqOrigin = Int32.Parse(parameters[4 + 2 * j]);
                            int freqShift = Int32.Parse(parameters[4 + 2 * j + 1]);

                            double freqLower = freqOrigin - freqShift;
                            double freqUpper = freqOrigin + freqShift;

                            

                            if (freqLower <= freq && freq <= freqUpper)
                            {
                                inRange = true;
                                break;
                            }
                        }

                        if (inRange)
                        {
                            valid += ddata[i];
                        }
                        total += ddata[i];
                    }

                    double validRate = valid / total * 100;
                    double ampl = total / divider / 32.7;

                    retValue = "Ampl=" + ampl.ToString("F") + ";Noise="+validRate.ToString("F");

                }
	        }
	        catch (Exception e)
	        {
		        Log.Debug(e.Message, e);
	        }

     
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sampleRate">采样率</param>
        /// <param name="length">分析时长</param>
        /// <returns></returns>
        private int CalcFFTPoints(int sampleRate, int length)
        {
            int fftPoints = 1;
            int sampleNum = length * sampleRate / 1000;
            while (fftPoints <= sampleNum)
            {
                fftPoints *= 2;
            }
            fftPoints /= 2;

            return fftPoints; 
        }

        /// <summary>
        /// 1维实数傅里叶变换
        /// 由Hermitian定理可知，输出结果有共轭对称性。输入n个实数，输出n/2+1个复数
        /// </summary>
        /// <param name="inData">n个实数，n必须为logical</param>
        /// <param name="output">n/2+1个复数</param>
        private void fft_r2c(double[] inData, out Complex[] output)
        {
            int n = inData.Length;
            IntPtr inHandle = fftw.malloc(n * sizeof(double));
            IntPtr outHandle = fftw.malloc((n/2 + 1) * sizeof(double) * 2);

            Marshal.Copy(inData, 0, inHandle, n);

            IntPtr plan = fftw.dft_r2c_1d(n, inHandle, outHandle, fftw_flags.Estimate);
            fftw.execute(plan);

            double[] dout = new double[(n/2 + 1) * 2];
            Marshal.Copy(outHandle, dout, 0, (n/2 + 1) * 2);

            output = new Complex[n/2 + 1];
            for (int i = 0; i < n / 2 + 1; i++)
            {
                output[i] = new Complex(dout[2 * i], dout[2 * i+1]);
            }

            fftw.destroy_plan(plan);
            fftw.free(inHandle);
            fftw.free(outHandle);
        }

        /// <summary>
        /// 输入一组数据，输出最大值及其索引
        /// </summary>
        /// <param name="?"></param>
        /// <param name="max"></param>
        /// <param name="i"></param>
        private void GetMax(double[] data, out double max, out int i)
        {
            int maxIndex = 0;
            for (int j = 0; j < data.Length; j++)
            {
                if (data[j] > data[maxIndex]) maxIndex = j;
            }

            i = maxIndex;
            max = data[i];
        }

        private void CalcFreqAndAmpl(int n, int sampleRate, Complex[] cdata, out double freq, out double ampl)
        {
            double[] data = new double[cdata.Length];
            
            data[0] = cdata[0].Magnitude / n / 32.7;
            for (int i = 1; i < cdata.Length; i++)
            {
                data[i] = cdata[i].Magnitude * 2 / n / 32.7;
            }

            double max;
            int maxIndex;
            GetMax(data, out max, out maxIndex);

            double freqInterval = (double)sampleRate / n;

            freq = freqInterval * maxIndex;
            ampl = max;

        }

        private void CalcThd(int n, int sampleRate, int baseFreq, Complex[] cdata, out double thd)
        {
            double[] data = new double[cdata.Length];

            data[0] = cdata[0].Magnitude / n;
            for (int i = 1; i < cdata.Length; i++)
            {
                data[i] = cdata[i].Magnitude * 2 / n;
            }

            double freqInterval = (double)sampleRate / n;

            int index = (int)Math.Round((double)baseFreq / freqInterval);
            double g1 = data[index] * data[index];
            double g = 0;

            for (int i = 2; i <= 5; i++)
            {
                g += data[i * index] * data[i * index];
            }

            thd = Math.Sqrt(g / g1) * 100;
        }

        private void CalcNoise(int n, int sampleRate, int baseFreq, int freqShift, Complex[] cdata, out double noise)
        {
            double[] data = new double[cdata.Length];

            data[0] = cdata[0].Magnitude / n;
            for (int i = 1; i < cdata.Length; i++)
            {
                data[i] = cdata[i].Magnitude * 2 / n;
            }

            double freqInterval = (double)sampleRate / n;

            int index = (int)Math.Round((double)baseFreq / freqInterval);

            int minFreq = 0;
            int maxFreq = 20000;
            int multi = maxFreq / baseFreq;

            double valid = 0;
            double total = 0;

            for (int i = 0; i < data.Length; i++)
            {
                double freq = i * freqInterval;
                if (minFreq <= freq && freq <= maxFreq)
                {
                    bool inRange = false;
                    for (int j = 1; j <= multi; j++)
                    {
                        double freqLower = baseFreq * j - freqShift;
                        double freqUpper = baseFreq * j + freqShift;

                        if (freqLower <= freq && freq <= freqUpper)
                        {
                            inRange = true;
                            break;
                        }
                    }

                    if (inRange)
                    {
                        valid += data[i];
                    }
                    total += data[i];
                }
            }

            if (total != 0)
            {
                noise = (total - valid) / total * 100.0;
            }
            else
            {
                noise = 100.0;
            }
        }
    }
}
