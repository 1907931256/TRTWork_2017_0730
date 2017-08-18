using System;
using System.Threading;

namespace FFTWSharp
{
	public class fftwf_plan
	{
		private static Mutex FFTW_Lock = new Mutex();

		protected IntPtr handle;

		public IntPtr Handle
		{
			get
			{
				return this.handle;
			}
		}

		public void Execute()
		{
			fftwf.execute(this.handle);
		}

		~fftwf_plan()
		{
			fftwf.destroy_plan(this.handle);
		}

		//public static fftwf_plan dft_1d(int n, fftwf_complexarray input, fftwf_complexarray output, fftw_direction direction, fftw_flags flags)
		//{
		//	fftwf_plan.FFTW_Lock.WaitOne();
		//	fftwf_plan fftwf_plan = new fftwf_plan();
		//	fftwf_plan.handle = fftwf.dft_1d(n, input.Handle, output.Handle, direction, flags);
		//	fftwf_plan.FFTW_Lock.ReleaseMutex();
		//	return fftwf_plan;
		//}

		//public static fftwf_plan dft_2d(int nx, int ny, fftwf_complexarray input, fftwf_complexarray output, fftw_direction direction, fftw_flags flags)
		//{
		//	fftwf_plan.FFTW_Lock.WaitOne();
		//	fftwf_plan fftwf_plan = new fftwf_plan();
		//	fftwf_plan.handle = fftwf.dft_2d(nx, ny, input.Handle, output.Handle, direction, flags);
		//	fftwf_plan.FFTW_Lock.ReleaseMutex();
		//	return fftwf_plan;
		//}

		//public static fftwf_plan dft_3d(int nx, int ny, int nz, fftwf_complexarray input, fftwf_complexarray output, fftw_direction direction, fftw_flags flags)
		//{
		//	fftwf_plan.FFTW_Lock.WaitOne();
		//	fftwf_plan fftwf_plan = new fftwf_plan();
		//	fftwf_plan.handle = fftwf.dft_3d(nx, ny, nz, input.Handle, output.Handle, direction, flags);
		//	fftwf_plan.FFTW_Lock.ReleaseMutex();
		//	return fftwf_plan;
		//}

		//public static fftwf_plan dft(int rank, int[] n, fftwf_complexarray input, fftwf_complexarray output, fftw_direction direction, fftw_flags flags)
		//{
		//	fftwf_plan.FFTW_Lock.WaitOne();
		//	fftwf_plan fftwf_plan = new fftwf_plan();
		//	fftwf_plan.handle = fftwf.dft(rank, n, input.Handle, output.Handle, direction, flags);
		//	fftwf_plan.FFTW_Lock.ReleaseMutex();
		//	return fftwf_plan;
		//}

		//public static fftwf_plan dft_r2c_1d(int n, fftwf_complexarray input, fftwf_complexarray output, fftw_flags flags)
		//{
		//	fftwf_plan.FFTW_Lock.WaitOne();
		//	fftwf_plan fftwf_plan = new fftwf_plan();
		//	fftwf_plan.handle = fftwf.dft_r2c_1d(n, input.Handle, output.Handle, flags);
		//	fftwf_plan.FFTW_Lock.ReleaseMutex();
		//	return fftwf_plan;
		//}

		//public static fftwf_plan dft_r2c_2d(int nx, int ny, fftwf_complexarray input, fftwf_complexarray output, fftw_flags flags)
		//{
		//	fftwf_plan.FFTW_Lock.WaitOne();
		//	fftwf_plan fftwf_plan = new fftwf_plan();
		//	fftwf_plan.handle = fftwf.dft_r2c_2d(nx, ny, input.Handle, output.Handle, flags);
		//	fftwf_plan.FFTW_Lock.ReleaseMutex();
		//	return fftwf_plan;
		//}

		//public static fftwf_plan dft_r2c_3d(int nx, int ny, int nz, fftwf_complexarray input, fftwf_complexarray output, fftw_flags flags)
		//{
		//	fftwf_plan.FFTW_Lock.WaitOne();
		//	fftwf_plan fftwf_plan = new fftwf_plan();
		//	fftwf_plan.handle = fftwf.dft_r2c_3d(nx, ny, nz, input.Handle, output.Handle, flags);
		//	fftwf_plan.FFTW_Lock.ReleaseMutex();
		//	return fftwf_plan;
		//}

		//public static fftwf_plan dft_r2c(int rank, int[] n, fftwf_complexarray input, fftwf_complexarray output, fftw_flags flags)
		//{
		//	fftwf_plan.FFTW_Lock.WaitOne();
		//	fftwf_plan fftwf_plan = new fftwf_plan();
		//	fftwf_plan.handle = fftwf.dft_r2c(rank, n, input.Handle, output.Handle, flags);
		//	fftwf_plan.FFTW_Lock.ReleaseMutex();
		//	return fftwf_plan;
		//}

		//public static fftwf_plan dft_c2r_1d(int n, fftwf_complexarray input, fftwf_complexarray output, fftw_direction direction, fftw_flags flags)
		//{
		//	fftwf_plan.FFTW_Lock.WaitOne();
		//	fftwf_plan fftwf_plan = new fftwf_plan();
		//	fftwf_plan.handle = fftwf.dft_c2r_1d(n, input.Handle, output.Handle, flags);
		//	fftwf_plan.FFTW_Lock.ReleaseMutex();
		//	return fftwf_plan;
		//}

		//public static fftwf_plan dft_c2r_2d(int nx, int ny, fftwf_complexarray input, fftwf_complexarray output, fftw_direction direction, fftw_flags flags)
		//{
		//	fftwf_plan.FFTW_Lock.WaitOne();
		//	fftwf_plan fftwf_plan = new fftwf_plan();
		//	fftwf_plan.handle = fftwf.dft_c2r_2d(nx, ny, input.Handle, output.Handle, flags);
		//	fftwf_plan.FFTW_Lock.ReleaseMutex();
		//	return fftwf_plan;
		//}

		//public static fftwf_plan dft_c2r_3d(int nx, int ny, int nz, fftwf_complexarray input, fftwf_complexarray output, fftw_direction direction, fftw_flags flags)
		//{
		//	fftwf_plan.FFTW_Lock.WaitOne();
		//	fftwf_plan fftwf_plan = new fftwf_plan();
		//	fftwf_plan.handle = fftwf.dft_c2r_3d(nx, ny, nz, input.Handle, output.Handle, flags);
		//	fftwf_plan.FFTW_Lock.ReleaseMutex();
		//	return fftwf_plan;
		//}

		//public static fftwf_plan dft_c2r(int rank, int[] n, fftwf_complexarray input, fftwf_complexarray output, fftw_direction direction, fftw_flags flags)
		//{
		//	fftwf_plan.FFTW_Lock.WaitOne();
		//	fftwf_plan fftwf_plan = new fftwf_plan();
		//	fftwf_plan.handle = fftwf.dft_c2r(rank, n, input.Handle, output.Handle, flags);
		//	fftwf_plan.FFTW_Lock.ReleaseMutex();
		//	return fftwf_plan;
		//}

		//public static fftwf_plan r2r_1d(int n, fftwf_complexarray input, fftwf_complexarray output, fftw_kind kind, fftw_flags flags)
		//{
		//	fftwf_plan.FFTW_Lock.WaitOne();
		//	fftwf_plan fftwf_plan = new fftwf_plan();
		//	fftwf_plan.handle = fftwf.r2r_1d(n, input.Handle, output.Handle, kind, flags);
		//	fftwf_plan.FFTW_Lock.ReleaseMutex();
		//	return fftwf_plan;
		//}

		//public static fftwf_plan r2r_2d(int nx, int ny, fftwf_complexarray input, fftwf_complexarray output, fftw_kind kindx, fftw_kind kindy, fftw_flags flags)
		//{
		//	fftwf_plan.FFTW_Lock.WaitOne();
		//	fftwf_plan fftwf_plan = new fftwf_plan();
		//	fftwf_plan.handle = fftwf.r2r_2d(nx, ny, input.Handle, output.Handle, kindx, kindy, flags);
		//	fftwf_plan.FFTW_Lock.ReleaseMutex();
		//	return fftwf_plan;
		//}

		//public static fftwf_plan r2r_3d(int nx, int ny, int nz, fftwf_complexarray input, fftwf_complexarray output, fftw_kind kindx, fftw_kind kindy, fftw_kind kindz, fftw_flags flags)
		//{
		//	fftwf_plan.FFTW_Lock.WaitOne();
		//	fftwf_plan fftwf_plan = new fftwf_plan();
		//	fftwf_plan.handle = fftwf.r2r_3d(nx, ny, nz, input.Handle, output.Handle, kindx, kindy, kindz, flags);
		//	fftwf_plan.FFTW_Lock.ReleaseMutex();
		//	return fftwf_plan;
		//}

		//public static fftwf_plan r2r(int rank, int[] n, fftwf_complexarray input, fftwf_complexarray output, fftw_kind[] kind, fftw_flags flags)
		//{
		//	fftwf_plan.FFTW_Lock.WaitOne();
		//	fftwf_plan fftwf_plan = new fftwf_plan();
		//	fftwf_plan.handle = fftwf.r2r(rank, n, input.Handle, output.Handle, kind, flags);
		//	fftwf_plan.FFTW_Lock.ReleaseMutex();
		//	return fftwf_plan;
		//}
	}
}
