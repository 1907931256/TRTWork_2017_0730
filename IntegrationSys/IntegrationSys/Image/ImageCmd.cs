using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationSys.Flow;
using IntegrationSys.LogUtil;
using System.Drawing;
using System.IO;

namespace IntegrationSys.Image
{
    class ImageCmd : IExecutable
    {
        const string ACTION_ROTATE = "图像旋转";

        private static ImageCmd instance_;

        public static ImageCmd Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new ImageCmd();
                }

                return instance_;
            }
        }

        private ImageCmd() {}

        public void ExecuteCmd(string action, string param, out string retValue)
        {
            if (ACTION_ROTATE == action)
            {
                ExecuteRotate(param, out retValue);
            }
            else
            {
                retValue = "Res=CmdNotSupport";
            }

        }

        private void ExecuteRotate(string param, out string retValue)
        {
            retValue = "Res=Fail";
            try
            {
                System.Drawing.Image image = null;
                using (FileStream fs = File.OpenRead(param))
                {
                    image = System.Drawing.Image.FromStream(fs);
                }

                image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                image.Save(param);
                retValue = "Res=Pass";
            }
            catch (Exception e)
            {
                Log.Debug(e.Message, e);
            }
        }
    }
}
