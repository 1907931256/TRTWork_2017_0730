using System;

namespace FFTWSharp
{
	[Flags]
	public enum fftw_flags : uint
	{
		Measure = 0u,
		DestroyInput = 1u,
		Unaligned = 2u,
		ConserveMemory = 4u,
		Exhaustive = 8u,
		PreserveInput = 16u,
		Patient = 32u,
		Estimate = 64u
	}
}
