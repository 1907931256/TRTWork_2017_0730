using System;
using System.Runtime.InteropServices;

namespace FFTWSharp
{
	public class fftwf
	{
		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_malloc", ExactSpelling = true)]
		public static extern IntPtr malloc(int length);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_free", ExactSpelling = true)]
		public static extern void free(IntPtr mem);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_destroy_plan", ExactSpelling = true)]
		public static extern void destroy_plan(IntPtr plan);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_cleanup", ExactSpelling = true)]
		public static extern void cleanup();

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_set_timelimit", ExactSpelling = true)]
		public static extern void set_timelimit(double seconds);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_execute", ExactSpelling = true)]
		public static extern void execute(IntPtr plan);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_dft_1d", ExactSpelling = true)]
		public static extern IntPtr dft_1d(int n, IntPtr input, IntPtr output, fftw_direction direction, fftw_flags flags);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_dft_2d", ExactSpelling = true)]
		public static extern IntPtr dft_2d(int nx, int ny, IntPtr input, IntPtr output, fftw_direction direction, fftw_flags flags);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_dft_3d", ExactSpelling = true)]
		public static extern IntPtr dft_3d(int nx, int ny, int nz, IntPtr input, IntPtr output, fftw_direction direction, fftw_flags flags);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_dft", ExactSpelling = true)]
		public static extern IntPtr dft(int rank, int[] n, IntPtr input, IntPtr output, fftw_direction direction, fftw_flags flags);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_dft_r2c_1d", ExactSpelling = true)]
		public static extern IntPtr dft_r2c_1d(int n, IntPtr input, IntPtr output, fftw_flags flags);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_dft_r2c_2d", ExactSpelling = true)]
		public static extern IntPtr dft_r2c_2d(int nx, int ny, IntPtr input, IntPtr output, fftw_flags flags);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_dft_r2c_3d", ExactSpelling = true)]
		public static extern IntPtr dft_r2c_3d(int nx, int ny, int nz, IntPtr input, IntPtr output, fftw_flags flags);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_dft_r2c", ExactSpelling = true)]
		public static extern IntPtr dft_r2c(int rank, int[] n, IntPtr input, IntPtr output, fftw_flags flags);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_dft_c2r_1d", ExactSpelling = true)]
		public static extern IntPtr dft_c2r_1d(int n, IntPtr input, IntPtr output, fftw_flags flags);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_dft_c2r_2d", ExactSpelling = true)]
		public static extern IntPtr dft_c2r_2d(int nx, int ny, IntPtr input, IntPtr output, fftw_flags flags);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_dft_c2r_3d", ExactSpelling = true)]
		public static extern IntPtr dft_c2r_3d(int nx, int ny, int nz, IntPtr input, IntPtr output, fftw_flags flags);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_dft_c2r", ExactSpelling = true)]
		public static extern IntPtr dft_c2r(int rank, int[] n, IntPtr input, IntPtr output, fftw_flags flags);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_r2r_1d", ExactSpelling = true)]
		public static extern IntPtr r2r_1d(int n, IntPtr input, IntPtr output, fftw_kind kind, fftw_flags flags);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_r2r_2d", ExactSpelling = true)]
		public static extern IntPtr r2r_2d(int nx, int ny, IntPtr input, IntPtr output, fftw_kind kindx, fftw_kind kindy, fftw_flags flags);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_r2r_3d", ExactSpelling = true)]
		public static extern IntPtr r2r_3d(int nx, int ny, int nz, IntPtr input, IntPtr output, fftw_kind kindx, fftw_kind kindy, fftw_kind kindz, fftw_flags flags);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_plan_r2r", ExactSpelling = true)]
		public static extern IntPtr r2r(int rank, int[] n, IntPtr input, IntPtr output, fftw_kind[] kind, fftw_flags flags);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_flops", ExactSpelling = true)]
		public static extern void flops(IntPtr plan, ref double add, ref double mul, ref double fma);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_print_plan", ExactSpelling = true)]
		public static extern void print_plan(IntPtr plan);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_export_wisdom_to_filename", ExactSpelling = true)]
		public static extern void export_wisdom_to_filename(string filename);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "fftwf_import_wisdom_from_filename", ExactSpelling = true)]
		public static extern void import_wisdom_from_filename(string filename);

		[DllImport("libfftw3f-3.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void fftwf_forget_wisdom();
	}
}
