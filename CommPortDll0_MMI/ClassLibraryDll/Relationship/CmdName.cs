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
            set { name= value; }
        }
        private string loop;

        public string Loop
        {
            get { return loop; }
            set { loop = value; }
        }
        public List<Action_> action;
    
    }

    public class Action_
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string res;

        public string Res
        {
            get { return res; }
            set { res = value; }
        }



    }
}
