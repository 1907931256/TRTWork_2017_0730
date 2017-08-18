using System;
using System.Runtime.InteropServices;

namespace FFTWSharp
{
	public class fftw
	{
		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_malloc", ExactSpelling = true)]
		public static extern IntPtr malloc(int length);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_free", ExactSpelling = true)]
		public static extern void free(IntPtr mem);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_destroy_plan", ExactSpelling = true)]
		public static extern void destroy_plan(IntPtr plan);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_cleanup", ExactSpelling = true)]
		public static extern void cleanup();

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_set_timelimit", ExactSpelling = true)]
		public static extern void set_timelimit(double seconds);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_execute", ExactSpelling = true)]
		public static extern void execute(IntPtr plan);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_plan_dft_1d", ExactSpelling = true)]
		public static extern IntPtr dft_1d(int n, IntPtr input, IntPtr output, fftw_direction direction, fftw_flags flags);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_plan_dft_2d", ExactSpelling = true)]
		public static extern IntPtr dft_2d(int nx, int ny, IntPtr input, IntPtr output, fftw_direction direction, fftw_flags flags);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_plan_dft_3d", ExactSpelling = true)]
		public static extern IntPtr dft_3d(int nx, int ny, int nz, IntPtr input, IntPtr output, fftw_direction direction, fftw_flags flags);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_plan_dft", ExactSpelling = true)]
		public static extern IntPtr dft(int rank, int[] n, IntPtr input, IntPtr output, fftw_direction direction, fftw_flags flags);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_plan_dft_r2c_1d", ExactSpelling = true)]
		public static extern IntPtr dft_r2c_1d(int n, IntPtr input, IntPtr output, fftw_flags flags);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_plan_dft_r2c_2d", ExactSpelling = true)]
		public static extern IntPtr dft_r2c_2d(int nx, int ny, IntPtr input, IntPtr output, fftw_flags flags);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_plan_dft_r2c_3d", ExactSpelling = true)]
		public static extern IntPtr dft_r2c_3d(int nx, int ny, int nz, IntPtr input, IntPtr output, fftw_flags flags);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_plan_dft_r2c", ExactSpelling = true)]
		public static extern IntPtr dft_r2c(int rank, int[] n, IntPtr input, IntPtr output, fftw_flags flags);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_plan_dft_c2r_1d", ExactSpelling = true)]
		public static extern IntPtr dft_c2r_1d(int n, IntPtr input, IntPtr output, fftw_flags flags);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_plan_dft_c2r_2d", ExactSpelling = true)]
		public static extern IntPtr dft_c2r_2d(int nx, int ny, IntPtr input, IntPtr output, fftw_flags flags);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_plan_dft_c2r_3d", ExactSpelling = true)]
		public static extern IntPtr dft_c2r_3d(int nx, int ny, int nz, IntPtr input, IntPtr output, fftw_flags flags);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_plan_dft_c2r", ExactSpelling = true)]
		public static extern IntPtr dft_c2r(int rank, int[] n, IntPtr input, IntPtr output, fftw_flags flags);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_plan_r2r_1d", ExactSpelling = true)]
		public static extern IntPtr r2r_1d(int n, IntPtr input, IntPtr output, fftw_kind kind, fftw_flags flags);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_plan_r2r_2d", ExactSpelling = true)]
		public static extern IntPtr r2r_2d(int nx, int ny, IntPtr input, IntPtr output, fftw_kind kindx, fftw_kind kindy, fftw_flags flags);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_plan_r2r_3d", ExactSpelling = true)]
		public static extern IntPtr r2r_3d(int nx, int ny, int nz, IntPtr input, IntPtr output, fftw_kind kindx, fftw_kind kindy, fftw_kind kindz, fftw_flags flags);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_plan_r2r", ExactSpelling = true)]
		public static extern IntPtr r2r(int rank, int[] n, IntPtr input, IntPtr output, fftw_kind[] kind, fftw_flags flags);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_flops", ExactSpelling = true)]
		public static extern void flops(IntPtr plan, ref double add, ref double mul, ref double fma);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_print_plan", ExactSpelling = true)]
		public static extern void print_plan(IntPtr plan);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_export_wisdom_to_filename", ExactSpelling = true)]
		public static extern void export_wisdom_to_filename(string filename);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftw_import_wisdom_from_filename", ExactSpelling = true)]
		public static extern void import_wisdom_from_filename(string filename);

		[DllImport("libfftw3-3.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void fftw_forget_wisdom();
	}
}
