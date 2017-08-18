using IntegrationSys.Flow;
using IntegrationSys.LogUtil;
using IntegrationSys.Net;
using System;
using System.IO;
using System.Threading;

namespace IntegrationSys.Image
{
	internal class ImageProcessCmd : IExecutable, IDisposable
	{
		private const string ACTION_RESULT = "图像处理结果";

		private Timer timer_;

		private FileSystemWatcher watcher_;

		private static ImageProcessCmd instance_;

		public static ImageProcessCmd Instance
		{
			get
			{
				if (ImageProcessCmd.instance_ == null)
				{
					ImageProcessCmd.instance_ = new ImageProcessCmd();
				}
				return ImageProcessCmd.instance_;
			}
		}

		private ImageProcessCmd()
		{
			try
			{
				this.watcher_ = new FileSystemWatcher("C:\\TRT_Camera_Tester_Picture\\test result", "result.txt");
				this.watcher_.NotifyFilter = (NotifyFilters.FileName | NotifyFilters.LastWrite);
				this.watcher_.Changed += new FileSystemEventHandler(this.OnChanged);
				this.watcher_.EnableRaisingEvents = true;
			}
			catch (Exception)
			{
				Log.Debug("C:\\TRT_Camera_Tester_Picture\\test result\\ not exist");
			}
			this.timer_ = new Timer(new TimerCallback(this.OnWatchedFileChange), null, -1, -1);
		}

		~ImageProcessCmd()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.timer_.Dispose();
				this.watcher_.Dispose();
			}
		}

		public void ExecuteCmd(string action, string param, out string retValue)
		{
			if ("图像处理结果" == action)
			{
				this.ProcessResult(param, out retValue);
				return;
			}
			retValue = "Res=CmdNotSupport";
		}

		private void ProcessResult(string param, out string retValue)
		{
			retValue = string.Empty;
			for (int i = 0; i < 6; i++)
			{
				string path = "d:\\DataFragment\\result_" + i + ".txt";
				try
				{
					using (StreamReader streamReader = new StreamReader(path))
					{
						string text;
						while ((text = streamReader.ReadLine()) != null)
						{
							string text2 = streamReader.ReadLine();
							if (text.IndexOf(param) != -1)
							{
								string[] array = text2.Split(new char[]
								{
									';'
								});
								string[] array2 = array;
								for (int j = 0; j < array2.Length; j++)
								{
									string text3 = array2[j];
									if (!string.IsNullOrEmpty(text3))
									{
										string[] array3 = text3.Split(new char[]
										{
											','
										});
										string text4 = retValue;
										retValue = string.Concat(new string[]
										{
											text4,
											array3[0],
											"=",
											array3[1],
											";"
										});
									}
								}
							}
						}
					}
				}
				catch (FileNotFoundException ex)
				{
					Log.Debug(ex.Message, ex);
				}
				catch (Exception ex2)
				{
					Log.Debug(ex2.Message, ex2);
				}
			}
		}

		private void OnChanged(object source, FileSystemEventArgs e)
		{
			Log.Debug(string.Concat(new object[]
			{
				"File: ",
				e.FullPath,
				" ",
				e.ChangeType
			}));
			this.timer_.Change(500, -1);
		}

		private void OnWatchedFileChange(object state)
		{
			Console.WriteLine("OnWatchedFileChange");
			string srcfilename = "C:\\TRT_Camera_Tester_Picture\\test result\\result.txt";
			string destfilename = "d:\\DataFragment\\result_" + NetUtil.GetStationIndex() + ".txt";
			FileTransferClient fileTransferClient = new FileTransferClient(NetUtil.GetStationIp(4));
			fileTransferClient.Upload(srcfilename, destfilename);
		}
	}
}
