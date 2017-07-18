using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Net;
using PhoneCmdUnit;
using IniUnit;
using System.Threading;
using System.Threading.Tasks;
using GeneralTst.RS;
using System.Data;

namespace GeneralTst
{
    public partial class General : Form
    {
        public DateTime LocalTime;
        private static string file;
        private static string INIFile = System.Environment.CurrentDirectory + "\\Phone.ini";
        private static int port = 6666;
        private static string host = "127.0.0.1";//服务器端ip地址
        private static string PhoneIp = string.Empty;//保存手机的wifi 地址
        private static string sendCmd = string.Empty;//保存发送的命令

        /// <summary>
        /// 保存日志内容
        /// </summary>
        private string logFile;
        /// <summary>
        /// 日志是否抓取标志
        /// </summary>
        private bool logCatFlag;

        /// <summary>
        /// 指示连接状态
        /// </summary>
        private static bool connectStatus = false;

        /// <summary>
        /// 指示wifi是否处于连接状态
        /// </summary>
        private static bool wifiStatus = false;

        /// <summary>
        /// 指示手机连接状态
        /// </summary>
        private static string devices = string.Empty;

        private static IPAddress ip = IPAddress.Parse(host);
        private static IPEndPoint ipe = new IPEndPoint(ip, port);
        private static PhoneCmd PhoneCmd_ = new PhoneCmd();



        public General()
        {
           
            Control.CheckForIllegalCrossThreadCalls = false;   //禁止.net对线程做检测
            file = System.Environment.CurrentDirectory + "\\GeneralDev.apk";
            InitializeComponent();
           
        }

        //Apk路径加载
        private void LoadApkFile_Click(object sender, EventArgs e)
        {
          

            Stream myStream = null;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = file;
            openFileDialog1.Filter = "apk files (*.apk)|*.apk";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            string strFile = openFileDialog1.FileName;
                            ApkFileShow.Text = strFile;
                            file = strFile;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }


        }

       
        private void General_Load(object sender, EventArgs e)
        {
            usb_radioBtn.Checked = true;
            ApkFileShow.Text = file;
         
           string cmd="Cmd";
           string[] cmdPhone=INIOperationClass.INIGetAllItems(INIFile, cmd);
           cmdComBox.Items.AddRange(cmdPhone);
           cmdComBox.SelectedIndex = 0;
           Thread thread_ = new Thread(delegate()
               {
                   FindDevices(out devices);

               });
           thread_.Start();
      
        }

        private static void FindDevices(out string cmd)
        {
          
            while (true)
            {
                AdbCommand.ExecuteAdbCommand("devices", out cmd);

                if (cmd.Length > 25)
                {
                    cmd = cmd.Substring(24, 16);
                    devices = cmd;
                    //connectStatus = true;
                    break;
                }
                if (connectStatus)
                {
                    break;
                }
            }

        }

 

        /// <summary>
        /// 安装apk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void installBtn_Click(object sender, EventArgs e)
        {
            dispalyTxt.AppendText(System.DateTime.Now + "  " + "开始安装"+ "\r\n");
            AdbCommand.InstallApk(file);
            dispalyTxt.AppendText(System.DateTime.Now + "  " + "安装成功" + "\r\n");
            AdbCommand.InstallApkAndStart();
            dispalyTxt.AppendText(System.DateTime.Now + "  " + "启动成功" + "\r\n");
        }

        /// <summary>
        ///卸载apk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uninstallBtn_Click(object sender, EventArgs e)
        {
            AdbCommand.UninstallApk();
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connect_Click(object sender, EventArgs e)
        {
            if (Wifi_radioBtn.Checked)//wifi连接
            {
               
                if (string.IsNullOrEmpty(PhoneIp))
                {
                    usb_radioBtn.Checked = true;
                    connectStatus = PhoneCmd_.Connect();
                    if (connectStatus)
                    {
                        wifiStatus = true;
                    }
                   
                    MessageBox.Show("请先使用Wifi state命令获取Wifi地址！");
                }
                else
                {
                    usb_radioBtn.Checked = false;
                   connectStatus= PhoneCmd_.Connect(PhoneIp);
                   dispalyTxt.AppendText(System.DateTime.Now + "  " + "连接状态=" + "Wifi" + "\r\n");
                   
                }
               
               
            }
            else
            {
               Wifi_radioBtn.Checked = false;
               connectStatus= PhoneCmd_.Connect();
               dispalyTxt.AppendText(System.DateTime.Now + "  " + "连接状态=" +"Usb" + "\r\n");
            }

           
           
        }


        /// <summary>
        /// 将value 插入到指定数组的指定的位置
        /// </summary>
        /// <param name="a">指定数组</param>
        /// <param name="value">待插入的元素</param>
        /// <param name="index">插入的位置</param>
        /// <returns>插入后的数组</returns>
        public static byte[] InsertNumber(byte[] a, byte value, int index)
        {
            try
            {
                //转换成List<int>集合
                List<byte> list = new List<byte>(a);
                //插入
                list.Insert(index, value);
                //从List<int>集合，再转换成数组
                return list.ToArray();
            }
            catch (Exception e)  // 捕获由插入位置非法而导致的异常
            {
                throw e;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void disconnect_btn_Click(object sender, EventArgs e)
        {
            string str=string.Empty;

            if (Wifi_radioBtn.Checked)
            {
                PhoneCmd_.Disconnect();
                //if ()
                //{
                    
                //}
            }
            else if (usb_radioBtn.Checked)
            {
                PhoneCmd_.Disconnect();
                if (!string.IsNullOrEmpty(PhoneIp))
                {
                    devices = PhoneIp + ":6666";
                }
                
            }


        }


        /// <summary>
        /// 发送按钮按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void cmdSendBtn_Click(object sender, EventArgs e)
        {
            string cmd=cmdComBox.Text;
            string res=string.Empty;
             var dispal = Task.Factory.StartNew(() =>
                {
                    res = CommandStart(cmd);

                    //目前需要禁止对线程访问
                    this.dispalyTxt.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + cmd + "   " + res + "\r\n");
                    //this.txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + strRec + "--->" + builder.ToString() + "\r\n");
                });
            
        }


        private string CommandStart(string sendCmd)
        {
            string str = string.Empty;
            #region
            if (sendCmd.StartsWith("adbcmd"))
            {

                if (sendCmd.IndexOf("openwifiset") != -1)//wifi设置
                {
                    AdbCommand.OpenWifiSettings(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("ctrlGPS") != -1)//Gps界面设置
                {
                    AdbCommand.CtrlGps(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("swipe") != -1)//划屏
                {
                    AdbCommand.Swipe(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("getFocusedActivity") != -1)//包名类名获取
                {
                    AdbCommand.GetFocusedActivity(out str);
                }
                else if (sendCmd.IndexOf("touch") != -1)//点击
                {
                    AdbCommand.TouchScreen(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("getprop") != -1)//信息获取
                {
                    AdbCommand.GetProp(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("snap") != -1)//截屏
                {
                    AdbCommand.ScreenCapture(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("rm") != -1)//删除
                {
                    AdbCommand.Remove(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("pull") != -1)//Pull
                {
                    AdbCommand.Pull(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("push") != -1)//Push
                {
                    AdbCommand.Push(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("key") != -1)//KeyEvent
                {
                    AdbCommand.KeyEvrnt(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("wifi connect") != -1)//wifi 连接
                {
                        MessageBox.Show("请确保手机在Apk开启界面");
                    //AdbCommand.GetFocusedActivity(out str);
                    //if (str.IndexOf("com.qwebob.generaldev/.GeneralDevActivity") != -1)
                    //{
                        connectStatus = PhoneCmd_.Connect(sendCmd.Substring(20, sendCmd.Length - 20));
                        if (connectStatus)
                        {
                            wifiStatus = true;
                        }
                        adbdevices_txt.Text = sendCmd.Substring(20, sendCmd.Length - 20);
                        Wifi_radioBtn.Checked = true;
                        usb_radioBtn.Checked = false;
                    //}
                    //else
                    //{
                        
                    //}

                }
                else if (sendCmd.IndexOf("tcpip") != -1)//KeyEvent
                {
                    AdbCommand.TcpIp(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("connect") != -1)//adb connect
                {
                    AdbCommand.AdbWifiConnect(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("disconnect") != -1)//adb connect
                {
                    AdbCommand.AdbDisconnect(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("start-server") != -1)//adb connect
                {
                    AdbCommand.StartServer(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("kill-server") != -1)//adb connect
                {
                    AdbCommand.KillServer(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("devices") != -1)//adb connect
                {
                    AdbCommand.Devices(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("install") != -1)//adb connect
                {
                    AdbCommand.Install(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("uninstall") != -1)//adb connect
                {
                    AdbCommand.Uninstall(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("get-serialno") != -1)//adb connect
                {
                    AdbCommand.GetSN(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("reboot") != -1)//adb connect
                {
                    AdbCommand.GetSN(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
                else if (sendCmd.IndexOf("logcat") != -1)//adb connect
                {
                    AdbCommand.LogCat(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }



                else//其他指令
                {
                    AdbCommand.ExecuteAdbShellCmd(sendCmd.Substring(7, sendCmd.Length - 7), out str);
                }
            }
            #endregion
            else if (sendCmd.IndexOf("音频操作") != -1)
            {
                if (sendCmd.IndexOf("播放声音") != -1)
                {
                    AudioCmd.Instance.ExecuteCmd("播放声音", sendCmd.Substring(9), out str);
                }
                else if (sendCmd.IndexOf("停止播放") != -1)
                {
                    AudioCmd.Instance.ExecuteCmd("停止播放", sendCmd.Substring(9), out str);
                }
                else if (sendCmd.IndexOf("录音") != -1)
                {
                    AudioCmd.Instance.ExecuteCmd("录音", sendCmd.Substring(7), out str);
                }
                else if (sendCmd.IndexOf("停止录音") != -1)
                {
                    AudioCmd.Instance.ExecuteCmd("停止录音", sendCmd.Substring(9), out str);
                }
                else
                {
                    str = "CmdNotSupport";
                }

            }
            else if (sendCmd.IndexOf("RStechCmd")!=-1)
            {
                string[] strPram = sendCmd.Split(new char[] { ' ' });

                RStechCmd.Instance.ExecuteCmd(strPram[1],sendCmd.Substring(11+strPram[1].Length), out str);
            }
            else
            {
                if (connectStatus)
                {
                    if (sendCmd.IndexOf("push")!=-1)
                    {
                        PhoneCmd_.ExecutePush(sendCmd, out str);
                    }
                    else if (sendCmd.IndexOf("pull") != -1)
                    {
                        PhoneCmd_.ExecutePull(sendCmd, out str);
                    }
                    else if (sendCmd == "Wifi state")
                    {
                        PhoneCmd_.ExecuteSendData(sendCmd, out str);
                        int num_ = str.IndexOf("ip");
                        string ip_ = str.Substring(num_ + 3);
                        PhoneIp = ip_.Remove(ip_.Length - 1);
                       
                    }
                    else
                    {
                        PhoneCmd_.ExecuteSendData(sendCmd, out str);
                    }
                    
                }
                else
                {
                    MessageBox.Show("请先连接手机");
                }

            }
            return str;
        }

        private void start_Apk_Click(object sender, EventArgs e)
        {
            dispalyTxt.AppendText(System.DateTime.Now + "  " + "启动开始" + "\r\n");
            string str = string.Empty;
            bool b= AdbCommand.ExecuteAdbCommand("shell am start -n com.qwebob.generaldev/.GeneralDevActivity", out str);
            dispalyTxt.AppendText(System.DateTime.Now + "  " + "启动"  +b.ToString()+"  " + str + "\r\n");
          
        }

        private void General_Paint(object sender, PaintEventArgs e)
        {
            if (!string.IsNullOrEmpty(devices))
            {
                adbdevices_txt.Text = devices;
            }
            
        }

        private void cmdComBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {
                cmdSendBtn_Click(this, null);
            }
        }

        /// <summary>
        /// 日志抓取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            logCatFlag = true;
            Thread thread_ = new Thread(delegate()
            {

           
                Process process = new Process();
                process.StartInfo.FileName = "adb.exe";
                process.StartInfo.Arguments = "logcat -s qwebob";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;

                process.Start();

                while (logCatFlag)
                {
                    logFile = process.StandardOutput.ReadLine();
                    textBox4.AppendText(logFile + "\r\n");
                }
                

                logFile = textBox4.Text;
                string path = @"e:\" +" APK"+DateTime.Now.ToString("yyyy-MM-dd") + "-"+DateTime.Now.Hour.ToString()+"-"+DateTime.Now.Minute.ToString()+ "-log.txt";
                FileStream fs = new FileStream(path, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(logFile);
                sw.Flush();
                sw.Close();

                process.WaitForExit();
                process.Close();


            });
            thread_.IsBackground = true;
            thread_.Start();
           
        }


        /// <summary>
        /// 停止日志抓取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            logCatFlag = false;
        }
    }
}
