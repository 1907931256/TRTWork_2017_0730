using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegrationSys.Flow
{
    class Property
    {
        public string Name
        {
            get;
            set;
        }

        public string Spec
        {
            get;
            set;
        }

        public string SpecDescription
        {
            get;
            set;
        }

        public string SpecKey
        {
            get;
            set;
        }

        public string ErrorCode
        {
            get;
            set;
        }

        public string SpecPrefix
        {
            get;
            set;
        }

        public string SpecSuffix
        {
            get;
            set;
        }

        /// <summary>
        /// 1：表示此item失败后，后面的item都不再测试。输出失败状态
        /// 2：表示此item失败后，后面的item不再测试。不输出任何状态
        /// </summary>
        public string Switch
        {
            get;
            set;
        }

        /// <summary>
        /// 1:警告,此站停止测试，取消警告后，继续测试
        /// 2:报警，所有站停止测试，人工清仓
        /// </summary>
        public string Alarm
        {
            get;
            set;
        }

        public bool Disable
        {
            get;
            set;
        }

        public string SpecEnable
        {
            get;
            set;
        }

        public string Brother
        {
            get;
            set;
        }

        public int Timeout
        {
            get;
            set;
        }

        public bool Editable
        {
            get;
            set;
        }

        public int Loop
        {
            get;
            set;
        }

        public bool Hide
        {
            get;
            set;
        }

        public string Condition
        {
            get;
            set;
        }

        public string Depend
        {
            get;
            set;
        }
    }
}
