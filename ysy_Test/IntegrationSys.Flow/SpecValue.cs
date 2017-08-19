using System;

namespace IntegrationSys.Flow
{
	internal class SpecValue
	{
        /// <summary>
        /// ����
        /// </summary>
		public string SpecDescription
		{
			get;
			set;
		}

        /// <summary>
        /// ���
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
        /// ʵ��ֵ
        /// ���ֵ 
        /// </summary>
		public string MeasuredValue
		{
			get;
			set;
		}

        /// <summary>
        /// �жϽ��
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
