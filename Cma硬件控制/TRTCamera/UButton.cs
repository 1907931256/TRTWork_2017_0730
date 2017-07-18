using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace TRTCamera
{
    public partial class UButton :Button
    {
        commPortHelp commProtHelp_ = commPortHelp.CreatCommPort();
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            string  strCmd = base.Text;
            string strRec = "";
            commProtHelp_.SendCmd(strCmd, out strRec);

        }

    }

}
