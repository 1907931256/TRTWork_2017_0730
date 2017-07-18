using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonPortCmd
{
    public class CmdName
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string loop;

        public string Loop
        {
            get { return loop; }
            set { loop = value; }
        }
        public List<Before> before;
        public List<After> after;

    }

    public class Before
    {
        private string action;

        public string Action
        {
            get { return action; }
            set { action = value; }
        }

        private string res;

        public string Res
        {
            get { return res; }
            set { res = value; }
        }
    }
        public class After
        {
            private string action;

            public string Action
            {
                get { return action; }
                set { action = value; }
            }

            private string res;

            public string Res
            {
                get { return res; }
                set { res = value; }
            }

        }
    
}
