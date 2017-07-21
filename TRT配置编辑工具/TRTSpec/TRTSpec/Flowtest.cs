using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TRTSpec
{
    public class Specs
    {
        /// <summary>
        /// 测试项目
        /// </summary>
        private string itme;

        public string Itme
        {
            get { return itme; }
            set { itme = value; }
        }

        /// <summary>
        /// 说明
        /// </summary>
        private string desc;
        public string Desc
        {
            get { return desc; }
            set { desc = value; }
        }

        /// <summary>
        ///规格
        /// </summary>
        private string spec;

        public string Spec
        {
            get { return spec; }
            set { spec = value; }
        }

        /// <summary>
        /// 判断方法
        /// </summary>
        private string compare;

        public string Compare
        {
            get { return compare; }
            set { compare = value; }
        }

      

        private string enspecdescribe;

        public string Enspecdescribe
        {
            get { return enspecdescribe; }
            set { enspecdescribe = value; }
        }
        private string specenable;

        public string Specenable
        {
            get { return specenable; }
            set { specenable = value; }
        }


        public Specs()
        {

        }
        public Specs(string desc_, string space_, string enspecdescribe_, string compare_, string specenable_,string item_)
        {
            
            desc = desc_;
            spec = space_;
            enspecdescribe = enspecdescribe_;
            compare = compare_;
            specenable = specenable_;
            item = item_;
        }

    }

    public class Params
    {

        /// <summary>
        /// 项目组别
        /// </summary>
        private string group;

        public string Group
        {
            get { return group; }
            set { group = value; }
        }


        /// <summary>
        /// 说明
        /// </summary>
        private string desc;

        public string Desc
        {
            get { return desc; }
            set { desc = value; }
        }

        private string parameters;

        public string Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        /// <summary>
        /// =号前面的内容
        /// </summary>
        private string code_name;

        public string Code_name
        {
            get { return code_name; }
            set { code_name = value; }
        }


        public Params()
        {

        }
        public Params(string group_,string desc_,string params_,string codeName_)
        {
            group = group_;
            desc = desc_;
            parameters = params_;
            code_name = codeName_;
        }
    }
}
