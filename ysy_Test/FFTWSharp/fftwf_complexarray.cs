using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace FFTWSharp
{
	public class fftwf_complexarray
	{
		private IntPtr handle;

		private int length;

		public IntPtr Handle
		{
			get
			{
				return this.handle;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		public fftwf_complexarray(int length)
		{
			this.length = length;
			this.handle = fftwf.malloc(this.length * 8);
		}

		public fftwf_complexarray(float[] data)
		{
			this.length = data.Length / 2;
			this.handle = fftwf.malloc(this.length * 8);
			this.SetData(data);
		}

		public fftwf_complexarray(Complex[] data)
		{
			this.length = data.Length;
			this.handle = fftw.malloc(this.length * 16);
			this.SetData(data);
		}

		public void SetData(float[] data)
		{
			if (data.Length / 2 != this.length)
			{
				throw new ArgumentException("Array length mismatch!");
			}
			Marshal.Copy(data, 0, this.handle, this.length * 2);
		}

		public void SetData(Complex[] data)
		{
			if (data.Length != this.length)
			{
				throw new ArgumentException("Array length mismatch!");
			}
			float[] array = new float[data.Length * 2];
			for (int i = 0; i < data.Length; i++)
			{
				array[2 * i] = (float)data[i].Real;
				array[2 * i + 1] = (float)data[i].Imaginary;
			}
			Marshal.Copy(array, 0, this.handle, this.length * 2);
		}

		public void SetZeroData()
		{
			float[] source = new float[this.Length * 2];
			Marshal.Copy(source, 0, this.handle, this.length * 2);
		}

		public Complex[] GetData_Complex()
		{
			float[] array = new float[this.length * 2];
			Marshal.Copy(this.handle, array, 0, this.length * 2);
			Complex[] array2 = new Complex[this.length];
			for (int i = 0; i < this.length; i++)
			{
				array2[i] = new Complex((double)array[2 * i], (double)array[2 * i + 1]);
			}
			return array2;
		}

		public float[] GetData_Real()
		{
			float[] array = new float[this.length * 2];
			Marshal.Copy(this.handle, array, 0, this.length * 2);
			float[] array2 = new float[this.length];
			for (int i = 0; i < this.length; i++)
			{
				array2[i] = array[2 * i];
			}
			return array2;
		}

		public float[] GetData_Float()
		{
			float[] array = new float[this.length * 2];
			Marshal.Copy(this.handle, array, 0, this.length * 2);
			return array;
		}

		~fftwf_complexarray()
		{
			fftwf.free(this.handle);
		}
	}
}
