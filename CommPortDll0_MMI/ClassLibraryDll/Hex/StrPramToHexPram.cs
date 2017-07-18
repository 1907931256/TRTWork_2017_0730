
namespace CommonPortCmd
{
    class StrPramToHexPram
    {


        /// <summary>
        /// 数据发送指令，映射成hex
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        public static string StrToHex(string pram)
        {
            string hexPram = "";
            if (Common.Model == "MMI")
            {
                switch (pram)
                {
                    #region 6合一MMI
                    #region 1站
                  
                    case "1站产品到位检测": hexPram = "72 05 11 01 01 FF 81";
                        break;
                    case "1站防呆检测": hexPram = "72 05 11 01 02 FF 81";
                        break;
                    case "1站USB拨出检测": hexPram = "72 05 11 01 03 FF 81";
                        break;
                    case "1站耳机检测": hexPram = "72 05 11 01 04 FF 81";
                        break;
                    case "1站后门上到位检测": hexPram = "72 05 11 01 05 FF 81";
                        break;
                    case "1站前门下到位检测": hexPram = "72 05 11 01 06 FF 81";
                        break;
                    case "1站红外感应检测": hexPram = "72 05 11 01 07 FF 81";
                        break;
                    case "1站报警清除检测": hexPram = "72 05 11 01 08 FF 81";
                        break;
                    case "1站电机1原点检测": hexPram = "72 05 11 01 09 FF 81"; //1站电机1原点检测
                        break;
                    case "1站电机1左边检测": hexPram = "72 05 11 01 0a FF 81"; //1站电机1左边检测
                        break;
                    case "1站电机1右边检测": hexPram = "72 05 11 01 0b FF 81"; //1站电机1右边检测
                        break;
                    case "1站上升1检测": hexPram = "72 05 11 01 0c FF 81"; //1站上升1检测
                        break;
                    case "1站下降1检测": hexPram = "72 05 11 01 0d FF 81"; //1站下降1检测
                        break;

                    case "1站吸嘴1检测": hexPram = "72 05 11 01 0e FF 81"; //1站吸嘴1检测
                        break;
                    case "1站电机2原点检测": hexPram = "72 05 11 01 0f FF 81"; //1站电机2原点检测
                        break;
                    case "1站电机2左边检测": hexPram = "72 05 11 01 10 FF 81"; //1站电机2左边检测
                        break;
                    case "1站电机2右边检测": hexPram = "72 05 11 01 11 FF 81"; //1站电机2右边检测
                        break;
                    case "1站上升2检测": hexPram = "72 05 11 01 12 FF 81"; //1站上升2检测
                        break;
                    case "1站下降2检测": hexPram = "72 05 11 01 13 FF 81"; //1站下降2检测
                        break;
                    case "1站吸嘴2检测": hexPram = "72 05 11 01 14 FF 81"; //1站吸嘴2检测
                        break;

                    case "1站电机3原点检测": hexPram = "72 05 11 01 15 FF 81"; //1站电机3原点检测
                        break;
                    case "1站电机3左边检测": hexPram = "72 05 11 01 16 FF 81"; //1站电机3左边检测
                        break;
                    case "1站电机3右边检测": hexPram = "72 05 11 01 17 FF 81"; //1站电机3右边检测
                        break;
                    case "1站上升3检测": hexPram = "72 05 11 01 18 FF 81"; //1站上升3检测
                        break;
                    case "1站下降3检测": hexPram = "72 05 11 01 19 FF 81"; //1站下降3检测
                        break;
                    case "1站吸嘴3检测": hexPram = "72 05 11 01 1a FF 81"; //1站吸嘴3检测
                        break;

                    case "1站电机4原点检测": hexPram = "72 05 11 01 1b FF 81"; //1站电机4原点检测
                        break;
                    case "1站电机4左边检测": hexPram = "72 05 11 01 1c FF 81"; //1站电机4左边检测
                        break;
                    case "1站电机4右边检测": hexPram = "72 05 11 01 1d FF 81"; //1站电机4右边检测
                        break;
                    case "1站上升4检测": hexPram = "72 05 11 01 1e FF 81"; //1站上升4检测
                        break;

                    case "1站下降4检测": hexPram = "72 05 11 01 1f FF 81"; //1站下降4检测
                        break;
                    case "1站吸嘴4检测": hexPram = "72 05 11 01 20 FF 81"; //1站吸嘴4检测
                        break;

                    case "1站电机5原点检测": hexPram = "72 05 11 01 21 FF 81"; //1站电机5原点检测
                        break;
                    case "1站电机5左边检测": hexPram = "72 05 11 01 22 FF 81"; //1站电机5左边检测
                        break;
                    case "1站电机5右边检测": hexPram = "72 05 11 01 23 FF 81"; //1站电机5右边检测
                        break;
                    case "1站上升5检测": hexPram = "72 05 11 01 24 FF 81"; //1站上升5检测
                        break;
                    case "1站下降5检测": hexPram = "72 05 11 01 25 FF 81"; //1站下降5检测
                        break;
                    case "1站吸嘴5检测": hexPram = "72 05 11 01 26 FF 81"; //1站吸嘴5检测
                        break;

                    case "1站电机6原点检测": hexPram = "72 05 11 01 27 FF 81"; //1站电机6原点检测
                        break;
                    case "1站电机6左边检测": hexPram = "72 05 11 01 28 FF 81"; //1站电机6左边检测
                        break;
                    case "1站电机6右边检测": hexPram = "72 05 11 01 29 FF 81"; //1站电机6右边检测
                        break;
                    case "1站上升6检测": hexPram = "72 05 11 01 2a FF 81"; //1站上升6检测
                        break;
                    case "1站下降6检测": hexPram = "72 05 11 01 2b FF 81"; //1站下降6检测
                        break;
                    case "1站吸嘴6检测": hexPram = "72 05 11 01 2c FF 81"; //1站吸嘴6检测
                        break;

                    case "1站电机7原点检测": hexPram = "72 05 11 01 2d FF 81"; //1站电机7原点检测
                        break;
                    case "1站电机7左边检测": hexPram = "72 05 11 01 2e FF 81"; //1站电机7左边检测
                        break;
                    case "1站电机7右边检测": hexPram = "72 05 11 01 2f FF 81"; //1站电机7右边检测
                        break;
                    case "1站上升7检测": hexPram = "72 05 11 01 30 FF 81"; //1站上升7检测
                        break;
                    case "1站下降7检测": hexPram = "72 05 11 01 31 FF 81"; //1站下降7检测
                        break;
                    case "1站吸嘴7检测": hexPram = "72 05 11 01 32 FF 81"; //1站吸嘴7检测
                        break;
                    case "1站电机8原点检测": hexPram = "72 05 11 01 33 FF 81"; //1站电机8原点检测
                        break;

                    case "1站电机8左边检测": hexPram = "72 05 11 01 34 FF 81"; //1站电机8左边检测
                        break;
                    case "1站电机8右边检测": hexPram = "72 05 11 01 35 FF 81"; //1站电机8右边检测
                        break;
                    case "1站上升8检测": hexPram = "72 05 11 01 36 FF 81"; //1站上升8检测
                        break;
                    case "1站下降8检测": hexPram = "72 05 11 01 37 FF 81"; //1站下降8检测
                        break;
                    case "1站吸嘴8检测": hexPram = "72 05 11 01 38 FF 81"; //1站吸嘴8检测
                        break;
                    case "1站报警1": hexPram = "72 05 11 01 38 FF 81";
                        break;
                    case "1站状态1": hexPram = "72 05 11 01 38 FF 81";
                        break;
                    case "1站45站靠近检测": hexPram = "72 05 11 01 39 FF 81";
                        break;
                     case "1站45站远离检测": hexPram = "72 05 11 01 3A FF 81";
                        break;
                    case "1站56站靠近检测": hexPram = "72 05 11 01 3B FF 81";
                        break;
                     case "1站56站远离检测": hexPram = "72 05 11 01 3c FF 81";
                        break;
                   
                    #endregion
                   


                    #region  1站气缸
                    case "1站手机固定": hexPram = "72 05 11 02 01 00 81"; //1站手机固定
                        break;
                    case "1站手机松开": hexPram = "72 05 11 02 01 FF 81"; //1站手机松开
                        break;
                    case "1站USB插入": hexPram = "72 05 11 02 02 00 81"; //1站USB插入
                        break;
                    case "1站USB拔出": hexPram = "72 05 11 02 02 FF 81"; //1站USB拔出
                        break;
                    case "1站耳机插入": hexPram = "72 05 11 02 03 00 81"; //1站耳机插入
                        break;
                    case "1站耳机拔出": hexPram = "72 05 11 02 03 FF 81"; //1站耳机拔出
                        break;
                    case "1站N磁靠近": hexPram = "72 05 11 02 04 00 81"; //1站N磁靠近
                        break;
                    case "1站N磁远离": hexPram = "72 05 11 02 04 FF 81"; //1站N磁远离
                        break;
                    case "1站S磁靠近": hexPram = "72 05 11 02 05 00 81"; //1站S磁靠近
                        break;
                    case "1站S磁远离": hexPram = "72 05 11 02 05 FF 81"; //1站S磁远离
                        break;
                    case "1站前后门开门": hexPram = "72 05 11 02 06 00 81"; //1站前门开门
                        break;
                    case "1站前后门关门": hexPram = "72 05 11 02 06 FF 81"; //1站前门关门
                        break;
                    //case "1站后门开门": hexPram = "72 05 11 02 07 00 81"; //1站后门开门
                    //    break;
                    //case "1站后门关门": hexPram = "72 05 11 02 07 FF 81"; //1站后门关门
                    //    break;
                    case "1站升降气缸1升起": hexPram = "72 05 11 02 08 00 81"; //1站升降气缸1升起
                        break;
                    case "1站升降气缸1降下": hexPram = "72 05 11 02 08 ff 81"; //1站升降气缸1降下
                        break;
                    case "1站取放吸嘴1吸取": hexPram = "72 05 11 02 09 00 81"; //1站取放吸嘴1吸取
                        break;
                    case "1站取放吸嘴1放气": hexPram = "72 05 11 02 09 FF 81"; //1站取放吸嘴1放下
                        break;

                    case "1站升降气缸2升起": hexPram = "72 05 11 02 0a 00 81"; //1站升降气缸2升起
                        break;
                    case "1站升降气缸2降下": hexPram = "72 05 11 02 0a FF 81"; //1站取放吸嘴2降下
                        break;
                    case "1站取放吸嘴2吸取": hexPram = "72 05 11 02 0b 00 81"; //1站取放吸嘴2吸取
                        break;
                    case "1站取放吸嘴2放气": hexPram = "72 05 11 02 0b FF 81"; //1站取放吸嘴2放下
                        break;

                    case "1站升降气缸3升起": hexPram = "72 05 11 02 0c 00 81"; //1站升降气缸3升起
                        break;
                    case "1站升降气缸3降下": hexPram = "72 05 11 02 0c FF 81"; //1站取放吸嘴3降下
                        break;
                    case "1站取放吸嘴3吸取": hexPram = "72 05 11 02 0d 00 81"; //1站取放吸嘴3吸取
                        break;
                    case "1站取放吸嘴3放气": hexPram = "72 05 11 02 0d FF 81"; //1站取放吸嘴3放下
                        break;

                    case "1站升降气缸4升起": hexPram = "72 05 11 02 0e 00 81"; //1站升降气缸4升起
                        break;
                    case "1站升降气缸4降下": hexPram = "72 05 11 02 0e FF 81"; //1站取放吸嘴4降下
                        break;
                    case "1站取放吸嘴4吸取": hexPram = "72 05 11 02 0f 00 81"; //1站取放吸嘴4吸取
                        break;
                    case "1站取放吸嘴4放气": hexPram = "72 05 11 02 0f FF 81"; //1站取放吸嘴4放下
                        break;

                    case "1站升降气缸5升起": hexPram = "72 05 11 02 10 00 81"; //1站升降气缸5升起
                        break;
                    case "1站升降气缸5降下": hexPram = "72 05 11 02 10 FF 81"; //1站取放吸嘴5降下
                        break;
                    case "1站取放吸嘴5吸取": hexPram = "72 05 11 02 11 00 81"; //1站取放吸嘴5吸取
                        break;
                    case "1站取放吸嘴5放气": hexPram = "72 05 11 02 11 FF 81"; //1站取放吸嘴5放下
                        break;

                    case "1站升降气缸6升起": hexPram = "72 05 11 02 12 00 81"; //1站升降气缸6升起
                        break;
                    case "1站升降气缸6降下": hexPram = "72 05 11 02 12 FF 81"; //1站取放吸嘴6降下
                        break;
                    case "1站取放吸嘴6吸取": hexPram = "72 05 11 02 13 00 81"; //1站取放吸嘴6吸取
                        break;
                    case "1站取放吸嘴6放气": hexPram = "72 05 11 02 13 FF 81"; //1站取放吸嘴6放下
                        break;

                    case "1站升降气缸7升起": hexPram = "72 05 11 02 14 00 81"; //1站升降气缸7升起
                        break;
                    case "1站升降气缸7降下": hexPram = "72 05 11 02 14 FF 81"; //1站取放吸嘴7降下
                        break;
                    case "1站取放吸嘴7吸取": hexPram = "72 05 11 02 15 00 81"; //1站取放吸嘴7吸取
                        break;
                    case "1站取放吸嘴7放气": hexPram = "72 05 11 02 15 FF 81"; //1站取放吸嘴7放下
                        break;

                    case "1站升降气缸8升起": hexPram = "72 05 11 02 16 00 81"; //1站升降气缸8升起
                        break;
                    case "1站升降气缸8降下": hexPram = "72 05 11 02 16 FF 81"; //1站取放吸嘴8降下
                        break;
                    case "1站取放吸嘴8吸取": hexPram = "72 05 11 02 17 00 81"; //1站取放吸嘴8吸取
                        break;
                    case "1站取放吸嘴8放气": hexPram = "72 05 11 02 17 FF 81"; //1站取放吸嘴8放下
                        break;

                    //case "1站报警": hexPram = "72 05 11 02 18 00 81"; //1站取放吸嘴8吸取
                    //    break;
                  
                    case "1站45站靠近": hexPram = "72 05 11 02 19 00 81";
                        break;
                    case "1站45站远离": hexPram = "72 05 11 02 19 FF 81";
                        break;
                    case "1站56站靠近": hexPram = "72 05 11 02 1A 00 81";
                        break;
                    case "1站56站远离": hexPram = "72 05 11 02 1A FF 81";
                        break;
                    #endregion


                    #region  04模块

                    case "1站电机左运动": hexPram = "72 05 11 04 08"; //1站取放吸嘴8放下
                        break;
                    case "1站电机右运动": hexPram = "72 05 11 04 09"; //1站取放吸嘴8放下
                        break;
                    case "1站电机中运动": hexPram = "72 05 11 04 0a"; //1站取放吸嘴8放下
                        break;
                    case "1站电机原点": hexPram = "72 05 11 04 0b FF 81"; //1站取放吸嘴8放下
                        break;
                    //case "1站取放": hexPram = "72 05 11 04 0C"; //1站取放吸嘴8放下
                    //    break;

                    #endregion




                    #region 耳机
                    case "1站EAR-L": hexPram = "72 05 11 03 01 00 81"; //1站EAR-L
                        break;
                    case "1站EAR-R": hexPram = "72 05 11 03 01 FF 81"; //1站EAR-R
                        break;
                    case "1站EAR-MIC打开": hexPram = "72 05 11 03 02 00 81"; //1站EAR-MIC打开
                        break;
                    case "1站EAR-MIC关闭": hexPram = "72 05 11 03 02 FF 81"; //1站EAR-MIC关闭
                        break;
                    case "1站EAR-VOL+按下": hexPram = "72 05 11 03 03 00 81"; //1站EAR-VOL+按下
                        break;
                    case "1站EAR-VOL+松开": hexPram = "72 05 11 03 03 FF 81"; //1站EAR-VOL+按下
                        break;
                    case "1站EAR-VOL-按下": hexPram = "72 05 11 03 04 00 81"; //1站EAR-VOL-按下
                        break;
                    case "1站EAR-VOL-松开": hexPram = "72 05 11 03 04 FF 81"; //1站EAR-VOL-按下
                        break;
                    case "1站HOOK按下": hexPram = "72 05 11 03 05 00 81"; //1站HOOK按下
                        break;
                    case "1站HOOK弹起": hexPram = "72 05 11 03 05 FF 81"; //1站HOOK弹起
                        break;
                    case "1站国标耳机": hexPram = "72 05 11 03 05 00 81"; //1站国标耳机
                        break;
                    case "1站美标耳机": hexPram = "72 05 11 03 05 FF 81"; //1站美标耳机
                        break;
                    #endregion

                    #region 05模块
                    case "1站原点信号正常": hexPram = "72 05 11 05 01 00 81";
                        break;
                    case "1站原点信号异常": hexPram = "72 05 11 05 01 FF 81";
                        break;
                    case "1站状态正常": hexPram = "72 05 11 05 02 00 81";
                        break;
                    case "1站状态异常": hexPram = "72 05 11 05 02 ff 81";
                        break;
                    case "1站指示灯绿": hexPram = "72 05 11 05 03 00 81";
                        break;
                    case "1站指示灯红": hexPram = "72 05 11 05 03 ff 81";
                        break;
                    case "1站指示灯黄": hexPram = "72 05 11 05 03 aa 81";
                        break;
                    #endregion
                    case "1站测试中": hexPram = "72 05 11 07 01 00 81";
                        break;
                    case "1站测试完成": hexPram = "72 05 11 07 01 ff 81";
                        break;
                    case "1站复位": hexPram = "72 04 11 5a ff 81";
                        break;

                    #endregion

                    #region 2站
                    case "2站产品到位检测": hexPram = "72 05 12 01 01 FF 81";
                        break;
                    case "2站防抖电机X原点检测": hexPram = "72 05 12 01 02 FF 81";
                        break;
                    case "2站防抖电机Y原点检测": hexPram = "72 05 12 01 03 FF 81";
                        break;
                    case "2站灯箱远离检测": hexPram = "72 05 12 01 04 FF 81";
                        break;
                    case "2站增距远离检测": hexPram = "72 05 12 01 05 FF 81";
                        break;
                    case "2站近焦远离检测": hexPram = "72 05 12 01 06 FF 81";
                        break;
                    case "2站解析远离检测": hexPram = "72 05 12 01 07 FF 81";
                        break;
                    case "2站色卡远离检测": hexPram = "72 05 12 01 08 FF 81";
                        break;
                    case "2站近焦上升到位检测": hexPram = "72 05 12 01 09 FF 81";
                        break;
                    case "1站原点检测2": hexPram = "72 05 12 01 0a FF 81";
                        break;
                    case "1站状态检测2": hexPram = "72 05 12 01 0b FF 81";
                        break;
                    //case "2站手机固定": hexPram = "72 05 12 02 01 00 81";
                    //    break;
                    case "2站手机松开": hexPram = "72 05 12 02 01 FF 81";
                        break;
                    case "2站校准光源靠近": hexPram = "72 05 12 02 02 00 81";
                        break;
                    case "2站校准光源远离": hexPram = "72 05 12 02 02 FF 81";
                        break;
                    case "2站增距镜靠近": hexPram = "72 05 12 02 03 00 81";
                        break;
                    case "2站增距镜远离": hexPram = "72 05 12 02 03 FF 81";
                        break;

                    case "2站近焦靠近": hexPram = "72 05 12 02 04 00 81";
                        break;
                    case "2站近焦远离": hexPram = "72 05 12 02 04 FF 81";
                        break;
                    case "2站近焦上升": hexPram = "72 05 12 02 05 00 81";
                        break;
                    case "2站近焦下降": hexPram = "72 05 12 02 05 FF 81";
                        break;

                    case "2站解析靠近": hexPram = "72 05 12 02 06 00 81";
                        break;
                    case "2站解析远离": hexPram = "72 05 12 02 06 FF 81";
                        break;
                    case "2站色卡靠近": hexPram = "72 05 12 02 07 00 81";
                        break;
                    case "2站色卡远离": hexPram = "72 05 12 02 07 FF 81";
                        break;
                    case "2站色卡上光源调节": hexPram = "72 05 12 03 01";
                        break;

                    case "2站色卡中光源调节": hexPram = "72 05 12 03 02";
                        break;

                    case "2站色卡下光源调节": hexPram = "72 05 12 03 03";
                        break;

                    case "2站校准光源调节": hexPram = "72 05 12 03 04";
                        break;

                    case "2站近距内光源调节": hexPram = "72 05 12 03 05";
                        break;


                    case "2站近距外光源调节": hexPram = "72 05 12 03 06";
                        break;
                 
                    case "2站OIS防抖1": hexPram = "72 05 12 04 01";
                        break;
                    case "2站OIS防抖2": hexPram = "72 05 12 04 02";
                        break;

                    case "2站电机电源": hexPram = "72 05 12 04 03 81";
                        break;
                    case "2站报警": hexPram = "72 05 12 05 01 00 81";
                        break;
                    case "2站报警正常": hexPram = "72 05 12 05 01 FF 81";
                        break;
                    case "2站状态正常": hexPram = "72 05 12 05 02 00 81";
                        break;
                    case "2站状态异常": hexPram = "72 05 12 05 02 FF 81";
                        break;
                    case "2站状态指示灯绿": hexPram = "72 05 12 05 03 00 81";
                        break;
                    case "2站状态指示灯红": hexPram = "72 05 12 05 03 FF 81";
                        break;
                    case "2站状态指示灯黄": hexPram = "72 05 12 05 03 AA 81";
                        break;
                    case "握手指令": hexPram = "72 04 12 0f 00 81";
                        break;
                    case "2站复位": hexPram = "72 04 12 5a ff 81";
                        break;
                    case "2站测试状态测试中": hexPram = "72 05 12 06 01 00 81";
                        break;
                    case "2站测试状态测试状态": hexPram = "72 05 12 06 01 FF 81";
                        break;

                    #endregion

                    #region 3站

                    case "3站产品到位检测": hexPram = "72 05 13 01 01 ff 81";
                        break;
                    case "3站解析远离检测": hexPram = "72 05 13 01 02 FF 81";
                        break;
                    case "3站45度放平检测": hexPram = "72 05 13 01 03 FF 81";
                        break;
                    case "3站45度顶起检测": hexPram = "72 05 13 01 04 FF 81";
                        break;
                    case "3站前白卡靠近检测": hexPram = "72 05 13 01 05 FF 81";
                        break;
                    case "3站前白卡远离检测": hexPram = "72 05 13 01 06 FF 81";
                        break;
                    case "3站前白卡上升检测": hexPram = "72 05 13 01 07 FF 81";
                        break;
                    case "3站前白卡下降检测": hexPram = "72 05 13 01 08 FF 81";
                        break;
                    case "1站原点检测3": hexPram = "72 05 13 01 09 FF 81";
                        break;
                    case "1站状态检测3": hexPram = "72 05 13 01 0a FF 81";
                        break;
                    case "报警清除检测": hexPram = "72 05 13 01 0b FF 81";
                        break;
                    case "3站手机固定": hexPram = "72 05 13 02 01 00 81";
                        break;
                    case "3站手机松开": hexPram = "72 05 13 02 01 FF 81";
                        break;
                    //case "3站45度顶起": hexPram = "72 05 13 02 02 00 81";
                    //    break;
                    case "3站45度放平": hexPram = "72 05 13 02 02 FF 81";
                        break;
                    case "3站前白卡靠近": hexPram = "72 05 13 02 03 00 81";
                        break;
                    case "3站前白卡远离": hexPram = "72 05 13 02 03 FF 81";
                        break;
                    case "3站解析靠近": hexPram = "72 05 13 02 04 00 81";
                        break;
                    case "3站解析远离": hexPram = "72 05 13 02 04 FF 81";
                        break;
                    //case "3站前白卡下降": hexPram = "72 05 13 02 05 00 81";
                    //    break;
                    case "3站前白卡上升": hexPram = "72 05 13 02 05 FF 81";
                        break;
                    case "3站取放上升": hexPram = "72 05 13 02 06 00 81";
                        break;
                    case "3站取放下降": hexPram = "72 05 13 02 06 FF 81";
                        break;
                    case "3站夹具固定": hexPram = "72 05 13 02 07 00 81";
                        break;
                    case "3站夹具松开": hexPram = "72 05 13 02 07 FF 81";
                        break;
                    #region   光源调节
                    case "3站前色卡上光源调节": hexPram = "72 05 13 03 01";
                        break;
                    case "3站前色卡中光源调节": hexPram = "72 05 13 03 02";
                        break;
                    case "3站前色卡下光源调节": hexPram = "72 05 13 03 03";
                        break;
                    case "3站前白光源调节": hexPram = "72 05 13 03 04";
                        break;
                    case "3站后白光源调节": hexPram = "72 05 13 03 05";
                        break;
                    #endregion
                    case "3站报警": hexPram = "72 05 13 05 01 00 81";
                        break;
                    case "3站报警正常": hexPram = "72 05 13 05 01 FF 81";
                        break;
                    case "3站状态正常": hexPram = "72 05 13 05 02 00 81";
                        break;
                    case "3站状态异常": hexPram = "72 05 13 05 02 FF 81";
                        break;
                    case "3站状态指示灯绿": hexPram = "72 05 13 05 03 00 81";
                        break;
                    case "3站状态指示灯红": hexPram = "72 05 13 05 03 FF 81";
                        break;
                    case "3站状态指示灯黄": hexPram = "72 05 13 05 03 AA 81";
                        break;
                    case "3站测试状态测试中": hexPram = "72 05 13 06 01 00 81";
                        break;
                    case "3站测试状态测试完成": hexPram = "72 05 13 06 01 FF 81";
                        break;
                    case "3站复位": hexPram = "72 04 13 5a ff 81";
                        break;
                    #endregion

                    #region 4站
                    case "4站产品到位检测": hexPram = "72 05 14 01 01 FF 81";
                        break;
                    case "4站天板远离检测": hexPram = "72 05 14 01 02 FF 81";
                        break;
                    case "4站天板靠近检测": hexPram = "72 05 14 01 03 FF 81";
                        break;
                    case "1站原点检测4": hexPram = "72 05 14 01 04 FF 81";
                        break;
                    case "1站状态检测4": hexPram = "72 05 14 01 05 FF 81";
                        break;
                    case "4报警清除检测": hexPram = "72 05 14 01 06 FF 81";
                        break;

                    case "4站手机固定": hexPram = "72 05 14 02 01 00 81";
                        break;
                    case "4站手机松开": hexPram = "72 05 14 02 01 FF 81";
                        break;
                    case "4站距离1遮挡": hexPram = "72 05 14 02 02 00 81";
                        break;
                    case "4站距离1远离": hexPram = "72 05 14 02 02 FF 81";
                        break;
                    case "4站距离2遮挡": hexPram = "72 05 14 02 03 00 81";
                        break;
                    case "4站距离2远离": hexPram = "72 05 14 02 03 FF 81";
                        break;
                    //case "4站天板靠近": hexPram = "72 05 14 02 04 00 81";
                        //break;
                    case "4站天板远离": hexPram = "72 05 14 02 04 FF 81";
                        break;
                    case "4站NFC靠近": hexPram = "72 05 14 02 05 00 81";
                        break;
                    case "4站NFC远离": hexPram = "72 05 14 02 05 FF 81";
                        break;
                    case "4站返回靠近": hexPram = "72 05 14 02 06 00 81";
                        break;
                    case "4站返回远离": hexPram = "72 05 14 02 06 FF 81";
                        break;
                    case "4站主页靠近": hexPram = "72 05 14 02 07 00 81";
                        break;
                    case "4站主页远离": hexPram = "72 05 14 02 07 FF 81";
                        break;
                    case "4站菜单靠近": hexPram = "72 05 14 02 08 00 81";
                        break;
                    case "4站菜单远离": hexPram = "72 05 14 02 08 FF 81";
                        break;
                    case "4站指纹靠近": hexPram = "72 05 14 02 09 00 81";
                        break;
                    case "4站指纹远离": hexPram = "72 05 14 02 09 FF 81";
                        break;
                    case "4站后白靠近": hexPram = "72 05 14 02 0a 00 81";
                        break;
                    case "4站后白远离": hexPram = "72 05 14 02 0a FF 81";
                        break;
                    case "4站后白光源调节": hexPram = "72 05 14 03 01";
                        break;

                    case "4站备用光源调节": hexPram = "72 05 14 03 02";
                        break;
                   
                    case "4站前闪光灯检测开始": hexPram = "72 05 14 04 01 FF 81";
                        break;
                    case "4站前闪光灯数据读取": hexPram = "72 05 14 04 01 00 81";
                        break;
                    case "4站LCD背光检测开始": hexPram = "72 05 14 04 02 FF 81";
                        break;
                    case "4站LCD背光数据读取": hexPram = "72 05 14 04 02 00 81";
                        break;
                    case "4站后闪光灯检测开始": hexPram = "72 05 14 04 03 FF 81";
                        break;
                    case "4站后闪光灯数据读取": hexPram = "72 05 14 04 03 00 81";
                        break;
                    case "4站信号灯检测开始": hexPram = "72 05 14 04 04 FF 81";
                        break;
                    case "4站信号灯数据读取": hexPram = "72 05 14 04 04 00 81";
                        break;
                    case "4站报警": hexPram = "72 05 14 05 01 00 81";
                        break;
                    case "4站报警正常": hexPram = "72 05 14 05 01 FF 81";
                        break;
                    case "4站状态正常": hexPram = "72 05 14 05 02 00 81";
                        break;
                    case "4站状态异常": hexPram = "72 05 14 05 02 FF 81";
                        break;
                    case "4站状态指示绿灯": hexPram = "72 05 14 05 03 00 81";
                        break;
                    case "4站状态指示黄灯": hexPram = "72 05 14 05 03 FF 81";
                        break;
                    case "4站状态指示红灯": hexPram = "72 05 14 05 03 AA 81";
                        break;

                    case "4站复位": hexPram = "72 04 14 5a ff 81";
                        break;
                    case "4站测试状态测试中": hexPram = "72 05 14 06 01 00 81";
                        break;
                    case "4站测试状态测试完成": hexPram = "72 05 14 06 01 FF 81";
                        break;
                    #endregion

                    #region 5站
                    case "5站产品到位检测": hexPram = "72 05 15 01 01 ff 81";
                        break;
                    case "5站人工嘴下降检测": hexPram = "72 05 15 01 02 ff 81";
                        break;
                    case "5站人工耳远离检测": hexPram = "72 05 15 01 03 ff 81";
                        break;
                    case "5站隔离上升检测": hexPram = "72 05 15 01 04 ff 81";
                        break;

                    case "5站托板下降检测": hexPram = "72 05 15 01 05 FF 81";
                        break;
                    case "5站隔离关闭检测": hexPram = "72 05 15 01 06 FF 81";
                        break;
                    case "1站原点检测5": hexPram = "72 05 15 01 07 FF 81";
                        break;
                    case "1站状态检测5": hexPram = "72 05 15 01 08 FF 81";
                        break;
                    case "报警清除5": hexPram = "72 05 15 01 09 FF 81";
                        break;



                    case "5站托板上升": hexPram = "72 05 15 02 01 00 81";
                        break;
                    case "5站托板下降": hexPram = "72 05 15 02 01 FF 81";
                        break;
                    case "5站USB插入": hexPram = "72 05 15 02 02 00 81";
                        break;
                    case "5站USB拔出": hexPram = "72 05 15 02 02 FF 81";
                        break;
                    case "5站隔音箱上升": hexPram = "72 05 15 02 03 00 81";
                        break;
                    //case "5站隔音箱下降": hexPram = "72 05 15 02 03 FF 81";
                    //    break;
                    case "5站人工嘴上升": hexPram = "72 05 15 02 04 00 81";
                        break;
                    case "5站人工嘴下降": hexPram = "72 05 15 02 04 FF 81";
                        break;
                    case "5站人工嘴左移": hexPram = "72 05 15 02 05 00 81";
                        break;
                    case "5站人工嘴右移": hexPram = "72 05 15 02 05 FF 81";
                        break;
                    case "5站人工耳靠近": hexPram = "72 05 15 02 06 00 81";
                        break;
                    case "5站人工耳远离": hexPram = "72 05 15 02 06 FF 81";
                        break;
                    case "5站耳机插入": hexPram = "72 05 15 02 07 00 81";
                        break;
                    case "5站耳机拔出": hexPram = "72 05 15 02 07 FF 81";
                        break;
                    case "5站报警": hexPram = "72 05 15 05 01 00 81";
                        break;
                    case "5站报警正常": hexPram = "72 05 15 05 01 FF 81";
                        break;
                    case "5站状态正常": hexPram = "72 05 15 05 02 00 81";
                        break;
                    case "5站状态异常": hexPram = "72 05 15 05 02 FF 81";
                        break;
                    case "5站状态指示绿灯": hexPram = "72 05 15 05 03 00 81";
                        break;
                    case "5站状态指示红灯": hexPram = "72 05 15 05 03 FF 81";
                        break;
                    case "5站状态指示黄灯": hexPram = "72 05 15 05 03 AA 81";
                        break;

                    case "5站复位": hexPram = "72 04 15 5a FF 81";
                        break;
                    case "5站测试状态测试中": hexPram = "72 05 15 06 01 00 81";
                        break;
                    case "5站测试状态测试完成": hexPram = "72 05 15 06 01 FF 81";

                        break;
                    case "5站振动打开": hexPram = "72 05 15 02 08 00 81";
                        break;
                    case "5站振动关闭": hexPram = "72 05 15 02 08 FF 81";
                        break;

                    case "5站听筒打开": hexPram = "72 05 15 02 09 00 81";
                        break;
                    case "5站听筒关闭": hexPram = "72 05 15 02 09 FF 81";
                        break;
                    case "5站喇叭打开": hexPram = "72 05 15 02 0A 00 81";
                        break;
                    case "5站喇叭关闭": hexPram = "72 05 15 02 0A FF 81";
                        break;
                    case "5站备用输入打开": hexPram = "72 05 15 02 0B 00 81";
                        break;
                    case "5站备用输入关闭": hexPram = "72 05 15 02 0B FF 81";
                        break;
                    case "5站MIC打开": hexPram = "72 05 15 02 0C 00 81";
                        break;
                    case "5站MIC关闭": hexPram = "72 05 15 02 0C FF 81";
                        break;
                    case "5站MIC1打开": hexPram = "72 05 15 02 0D 00 81";
                        break;
                    case "5站MIC1关闭": hexPram = "72 05 15 02 0D FF 81";
                        break;
                    case "5站备用输出打开": hexPram = "72 05 15 02 0E 00 81";
                        break;
                    case "5站备用输出关闭": hexPram = "72 05 15 02 0E FF 81";
                        break;



                    #endregion

                    #region 6站
                    case "6站产品到位检测": hexPram = "72 05 16 01 01 FF 81"; break;

                    case "6站电机X位置检测": hexPram = "72 05 16 01 02 FF 81"; break;

                    case "6站电机Y位置检测": hexPram = "72 05 16 01 03 FF 81"; break;

                    case "6站电机Z位置检测": hexPram = "72 05 16 01 04 FF 81"; break;

                    case "6站USB拔出检测": hexPram = "72 05 16 01 05 FF 81"; break;

                    case "6站耳机拔出检测": hexPram = "72 05 16 01 06 FF 81"; break;

                    case "6站取放下压到位检测": hexPram = "72 05 16 01 07 FF 81"; break;

                    case "6站取放抬起到位检测": hexPram = "72 05 16 01 08 FF 81"; break;

                    case "6站TP抬起到位检测": hexPram = "72 05 16 01 09 FF 81"; break;

                    case "6站吸合状态检查": hexPram = "72 05 16 01 0a FF 81"; break;

                    case "6站取放到位检测": hexPram = "72 05 16 01 0b FF 81"; break;

                    case "1站原点检测6": hexPram = "72 05 16 01 0c FF 81"; break;

                    case "1站状态检测6": hexPram = "72 05 16 01 0d FF 81"; break;

                    case "报警清除检测6": hexPram = "72 05 16 01 0e FF 81"; break;

                    case "6站取放中间到位检测": hexPram = "72 05 16 01 0f FF 81"; break;

                    case "6站TP按下到位检测": hexPram = "72 05 16 01 10 FF 81"; break;

                    case "6站手机固定": hexPram = "72 05 16 02 01 00 81"; break;

                    case "6站手机松开": hexPram = "72 05 16 02 01 FF 81"; break;

                    case "6站音量-按下": hexPram = "72 05 16 02 02 00 81"; break;

                    case "6站音量-弹起": hexPram = "72 05 16 02 02 FF 81"; break;

                    case "6站音量+按下": hexPram = "72 05 16 02 03 00 81"; break;

                    case "6站音量+弹起": hexPram = "72 05 16 02 03 FF 81"; break;

                    case "6站TP按下": hexPram = "72 05 16 02 04 00 81"; break;

                    case "6站TP弹起": hexPram = "72 05 16 02 04 FF 81"; break;

                    case "6站USB插入": hexPram = "72 05 16 02 05 00 81"; break;

                    case "6站USB拔出": hexPram = "72 05 16 02 05 FF 81"; break;

                    case "6站吸盘吸合": hexPram = "72 05 16 02 06 00 81"; break;
                     
                    case "6站吸盘松开": hexPram = "72 05 16 02 06 FF 81";
                        break;
                    case "6站取放靠近": hexPram = "72 05 16 02 07 00 81";
                        break;
                    case "6站取放远离": hexPram = "72 05 16 02 07 FF 81";
                        break;
                    case "6站耳机插入": hexPram = "72 05 16 02 08 00 81";
                        break;
                    case "6站耳机拔出": hexPram = "72 05 16 02 08 FF 81";
                        break;
                    case "6站开机按下": hexPram = "72 05 16 02 09 00 81";
                        break;
                    case "6站开机弹起": hexPram = "72 05 16 02 09 FF 81";
                        break;
                    case "6站TOUCH按下": hexPram = "72 05 16 02 0a 00 81";
                        break;
                    case "6站TOUCH弹起": hexPram = "72 05 16 02 0a FF 81";
                        break;
                    case "6站EAR-L": hexPram = "72 05 16 03 01 00 81";
                        break;
                    case "6站EAR-R": hexPram = "72 05 16 03 01 FF 81";
                        break;
                    case "6站EAR-MIC打开": hexPram = "72 05 16 03 02 00 81";
                        break;
                    case "6站EAR-MIC关闭": hexPram = "72 05 16 03 02 FF 81";
                        break;
                    case "6站EAR-VOL+按下": hexPram = "72 05 16 03 03 00 81";
                        break;
                    case "6站EAR-VOL+弹起": hexPram = "72 05 16 03 03 FF 81";
                        break;
                    case "6站EAR-VOL-按下": hexPram = "72 05 16 03 04 00 81";
                        break;
                    case "6站EAR-VOL-弹起": hexPram = "72 05 16 03 04 FF 81";
                        break;
                    case "6站HOOK按下": hexPram = "72 05 16 03 05 00 81";
                        break;
                    case "6站HOOK弹起": hexPram = "72 05 16 03 05 FF 81";
                        break;
                    case "6站国标耳机": hexPram = "72 05 16 03 06 00 81";
                        break;
                    case "6站美标耳机": hexPram = "72 05 16 03 06 FF 81";
                        break;

                    case "6站电脑连接": hexPram = "72 05 16 04 00 FF 81";
                        break;
                    case "6站电脑断开": hexPram = "72 05 16 04 00 00 81";
                        break;
                    case "6站充电打开": hexPram = "72 05 16 04 01 FF 81";
                        break;
                    case "6站充电关闭": hexPram = "72 05 16 04 01 00 81";
                        break;
                    case "6站OTG连接": hexPram = "72 05 16 04 02 FF 81";
                        break;
                    case "6站OTG断开": hexPram = "72 05 16 04 02 00 81";
                        break;
                    case "6站OTG反插": hexPram = "72 05 16 04 03 FF 81";
                        break;
                    case "6站OTG正插": hexPram = "72 05 16 04 03 00 81";
                        break;
                    case "6站负载断开": hexPram = "72 05 16 04 04 FF 81";
                        break;
                    case "6站负载连接": hexPram = "72 05 16 04 04 00 81";
                        break;
                    case "6站电压读取": hexPram = "72 05 16 04 10 00 81";
                        break;
                    case "6站充电电流读取": hexPram = "72 05 16 04 11 00 81";
                        break;
                    case "6站负载电流读取": hexPram = "72 05 16 04 12 00 81";
                        break;
                    case "6站报警": hexPram = "72 05 16 05 01 00 81";
                        break;
                    case "6站报警正常": hexPram = "72 05 16 05 01 FF 81";
                        break;
                    case "6站状态正常": hexPram = "72 05 16 05 02 00 81";
                        break;
                    case "6站状态异常": hexPram = "72 05 16 05 02 ff 81";
                        break;

                    case "6站状态指示绿灯": hexPram = "72 05 16 05 03 00 81";
                        break;
                    case "6站状态指示红灯": hexPram = "72 05 16 05 03 FF 81";
                        break;
                    case "6站状态指示黄灯": hexPram = "72 05 16 05 03 AA 81";
                        break;

                    case "6站复位": hexPram = "72 04 16 5A FF 81";
                        break;
                    case "6站获取位置": hexPram = "72 05 16 06 05 FF 81";
                        break;
                    case "6站回零点": hexPram = "72 05 16 06 03 FF 81";
                        break;
                    case "6站运动": hexPram = "72 0D 16 06 01";
                        break;
                    case "6站测试完成": hexPram = "72 05 16 05 02 00 81";
                        break;
                    case "6站测试中": hexPram = "72 05 16 05 02 FF 81";
                        break;
                    #endregion
                 
                }
            }
            else if (Common.Model == "CAM")
            {
                switch (pram)
                {
                    #region  1站
                    #region  01模块 1站检测
                    case "1站产品到位检测": hexPram = "72 05 C1 01 01 FF 81";
                        break;
                    case "1站防呆检测": hexPram = "72 05 C1 01 02 FF 81";
                        break;
                    case "1站USB拨出检测": hexPram = "72 05 C1 01 03 FF 81";
                        break;
                    case "1站取放原点检测": hexPram = "72 05 C1 01 04 FF 81";
                        break;
                    case "1站取放1抬起检测": hexPram = "72 05 C1 01 05 FF 81";
                        break;
                    case "1站取放1下压检测": hexPram = "72 05 C1 01 06 FF 81";
                        break;
                    case "1站取放1吸取检测": hexPram = "72 05 C1 01 07 FF 81";
                        break;
                    case "1站取放2抬起检测": hexPram = "72 05 C1 01 08 FF 81";
                        break;
                    case "1站取放2下压检测": hexPram = "72 05 C1 01 09 FF 81";
                        break;
                    case "1站取放2吸取检测": hexPram = "72 05 C1 01 0a FF 81";
                        break;
                    case "1站警告取消检测": hexPram = "72 05 C1 01 0b FF 81";
                        break;
                    #endregion

                    #region  02模块 1站气缸
                    case "1站产品固定": hexPram = "72 05 C1 02 01 00 81";
                        break;
                    case "1站产品松开": hexPram = "72 05 C1 02 01 FF 81";
                        break;
                    case "1站USB插入": hexPram = "72 05 C1 02 02 00 81";
                        break;
                    case "1站USB拔出": hexPram = "72 05 C1 02 02 FF 81";
                        break;
                    case "1站后白靠近": hexPram = "72 05 C1 02 03 00 81";
                        break;
                    case "1站后白远离": hexPram = "72 05 C1 02 03 FF 81";
                        break;
                    case "1站取放1抬起": hexPram = "72 05 C1 02 04 FF 81";
                        break;
                    case "1站取放1下压": hexPram = "72 05 C1 02 04 00 81";
                        break;
                    case "1站取放1吸取": hexPram = "72 05 C1 02 05 FF 81";
                        break;
                    case "1站取放1松开": hexPram = "72 05 C1 02 05 00 81";
                        break;
                    case "1站取放2抬起": hexPram = "72 05 C1 02 06 FF 81";
                        break;
                    case "1站取放2下压": hexPram = "72 05 C1 02 06 00 81";
                        break;
                    case "1站取放2吸取": hexPram = "72 05 C1 02 07 FF 81";
                        break;
                    case "1站取放2松开": hexPram = "72 05 C1 02 07 00 81";
                        break;
                    case "1站状态警告": hexPram = "72 05 C1 02 08 FF 81";
                        break;
                    case "1站状态正常": hexPram = "72 05 C1 02 08 00 81";
                        break;
                    #endregion

                    #region 03模块  光源调节
                    case "1站后白光源调节": hexPram = "72 05 C1 03 01";
                        break;
                    case "1站近焦光源调节": hexPram = "72 05 C1 03 02";
                        break;
                    #endregion

                    #region  04模块
                    case "1站状态灯绿灯": hexPram = "72 05 C1 04 01 00 81";
                        break;
                    case "1站状态灯红灯": hexPram = "72 05 C1 04 01 FF 81";
                        break;
                    case "1站状态灯黄灯": hexPram = "72 05 C1 04 01 AA 81";
                        break;
                    case "1站取放": hexPram = "72 05 C1 04 02 00 81";
                        break;
                    #endregion
                    #endregion

                    #region  2站
                    #region  01模块 2站检测
                    case "2站产品到位检测": hexPram = "72 05 C2 01 01 FF 81";
                        break;
                    case "2站OISX原点检测": hexPram = "72 05 C2 01 02 FF 81";
                        break;
                    case "2站OISZ原点检测": hexPram = "72 05 C2 01 03 FF 81";
                        break;
                    case "2站解析远离检测": hexPram = "72 05 C2 01 04 FF 81";
                        break;
                    case "2站色卡远离检测": hexPram = "72 05 C2 01 05 FF 81";
                        break;
                    case "2站警告取消": hexPram = "72 05 C2 01 06 FF 81";
                        break;
                    #endregion

                    #region  02模块 2站气缸
                    case "2站手机固定": hexPram = "72 05 C2 02 01 00 81";
                        break;
                    case "2站手机松开": hexPram = "72 05 C2 02 01 FF 81";
                        break;
                    case "2站上下靠近": hexPram = "72 05 C2 02 02 00 81";
                        break;
                    case "2站上下远离": hexPram = "72 05 C2 02 02 FF 81";
                        break;
                    case "2站左右靠近": hexPram = "72 05 C2 02 03 00 81";
                        break;
                    case "2站左右远离": hexPram = "72 05 C2 02 03 FF 81";
                        break;
                    case "2站前后靠近": hexPram = "72 05 C2 02 04 FF 81";
                        break;
                    case "2站前后远离": hexPram = "72 05 C2 02 04 00 81";
                        break;
                    case "2站增距靠近": hexPram = "72 05 C2 02 05 FF 81";
                        break;
                    case "2站增距远离": hexPram = "72 05 C2 02 05 00 81";
                        break;
                    case "2站解析靠近": hexPram = "72 05 C2 02 06 FF 81";
                        break;
                    case "2站解析远离": hexPram = "72 05 C2 02 06 00 81";
                        break;
                    case "2站色卡靠近": hexPram = "72 05 C2 02 07 FF 81";
                        break;
                    case "2站色卡远离": hexPram = "72 05 C2 02 07 00 81";
                        break;
                    case "2站状态警告": hexPram = "72 05 C2 02 08 FF 81";
                        break;
                    case "2站状态正常": hexPram = "72 05 C2 02 08 00 81";
                        break;
                    #endregion

                    #region 03模块  光源调节
                    case "2站后色光源调节": hexPram = "72 05 C2 03 01";
                        break;
                    #endregion

                    #region  04模块
                    case "2站状态灯绿灯": hexPram = "72 05 C2 04 01 00 81";
                        break;
                    case "2站状态灯红灯": hexPram = "72 05 C2 04 01 FF 81";
                        break;
                    case "2站状态灯黄灯": hexPram = "72 05 C2 04 01 AA 81";
                        break;
                    case "2站OISX电机": hexPram = "72 05 C2 04 02";
                        break;
                    case "2站OISY电机": hexPram = "72 05 C2 04 03";
                        break;
                    #endregion
                    #endregion

                    #region  3站
                    #region  01模块 3站检测
                    case "3站产品到位检测": hexPram = "72 05 C3 01 01 FF 81";
                        break;
                    case "3站前白远离检测": hexPram = "72 05 C3 01 02 FF 81";
                        break;
                    case "3站解析远离检测": hexPram = "72 05 C3 01 03 FF 81";
                        break;
                    case "3站暗室打开检测": hexPram = "72 05 C3 01 04 FF 81";
                        break;
                    case "3站取放3抬起检测": hexPram = "72 05 C3 01 05 FF 81";
                        break;
                    case "3站取放3下压检测": hexPram = "72 05 C3 01 06 FF 81";
                        break;
                    case "3站取放3吸取检测": hexPram = "72 05 C3 01 07 FF 81";
                        break;
                    case "3站取放3出仓检测": hexPram = "72 05 C3 01 08 FF 81";
                        break;
                    case "3站出仓原点检测": hexPram = "72 05 C3 01 09 FF 81";
                        break;
                    case "3站警告取消检测": hexPram = "72 05 C3 01 0a FF 81";
                        break;


                    #endregion

                    #region  02模块 3站气缸
                    case "3站产品固定": hexPram = "72 05 C3 02 01 00 81";
                        break;
                    case "3站产品松开": hexPram = "72 05 C3 02 01 FF 81";
                        break;
                    case "3站前白靠近": hexPram = "72 05 C3 02 02 00 81";
                        break;
                    case "3站前白远离": hexPram = "72 05 C3 02 02 FF 81";
                        break;
                    case "3站后白靠近": hexPram = "72 05 C3 02 03 00 81";
                        break;
                    case "3站后白远离": hexPram = "72 05 C3 02 03 FF 81";
                        break;
                    case "3站暗室打开": hexPram = "72 05 C3 02 04 FF 81";
                        break;
                    case "3站暗室关闭": hexPram = "72 05 C3 02 04 00 81";
                        break;
                    case "3站取放3抬起": hexPram = "72 05 C3 02 05 FF 81";
                        break;
                    case "3站取放3下压": hexPram = "72 05 C3 02 05 00 81";
                        break;
                    case "3站取放3吸取": hexPram = "72 05 C3 02 06 FF 81";
                        break;
                    case "3站取放3松开": hexPram = "72 05 C3 02 06 00 81";
                        break;
                    case "3站状态警告": hexPram = "72 05 C3 02 07 FF 81";
                        break;
                    case "3站状态正常": hexPram = "72 05 C3 02 07 00 81";
                        break;

                    #endregion

                    #region 03模块  光源调节
                    case "3站前色光源调节": hexPram = "72 05 C3 03 01";
                        break;
                    case "3站前白光源调节": hexPram = "72 05 C3 03 01";
                        break;
                    case "3站后白光源调节": hexPram = "72 05 C3 03 01";
                        break;
                    case "3站近焦光源调节": hexPram = "72 05 C3 03 01";
                        break;
                    #endregion

                    #region  04模块
                    case "3站状态灯绿灯": hexPram = "72 05 C3 04 01 00 81";
                        break;
                    case "3站状态灯红灯": hexPram = "72 05 C3 04 01 FF 81";
                        break;
                    case "3站状态灯黄灯": hexPram = "72 05 C3 04 01 AA 81";
                        break;
                    case "3站出仓取放": hexPram = "72 05 C3 04 02 00 81";
                        break;
                    #endregion
                    #endregion

                }
            }


            return hexPram;

        }
    }
}
