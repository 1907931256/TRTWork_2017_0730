﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PhoneCmdUnit
{
     internal  class RespPair
    {
        public ManualResetEvent RespEvent
		{
			get;
			set;
		}

		public string Resp
		{
			get;
			set;
		}
    }
}
