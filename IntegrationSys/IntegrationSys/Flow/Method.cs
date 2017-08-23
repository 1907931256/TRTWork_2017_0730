using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegrationSys.Flow
{
    class Method
    {
        //private string name_;
        //private string action_;
        //private string param_;
        //private string regex_;
        //private string spec_;
        //private bool disable_;
        //private bool bedepend_;
        //private bool depend_;

        public string Name
        {
            get;
            set;
        }

        public string Action
        {
            get;
            set;
        }

        public string Param
        {
            get;
            set;
        }

        public string Compare
        {
            get;
            set;
        }

        //public string Spec
        //{
        //    get;
        //    set;
        //}

        public bool Disable
        {
            get;
            set;
        }

        public bool Bedepend
        {
            get;
            set;
        }

        public bool Depend
        {
            get;
            set;
        }
    }
}
