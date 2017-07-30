using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace CommonClass
{
    class ShujuChuli
    {



        /// <summary>
        /// 将字符串转换成16进制，以字节数组返回
        /// </summary>
        /// <param name="hs">要转换的字符数组</param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string hs)
        {

            if (hs.Length>1)
            {
                string[] strArr = hs.Trim().Split(' ');
                byte[] b = new byte[strArr.Length];
                //逐个字符变为16进制字节数据
                for (int i = 0; i < strArr.Length; i++)
                {
                    b[i] = Convert.ToByte(strArr[i], 16);
                }
                //按照指定编码将字节数组变为字符串
                return b;
         
            }
            else
            {
                return null;
            }
               
          
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
           }
            if (countPram==5)//机械手处理
	        {
                       #region  针对trt机械手做特殊处理
                       strPram[0] = ShujuChuli.StrToGaoDiWei(strPram[0]);
                       strPram[1] = ShujuChuli.StrToGaoDiWei(strPram[1]);
                       strPram[2] = ShujuChuli.StringToHexString(strPram[2]);
                       strPram[3] = ShujuChuli.StringToHexString(strPram[3]);
                       strPram[4] = ShujuChuli.StringToHexString(strPram[4]);
                       strPram[5] = ShujuChuli.StrToGaoDiWei(strPram[5]);
                       strReturn = strPram[0] + " " + strPram[1] + " " + strPram[2] + " " + strPram[3] + " " + strPram[4] + " " + strPram[5];
                       #endregion
	        }
           else if (countPram==2)
	        {
                strPram[0] = ShujuChuli.StringToHexString(strPram[0]);
                strPram[1] = ShujuChuli.StringToHexString(strPram[1]);
                strPram[2] = ShujuChuli.StringToHexString(strPram[2]);
                strReturn = strPram[0] + " " + strPram[1] + " " + strPram[2];
	        }
           else //参数只有一个
           {
               strReturn = ShujuChuli.StringToHexString(str);
               if (strReturn.Length > 2 && strReturn.Length<5)//参数大于255
               {
                   strReturn =strReturn.Substring(2) +" " + strReturn.Substring(0, 2);
               }
               
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

        /// <summary>
        /// 字节数组转换成字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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
        ///   string 转 HexString
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StringToHexString(string str)
        {
            string strReturn=string.Empty;
            strReturn = Convert.ToString(Convert.ToInt32(str), 16);
            return strReturn;
        }

        /// <summary>
        /// 更具下标获取字符串
        /// 传入字符串用空格隔开
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetIndexString(string data,int index)
        {
            string ret;
            int countPram = Regex.Matches(data, @" ").Count;//利用正则表达式

            ret = data.Substring(index * 3, 2);


            return ret;
        }



}
  
}
