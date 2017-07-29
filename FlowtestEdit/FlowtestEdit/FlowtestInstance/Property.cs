using System.Collections.Generic;
using System.Xml.Linq;

namespace FlowtestEdit.FlowtestInstance
{
    public class Property
    {

        public string name;
        public string spec;

        //public string specdescribe;
        //public string enspecdescribe;
        //public string errcode;
        //public string specprefix;
        //public string specsuffix;
        //public string switch_;
        //public string alarm;
        //public string disable;
        //public string specenable;
        //public string brother;
        //public string timeout;
        //public string editable;
        //public string loop;
        //public string hide;
        //public string condition;
        //public string depend;


        public Property(string name_, string spec_)
        {
            name_= name;
            spec_ = spec;
        }

    }
}
