using System;
using System.Threading;

namespace FFTWSharp
{
	public class fftw_plan
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
			fftw.execute(this.handle);
		}

		~fftw_plan()
		{
			fftw.destroy_plan(this.handle);
		}

		//public static fftw_plan dft_1d(int n, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
		//{
		//	fftw_plan.FFTW_Lock.WaitOne();
		//	fftw_plan fftw_plan = new fftw_plan();
		//	fftw_plan.handle = fftw.dft_1d(n, input.Handle, output.Handle, direction, flags);
		//	fftw_plan.FFTW_Lock.ReleaseMutex();
		//	return fftw_plan;
		//}

		//public static fftw_plan dft_2d(int nx, int ny, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
		//{
		//	fftw_plan.FFTW_Lock.WaitOne();
		//	fftw_plan fftw_plan = new fftw_plan();
		//	fftw_plan.handle = fftw.dft_2d(nx, ny, input.Handle, output.Handle, direction, flags);
		//	fftw_plan.FFTW_Lock.ReleaseMutex();
		//	return fftw_plan;
		//}

		//public static fftw_plan dft_3d(int nx, int ny, int nz, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
		//{
		//	fftw_plan.FFTW_Lock.WaitOne();
		//	fftw_plan fftw_plan = new fftw_plan();
		//	fftw_plan.handle = fftw.dft_3d(nx, ny, nz, input.Handle, output.Handle, direction, flags);
		//	fftw_plan.FFTW_Lock.ReleaseMutex();
		//	return fftw_plan;
		//}

		//public static fftw_plan dft(int rank, int[] n, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
		//{
		//	fftw_plan.FFTW_Lock.WaitOne();
		//	fftw_plan fftw_plan = new fftw_plan();
		//	fftw_plan.handle = fftw.dft(rank, n, input.Handle, output.Handle, direction, flags);
		//	fftw_plan.FFTW_Lock.ReleaseMutex();
		//	return fftw_plan;
		//}

		//public static fftw_plan dft_r2c_1d(int n, fftw_complexarray input, fftw_complexarray output, fftw_flags flags)
		//{
		//	fftw_plan.FFTW_Lock.WaitOne();
		//	fftw_plan fftw_plan = new fftw_plan();
		//	fftw_plan.handle = fftw.dft_r2c_1d(n, input.Handle, output.Handle, flags);
		//	fftw_plan.FFTW_Lock.ReleaseMutex();
		//	return fftw_plan;
		//}

		//public static fftw_plan dft_r2c_2d(int nx, int ny, fftw_complexarray input, fftw_complexarray output, fftw_flags flags)
		//{
		//	fftw_plan.FFTW_Lock.WaitOne();
		//	fftw_plan fftw_plan = new fftw_plan();
		//	fftw_plan.handle = fftw.dft_r2c_2d(nx, ny, input.Handle, output.Handle, flags);
		//	fftw_plan.FFTW_Lock.ReleaseMutex();
		//	return fftw_plan;
		//}

		//public static fftw_plan dft_r2c_3d(int nx, int ny, int nz, fftw_complexarray input, fftw_complexarray output, fftw_flags flags)
		//{
		//	fftw_plan.FFTW_Lock.WaitOne();
		//	fftw_plan fftw_plan = new fftw_plan();
		//	fftw_plan.handle = fftw.dft_r2c_3d(nx, ny, nz, input.Handle, output.Handle, flags);
		//	fftw_plan.FFTW_Lock.ReleaseMutex();
		//	return fftw_plan;
		//}

		//public static fftw_plan dft_r2c(int rank, int[] n, fftw_complexarray input, fftw_complexarray output, fftw_flags flags)
		//{
		//	fftw_plan.FFTW_Lock.WaitOne();
		//	fftw_plan fftw_plan = new fftw_plan();
		//	fftw_plan.handle = fftw.dft_r2c(rank, n, input.Handle, output.Handle, flags);
		//	fftw_plan.FFTW_Lock.ReleaseMutex();
		//	return fftw_plan;
		//}

		//public static fftw_plan dft_c2r_1d(int n, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
		//{
		//	fftw_plan.FFTW_Lock.WaitOne();
		//	fftw_plan fftw_plan = new fftw_plan();
		//	fftw_plan.handle = fftw.dft_c2r_1d(n, input.Handle, output.Handle, flags);
		//	fftw_plan.FFTW_Lock.ReleaseMutex();
		//	return fftw_plan;
		//}

		//public static fftw_plan dft_c2r_2d(int nx, int ny, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
		//{
		//	fftw_plan.FFTW_Lock.WaitOne();
		//	fftw_plan fftw_plan = new fftw_plan();
		//	fftw_plan.handle = fftw.dft_c2r_2d(nx, ny, input.Handle, output.Handle, flags);
		//	fftw_plan.FFTW_Lock.ReleaseMutex();
		//	return fftw_plan;
		//}

		//public static fftw_plan dft_c2r_3d(int nx, int ny, int nz, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
		//{
		//	fftw_plan.FFTW_Lock.WaitOne();
		//	fftw_plan fftw_plan = new fftw_plan();
		//	fftw_plan.handle = fftw.dft_c2r_3d(nx, ny, nz, input.Handle, output.Handle, flags);
		//	fftw_plan.FFTW_Lock.ReleaseMutex();
		//	return fftw_plan;
		//}

		//public static fftw_plan dft_c2r(int rank, int[] n, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
		//{
		//	fftw_plan.FFTW_Lock.WaitOne();
		//	fftw_plan fftw_plan = new fftw_plan();
		//	fftw_plan.handle = fftw.dft_c2r(rank, n, input.Handle, output.Handle, flags);
		//	fftw_plan.FFTW_Lock.ReleaseMutex();
		//	return fftw_plan;
		//}

		//public static fftw_plan r2r_1d(int n, fftw_complexarray input, fftw_complexarray output, fftw_kind kind, fftw_flags flags)
		//{
		//	fftw_plan.FFTW_Lock.WaitOne();
		//	fftw_plan fftw_plan = new fftw_plan();
		//	fftw_plan.handle = fftw.r2r_1d(n, input.Handle, output.Handle, kind, flags);
		//	fftw_plan.FFTW_Lock.ReleaseMutex();
		//	return fftw_plan;
		//}

		//public static fftw_plan r2r_2d(int nx, int ny, fftw_complexarray input, fftw_complexarray output, fftw_kind kindx, fftw_kind kindy, fftw_flags flags)
		//{
		//	fftw_plan.FFTW_Lock.WaitOne();
		//	fftw_plan fftw_plan = new fftw_plan();
		//	fftw_plan.handle = fftw.r2r_2d(nx, ny, input.Handle, output.Handle, kindx, kindy, flags);
		//	fftw_plan.FFTW_Lock.ReleaseMutex();
		//	return fftw_plan;
		//}

		//public static fftw_plan r2r_3d(int nx, int ny, int nz, fftw_complexarray input, fftw_complexarray output, fftw_kind kindx, fftw_kind kindy, fftw_kind kindz, fftw_flags flags)
		//{
		//	fftw_plan.FFTW_Lock.WaitOne();
		//	fftw_plan fftw_plan = new fftw_plan();
		//	fftw_plan.handle = fftw.r2r_3d(nx, ny, nz, input.Handle, output.Handle, kindx, kindy, kindz, flags);
		//	fftw_plan.FFTW_Lock.ReleaseMutex();
		//	return fftw_plan;
		//}

		//public static fftw_plan r2r(int rank, int[] n, fftw_complexarray input, fftw_complexarray output, fftw_kind[] kind, fftw_flags flags)
		//{
		//	fftw_plan.FFTW_Lock.WaitOne();
		//	fftw_plan fftw_plan = new fftw_plan();
		//	fftw_plan.handle = fftw.r2r(rank, n, input.Handle, output.Handle, kind, flags);
		//	fftw_plan.FFTW_Lock.ReleaseMutex();
		//	return fftw_plan;
		//}
	}
}
