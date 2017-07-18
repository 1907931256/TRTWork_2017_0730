using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TRTCamera
{
    public class UCheckBox:CheckBox
    {
        commPortHelp commProtHelp_ = commPortHelp.CreatCommPort();
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (base.Checked)
            {
                string str = base.Tag.ToString();
                    // base.Tag.ToString();
               string strRec = "";
               commProtHelp_.SendCmd(str, out strRec);
              
            }
            else
            {
            string str = base.Text;
            string strRec = "";
            commProtHelp_.SendCmd(str, out strRec);
       

            }
        }
    }
}
