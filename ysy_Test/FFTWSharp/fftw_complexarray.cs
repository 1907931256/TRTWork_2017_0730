using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace FFTWSharp
{
	public class fftw_complexarray
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

		public fftw_complexarray(int length)
		{
			this.length = length;
			this.handle = fftw.malloc(this.length * 16);
		}

		public fftw_complexarray(double[] data)
		{
			this.length = data.Length / 2;
			this.handle = fftw.malloc(this.length * 16);
			this.SetData(data);
		}

		public fftw_complexarray(Complex[] data)
		{
			this.length = data.Length;
			this.handle = fftw.malloc(this.length * 16);
			this.SetData(data);
		}

		public void SetData(double[] data)
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
			double[] array = new double[data.Length * 2];
			for (int i = 0; i < data.Length; i++)
			{
				array[2 * i] = data[i].Real;
				array[2 * i + 1] = data[i].Imaginary;
			}
			Marshal.Copy(array, 0, this.handle, this.length * 2);
		}

		public void SetZeroData()
		{
			double[] source = new double[this.Length * 2];
			Marshal.Copy(source, 0, this.handle, this.length * 2);
		}

		public Complex[] GetData_Complex()
		{
			double[] array = new double[this.length * 2];
			Marshal.Copy(this.handle, array, 0, this.length * 2);
			Complex[] array2 = new Complex[this.length];
			for (int i = 0; i < this.length; i++)
			{
				array2[i] = new Complex(array[2 * i], array[2 * i + 1]);
			}
			return array2;
		}

		public double[] GetData_Real()
		{
			double[] array = new double[this.length * 2];
			Marshal.Copy(this.handle, array, 0, this.length * 2);
			double[] array2 = new double[this.length];
			for (int i = 0; i < this.length; i++)
			{
				array2[i] = array[2 * i];
			}
			return array2;
		}

		public double[] GetData_Double()
		{
			double[] array = new double[this.length * 2];
			Marshal.Copy(this.handle, array, 0, this.length * 2);
			return array;
		}

		~fftw_complexarray()
		{
			fftw.free(this.handle);
		}
	}
}
