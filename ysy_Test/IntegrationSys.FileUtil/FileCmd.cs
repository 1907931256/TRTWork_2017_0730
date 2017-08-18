using IntegrationSys.Flow;
using IntegrationSys.Net;
using System;
using System.Collections.Generic;
using System.IO;

namespace IntegrationSys.FileUtil
{
	internal class FileCmd : IExecutable
	{
		private delegate void ExecuteMatchCmd(string param, out string retValue);

		private const string ACTION_FILE_COPY = "文件拷贝";

		private const string ACTION_FILE_REMOVE = "文件删除";

		private const string ACTION_FILE_MOVE = "文件剪切";

		private const string ACTION_FILE_TRANSFER = "文件传输";

		private Dictionary<string, FileCmd.ExecuteMatchCmd> cmdDict_;

		private static FileCmd instance_;

		public static FileCmd Instance
		{
			get
			{
				if (FileCmd.instance_ == null)
				{
					FileCmd.instance_ = new FileCmd();
				}
				return FileCmd.instance_;
			}
		}

		private FileCmd()
		{
			this.cmdDict_ = new Dictionary<string, FileCmd.ExecuteMatchCmd>();
			this.cmdDict_.Add("文件拷贝", new FileCmd.ExecuteMatchCmd(this.ExecuteCopy));
			this.cmdDict_.Add("文件删除", new FileCmd.ExecuteMatchCmd(this.ExecuteRemove));
			this.cmdDict_.Add("文件剪切", new FileCmd.ExecuteMatchCmd(this.ExecuteMove));
			this.cmdDict_.Add("文件传输", new FileCmd.ExecuteMatchCmd(this.ExecuteTransfer));
		}

		public void ExecuteCmd(string action, string param, out string retValue)
		{
			if (this.cmdDict_.ContainsKey(action))
			{
				this.cmdDict_[action](param, out retValue);
				return;
			}
			retValue = "Res=CmdNotSupport";
		}

		private void ExecuteCopy(string param, out string retValue)
		{
			if (string.IsNullOrEmpty(param))
			{
				retValue = "Res=ArgumentException";
				return;
			}
			string[] array = param.Split(new char[]
			{
				' '
			});
			if (array.Length == 2)
			{
				File.Copy(array[0], array[1], true);
				retValue = "Res=Pass";
				return;
			}
			retValue = "Res=ArgumentException";
		}

		private void ExecuteRemove(string param, out string retValue)
		{
			if (!string.IsNullOrEmpty(param))
			{
				File.Delete(param);
				retValue = "Res=Pass";
				return;
			}
			retValue = "Res=ArgumentException";
		}

		private void ExecuteMove(string param, out string retValue)
		{
			if (string.IsNullOrEmpty(param))
			{
				retValue = "Res=ArgumentException";
				return;
			}
			string[] array = param.Split(new char[]
			{
				' '
			});
			if (array.Length == 2)
			{
				if (File.Exists(array[1]))
				{
					File.Delete(array[1]);
				}
				File.Move(array[0], array[1]);
				retValue = "Res=Pass";
				return;
			}
			retValue = "Res=ArgumentException";
		}

		private void ExecuteTransfer(string param, out string retValue)
		{
			retValue = "Res=ArgumentException";
			if (!string.IsNullOrEmpty(param))
			{
				string[] array = param.Split(new char[]
				{
					' '
				});
				if (array.Length == 2)
				{
					string[] array2 = array[1].Split(new char[]
					{
						'@'
					});
					if (array2[0].StartsWith("machine"))
					{
						try
						{
							int num = int.Parse(array2[0].Substring("machine".Length));
							FileTransferClient fileTransferClient = new FileTransferClient(NetUtil.GetStationIp(num - 1));
							if (fileTransferClient.Upload(array[0], array2[1]) == 0)
							{
								retValue = "Res=Pass";
							}
							else
							{
								retValue = "Res=Fail";
							}
						}
						catch (Exception)
						{
						}
					}
				}
			}
		}
	}
}
