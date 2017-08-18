using IntegrationSys.Flow;
using IntegrationSys.LogUtil;
using System;
using System.Drawing;
using System.IO;

namespace IntegrationSys.Image
{
	internal class ImageCmd 
	{
		private const string ACTION_ROTATE = "图像旋转";

		private static ImageCmd instance_;

		public static ImageCmd Instance
		{
			get
			{
				if (ImageCmd.instance_ == null)
				{
					ImageCmd.instance_ = new ImageCmd();
				}
				return ImageCmd.instance_;
			}
		}

		private ImageCmd()
		{
		}

		//public void ExecuteCmd(string action, string param, out string retValue)
		//{
		//	if ("图像旋转" == action)
		//	{
		//		this.ExecuteRotate(param, out retValue);
		//		return;
		//	}
		//	retValue = "Res=CmdNotSupport";
		//}

		//private void ExecuteRotate(string param, out string retValue)
		//{
		//	retValue = "Res=Fail";
		//	try
		//	{
		//		Image image = null;
		//		using (FileStream fileStream = File.OpenRead(param))
		//		{
		//			image = Image.FromStream(fileStream);
		//		}
		//		image.RotateFlip(RotateFlipType.Rotate90FlipNone);
		//		image.Save(param);
		//		retValue = "Res=Pass";
		//	}
		//	catch (Exception ex)
		//	{
		//		Log.Debug(ex.Message, ex);
		//	}
		//}
	}
}
