using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;


namespace TRTCamera
{
    class ShujuChuli
    {

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

      
        /// <summary>
        /// 取一个数据的高低八位，以低八位加高八位的形式显示
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StrToGaoDiWei(string str)
        {
            string strGao = ((Convert.ToInt32(str)>>8) & 255).ToString("x2");     //取高八位
            string strDi = (Convert.ToInt32(str) & 0xff).ToString("x2");        
            return strDi + " " + strGao;
        }
        /// <summary>
        /// 接收一组低八位和高八位返回一个字符串
        /// </summary>
        /// <param name="Di">低八位</param>
        /// <param name="Gao">高八位</param>
        /// <returns>拼接成字符串</returns>
        public static string DiGaoBaWei(byte Di, byte Gao)
        {

            string str = Gao.ToString("x2") + Di.ToString("x2");


           int a=  Convert.ToInt32(str, 16);
         
            return a.ToString();
        }

        /// <summary>
        /// 将字符串转换成Hex字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StrToHex(string str)//40000 40000 20 255 255 65535
        {
            string strReturn = "";
            //countPram>0 表示有多个参数存在
           int countPram= Regex.Matches(str, @" ").Count;//利用正则表达式
           string[] strPram = new string[countPram + 1];//创建一个数组保存参数数据
             ArrayList lt = new ArrayList(); //存放空格出现的位置
           if (countPram>0)
           {
               int index = 0;
               foreach (Char ch in str)
               {
                   if (ch == ' ')
                   {
                       lt.Add(index);
                   }
                   index++;
               }


               strPram[0] = str.Substring(0, Convert.ToInt32(lt[0].ToString()));
               for (int i = 1; i < countPram+1; i++)
               {
                   if (i<countPram)
                   {
                       strPram[i] = str.Substring(Convert.ToInt32(lt[i - 1].ToString()) + 1, Convert.ToInt32(lt[i].ToString()) - Convert.ToInt32(lt[i - 1].ToString()) - 1);
                   }
                   else
                   {
                       strPram[i] = str.Substring(Convert.ToInt32(lt[i - 1].ToString()) + 1, str.Length - Convert.ToInt32(lt[i - 1].ToString()) - 1);
                   }
                 
               }
               //for (int i = 0; i < strPram.Length; i++)
               //{
               //  strPram[i]=ShujuChuli.StrToGaoDiWei(strPram[i]);
               //  strReturn += strPram[i]+" ";
               //}
               #region  针对trt机械手做特殊处理
               strPram[0] = ShujuChuli.StrToGaoDiWei(strPram[0]);
               strPram[1] = ShujuChuli.StrToGaoDiWei(strPram[1]);
               strPram[2] = ShujuChuli.StrToHex(strPram[2]);
               strPram[3] = ShujuChuli.StrToHex(strPram[3]);
               strPram[4] = ShujuChuli.StrToHex(strPram[4]);
               strPram[5] = ShujuChuli.StrToGaoDiWei(strPram[5]);
               strReturn = strPram[0] + " " + strPram[1] + " " + strPram[2]+ " " + strPram[3]+ " " + strPram[4]+ " " + strPram[5];
               #endregion

           }
           else
           {
               //strReturn=Convert.ToString(Convert.ToInt32(str), 16);
               strReturn = Convert.ToInt32(str).ToString("X2");
           }
            //可以判断是否有空格，是否是多个参数



       return strReturn;
         
        }
     
        /// <summary>
        /// 接受一个数据以低高位组合起来的数据，返回一个十进制的字符串
        /// </summary>
        /// <param name="strHex"></param>
        /// <returns></returns>
        public static string DiGaoHexStringToString(string strHex)
        {
         
            byte di = Convert.ToByte(strHex.Substring(0, 2),16);
            byte gao = Convert.ToByte(strHex.Substring(2, 2),16);
            string str = DiGaoBaWei(di, gao);
            return str;
        }

        /// <summary>
        /// 16进制字符串转10进制
        /// </summary>
        /// <param name="strHex"></param>
        /// <returns></returns>
        public static string  HexStringToInt(string strHex)
        {
         return Convert.ToInt32(strHex, 16).ToString();
        }


        public static string ByteToHexString(byte[] data)
        {
            string hex = string.Empty;

            for (int i = 0; i < data.Length; i++)
            {
                hex += data[i].ToString("X2");
                hex += " ";
            }
            return hex;
        }


        /// <summary>
        /// 一个INT32数字转换成 4 个 数字Hex
        /// </summary>
        /// <param name="num"></param>
        public static string INT32ToFourString(Int32 num)
        {
            string str=string.Empty;

           byte[] bytes= BitConverter.GetBytes(num);

           string s_hex= BitConverter.ToString(bytes);

           string[] s_hexs = s_hex.Split('-');

           for (int i = 0; i < s_hexs.Length; i++)
           {
               str += s_hexs[s_hexs.Length-1-i] + " ";
           }

           return str;
        }
}
  
}
