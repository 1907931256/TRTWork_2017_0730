using System;

namespace IntegrationSys.Flow
{
	internal class SpecValue
	{
        /// <summary>
        /// 描述
        /// </summary>
		public string SpecDescription
		{
			get;
			set;
		}

        /// <summary>
        /// 规格
        /// </summary>
		public string Spec
		{
			get;
			set;
		}

		public string SpecKey
		{
			get;
			set;
		}

        /// <summary>
        /// 实测值
        /// 测得值 
        /// </summary>
		public string MeasuredValue
		{
			get;
			set;
		}

        /// <summary>
        /// 判断结果
        /// </summary>
		public string JudgmentResult
		{
			get;
			set;
		}

		public bool Disable
		{
			get;
			set;
		}
	}
}
