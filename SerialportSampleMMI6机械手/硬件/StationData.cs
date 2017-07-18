using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Station
{
    class StationData_1                 //站1数据
    {
        public string err = "There is a error occurred";
        public string fuwei = "72 04 11 5a ff 81";

        #region  检测
        public string a1 = "72 05 11 01 01 FF 81"; //1站产品到位检测

        public string a2 = "72 05 11 01 02 FF 81"; //1站防呆检测

        public string a3 = "72 05 11 01 03 FF 81"; //1站USB拨出检测

        public string a4 = "72 05 11 01 04 FF 81"; //1站耳机检测

        public string a5 = "72 05 11 01 05 FF 81"; //1站关门检测

        public string a6 = "72 05 11 01 06 FF 81"; //1站开门检测

        public string a7 = "72 05 11 01 07 FF 81"; //1站红外感应检测

        public string a8 = "72 05 11 01 08 FF 81"; //1站暗室打开检测


        public string a9 = "72 05 11 01 09 FF 81"; //1站电机1原点检测

        public string b1 = "72 05 11 01 0a FF 81"; //1站电机1左边检测

        public string b2 = "72 05 11 01 0b FF 81"; //1站电机1右边检测

        public string b3 = "72 05 11 01 0c FF 81"; //1站上升1检测

        public string b4 = "72 05 11 01 0d FF 81"; //1站下降1检测

        public string b5 = "72 05 11 01 0e FF 81"; //1站吸嘴1检测

        public string b6 = "72 05 11 01 0f FF 81"; //1站电机2原点检测

        public string b7 = "72 05 11 01 10 FF 81"; //1站电机2左边检测

        public string b8 = "72 05 11 01 11 FF 81"; //1站电机2右边检测

        public string b9 = "72 05 11 01 12 FF 81"; //1站上升2检测

        public string c1 = "72 05 11 01 13 FF 81"; //1站下降2检测

        public string c2 = "72 05 11 01 14 FF 81"; //1站吸嘴2检测

        public string c3 = "72 05 11 01 15 FF 81"; //1站电机3原点检测

        public string c4 = "72 05 11 01 16 FF 81"; //1站电机3左边检测

        public string c5 = "72 05 11 01 17 FF 81"; //1站电机3右边检测

        public string c6 = "72 05 11 01 18 FF 81"; //1站上升3检测

        public string c7 = "72 05 11 01 19 FF 81"; //1站下降3检测

        public string c8 = "72 05 11 01 1a FF 81"; //1站吸嘴3检测

        public string c9 = "72 05 11 01 1b FF 81"; //1站电机4原点检测

        public string d1 = "72 05 11 01 1c FF 81"; //1站电机4左边检测

        public string d2 = "72 05 11 01 1d FF 81"; //1站电机4右边检测

        public string d3 = "72 05 11 01 1e FF 81"; //1站上升4检测



        public string d5 = "72 05 11 01 1f FF 81"; //1站下降4检测

        public string d6 = "72 05 11 01 20 FF 81"; //1站吸嘴4检测

        public string d7 = "72 05 11 01 21 FF 81"; //1站电机5原点检测

        public string d8 = "72 05 11 01 22 FF 81"; //1站电机5左边检测

        public string d9 = "72 05 11 01 23 FF 81"; //1站电机5右边检测

        public string e1 = "72 05 11 01 24 FF 81"; //1站上升5检测

        public string e2 = "72 05 11 01 25 FF 81"; //1站下降5检测

        public string e3 = "72 05 11 01 26 FF 81"; //1站吸嘴5检测





        public string e4 = "72 05 11 01 27 FF 81"; //1站电机6原点检测

        public string e5 = "72 05 11 01 28 FF 81"; //1站电机6左边检测

        public string e6 = "72 05 11 01 29 FF 81"; //1站电机6右边检测

        public string e7 = "72 05 11 01 2a FF 81"; //1站上升6检测

        public string e8 = "72 05 11 01 2b FF 81"; //1站下降6检测

        public string e9 = "72 05 11 01 2c FF 81"; //1站吸嘴6检测

        public string f1 = "72 05 11 01 2d FF 81"; //1站电机7原点检测

        public string f2 = "72 05 11 01 2e FF 81"; //1站电机7左边检测

        public string f3 = "72 05 11 01 2f FF 81"; //1站电机7右边检测

        public string f4 = "72 05 11 01 30 FF 81"; //1站上升7检测

        public string f5 = "72 05 11 01 31 FF 81"; //1站下降7检测

        public string f6 = "72 05 11 01 32 FF 81"; //1站吸嘴7检测

        public string f7 = "72 05 11 01 33 FF 81"; //1站电机8原点检测

        public string f8 = "72 05 11 01 34 FF 81"; //1站电机8左边检测

        public string f9 = "72 05 11 01 35 FF 81"; //1站电机8右边检测

        public string g1 = "72 05 11 01 36 FF 81"; //1站上升8检测

        public string g2 = "72 05 11 01 37 FF 81"; //1站下降8检测

        public string g3 = "72 05 11 01 38 FF 81"; //1站吸嘴8检测

        public string ZhuangTai_26 = "72 05 11 01 39 FF 81"; //1站下降8检测

        public string BaoJin_26 = "72 05 11 01 3a FF 81"; //1站吸嘴8检测





        #endregion

        #region 气缸

        public string x1 = "72 05 11 02 01 00 81"; //1站手机固定
        public string x2 = "72 05 11 02 01 FF 81"; //1站手机松开

        public string x3 = "72 05 11 02 02 00 81"; //1站USB插入
        public string x4 = "72 05 11 02 02 FF 81"; //1站USB拔出

        public string x5 = "72 05 11 02 03 00 81"; //1站耳机插入
        public string x6 = "72 05 11 02 03 FF 81"; //1站耳机拔出

        public string x7 = "72 05 11 02 04 00 81"; //1站N磁靠近
        public string x8 = "72 05 11 02 04 FF 81"; //1站N磁远离

        public string x9 = "72 05 11 02 05 00 81"; //1站S磁靠近
        public string y1 = "72 05 11 02 05 FF 81"; //1站S磁远离

        public string y2 = "72 05 11 02 06 00 81"; //1站前门开门
        public string y3 = "72 05 11 02 06 FF 81"; //1站前门关门

        public string y4 = "72 05 11 02 07 00 81"; //1站后门开门
        public string y5 = "72 05 11 02 07 FF 81"; //1站后门关门

        public string y6 = "72 05 11 02 08 00 81"; //1站升降机缸1升起
        public string y7 = "72 05 11 02 08 FF 81"; //1站升降机缸1降下

        public string y8 = "72 05 11 02 09 00 81"; //1站取放吸嘴1吸取
        public string y9 = "72 05 11 02 09 FF 81"; //1站取放吸嘴1放下

        public string z1 = "72 05 11 02 0a 00 81"; //1站升降机缸2升起
        public string z2 = "72 05 11 02 0a FF 81"; //1站取放吸嘴2降下

        public string z3 = "72 05 11 02 0b 00 81"; //1站取放吸嘴2吸取
        public string z4 = "72 05 11 02 0b FF 81"; //1站取放吸嘴2放下

        public string z5 = "72 05 11 02 0c 00 81"; //1站升降机缸3升起
        public string z6 = "72 05 11 02 0c FF 81"; //1站取放吸嘴3降下

        public string z7 = "72 05 11 02 0d 00 81"; //1站取放吸嘴3吸取
        public string z8 = "72 05 11 02 0d FF 81"; //1站取放吸嘴3放下

        public string z9 = "72 05 11 02 0e 00 81"; //1站升降机缸4升起
        public string q1 = "72 05 11 02 0e FF 81"; //1站取放吸嘴4降下

        public string q2 = "72 05 11 02 0f 00 81"; //1站取放吸嘴4吸取
        public string q3 = "72 05 11 02 0f FF 81"; //1站取放吸嘴4放下

        public string q4 = "72 05 11 02 10 00 81"; //1站升降机缸5升起
        public string q5 = "72 05 11 02 10 FF 81"; //1站取放吸嘴5降下

        public string q6 = "72 05 11 02 11 00 81"; //1站取放吸嘴5吸取
        public string q7 = "72 05 11 02 11 FF 81"; //1站取放吸嘴5放下

        public string q8 = "72 05 11 02 12 00 81"; //1站升降机缸6升起
        public string q9 = "72 05 11 02 12 FF 81"; //1站取放吸嘴6降下

        public string q10 = "72 05 11 02 13 00 81"; //1站取放吸嘴6吸取
        public string q11 = "72 05 11 02 13 FF 81"; //1站取放吸嘴6放下

        public string q12 = "72 05 11 02 14 00 81"; //1站升降机缸7升起
        public string q13 = "72 05 11 02 14 FF 81"; //1站取放吸嘴7降下

        public string q14 = "72 05 11 02 15 00 81"; //1站取放吸嘴7吸取
        public string q15 = "72 05 11 02 15 FF 81"; //1站取放吸嘴7放下

        public string q16 = "72 05 11 02 16 00 81"; //1站升降机缸8升起
        public string q17 = "72 05 11 02 16 FF 81"; //1站取放吸嘴8降下

        public string q18 = "72 05 11 02 17 00 81"; //1站取放吸嘴8吸取
        public string q19 = "72 05 11 02 17 FF 81"; //1站取放吸嘴8放下

        #endregion

        #region  状态
        public string t1 = "72 05 11 05 01 00 81"; //
        public string t2 = "72 05 11 05 01 FF 81"; //

        public string t3 = "72 05 11 05 02 00 81"; //1站状态正常
        public string t4 = "72 05 11 05 02 FF 81"; //1站状态异常

        public string t5 = "72 05 11 05 03 00 81"; //1站绿灯
        public string t6 = "72 05 11 05 03 AA 81"; //1站黄灯
        public string t7 = "72 05 11 05 03 FF 81"; //1站红灯
        #endregion

        #region 耳机
        public string r1 = "72 05 11 03 01 00 81"; //1站EAR-L
        public string r2 = "72 05 11 03 01 FF 81"; //1站EAR-R

        public string r3 = "72 05 11 03 02 00 81"; //1站EAR-MIC打开
        public string r4 = "72 05 11 03 02 FF 81"; //1站EAR-MIC关闭

        public string r5 = "72 05 11 03 03 00 81"; //1站EAR-VOL+按下
        public string r6 = "72 05 11 03 03 FF 81"; //1站EAR-VOL+按下

        public string r7 = "72 05 11 03 04 00 81"; //1站EAR-VOL-按下
        public string r8 = "72 05 11 03 04 FF 81"; //1站EAR-VOL-按下

        public string r9 = "72 05 11 03 05 00 81"; //1站HOOK按下
        public string r10 = "72 05 11 03 05 FF 81"; //1站HOOK弹起

        public string r11 = "72 05 11 03 05 00 81"; //1站国标耳机
        public string r12 = "72 05 11 03 05 FF 81"; //1站美标耳机

        #endregion

        #region    06模块 电机部分调节

        public string yundong1_zuo = "72 05 11 04 08 00 81"; //1左

        public string yundong1_zhong = "72 05 11 04 08 AA 81"; //1中

        public string yundong1_you = "72 05 11 04 08 FF 81"; //1右


        public string yundong2_zuo = "72 05 11 04 09 00 81"; //1左

        public string yundong2_zhong = "72 05 11 04 09 AA 81"; //1中

        public string yundong2_you = "72 05 11 04 09 FF 81"; //1右



        public string yundong3_zuo = "72 05 11 04 08 0a 81"; //1左

        public string yundong3_zhong = "72 05 11 04 0a AA 81"; //1中

        public string yundong3_you = "72 05 11 04 0a FF 81"; //1右


        public string yundong4_zuo = "72 05 11 04 0b 00 81"; //1左

        public string yundong4_zhong = "72 05 11 04 0b AA 81"; //1中

        public string yundong4_you = "72 05 11 04 0b FF 81"; //1右



        public string yundong5_zuo = "72 05 11 04 0c 00 81"; //1左

        public string yundong5_zhong = "72 05 11 04 0c AA 81"; //1中

        public string yundong5_you = "72 05 11 04 0c FF 81"; //1右
      
        #endregion


        public string ceShiZhuangTai_1 = "72 05 11 07 01 00 81"; //  3站测试状态测试中
        public string ceShiZhuangTai_2 = "72 05 11 07 01 ff 81"; //   3站测试状态测试完成

        public string MS_QC = "72 05 11 07 02 00 81"; //  3站测试状态测试中
        public string MS_ZC = "72 05 11 07 02 ff 81"; //   3站测试状态测试完成



    }
    class StationData_4                 //站2数据  改5站
    {
        public string fuwei = "72 04 14 5a ff 81";
#region 到位检测
        public string canPinDaoWei_JC = "72 05 14 01 01 FF 81";       // 5站产品到位检测

        public string tianBanYuanLI_JC = "72 05 14 01 02 FF 81";       // 5站天板远离检测

        public string tianBan_KaoJin_JC = "72 05 14 01 03 FF 81";       // 5站天板靠近检测

        public string yuanDian_JC = "72 05 14 01 04 FF 81";       // 1站原点检测

        public string zhuangTai_JC = "72 05 14 01 05 FF 81";       // 1站状态检测



#endregion
        #region 气缸动作


        public string guDing_QG = "72 05 14 02 01 00 81";     //5站手机固定
        public string songKai_QG = "72 05 14 02 01 FF 81";     //5站手机松开

        public string juLi_1_ZheTang_QG = "72 05 14 02 02 00 81";     //5站距离近遮挡
        public string juLi_1_YuanLi_QG = "72 05 14 02 02 FF 81";     //5站距离1远离

        public string juLi_2_ZheTang_QG = "72 05 14 02 03 00 81";     //5站距离中遮挡
        public string juLi_2_YuanLi_QG = "72 05 14 02 03 FF 81";     //5站距离中远离

        public string tianBan_2_ZheTang_QG = "72 05 14 02 04 00 81";     //5站天板遮挡
        public string tianBan_2_YuanLi_QG = "72 05 14 02 04 FF 81";     //5站天板远离

        public string nFC_2_ZheTang_QG = "72 05 14 02 05 00 81";     //5站NFC2遮挡
        public string nFC_2_YuanLi_QG = "72 05 14 02 05 FF 81";     //5站NFC2远离
  
        public string FH_KJ_QG = "72 05 14 02 06 00 81";     //5站返回靠近/远离
        public string FH_YL_QG = "72 05 14 02 06 FF 81";     //5站返回靠近/远离

        public string ZY_KJ_QG = "72 05 14 02 07 00 81";     //5站主页靠近/远离
        public string ZY_YL_QG = "72 05 14 02 07 FF 81";     //5站主页靠近/远离

        public string CD_KJ_QG = "72 05 14 02 08 00 81";     //5站菜单靠近/远离
        public string CD_YL_QG = "72 05 14 02 08 FF 81";     //5站菜单靠近/远离

        public string ZW_KJ_QG = "72 05 14 02 09 00 81";     //5站指纹靠近/远离
        public string ZW_YL_QG = "72 05 14 02 09 FF 81";     //5站指纹靠近/远离

        public string HB_KJ_QG = "72 05 14 02 0a 00 81";     //5站后白靠近/远离
        public string HB_YL_QG = "72 05 14 02 0a FF 81";     //5站后白靠近/远离
        #endregion

#region 光源调节
  
        public string aa1 = "72 05 14 03 01";       //5站后白光源调节

        public string aa2 = "72 05 14 03 02";       // 5站备用光源调节

      

#endregion



        #region  数据读取
        public string qianShangGuang_JC = "72 05 14 04 01 FF 81";       // 前闪光灯检测开始

        public string qianShangGuang_DuQu = "72 05 14 04 01 00 81";       // 前闪光灯数据读取

        public string lCDBeiGuang_JC = "72 05 14 04 02 FF 81";       // LCD背光检测开始

        public string lCDBeiGuang_DuQu = "72 05 14 04 02 00 81";       // LCD背光数据读取

        public string houShangGuang_JC = "72 05 14 04 03 FF 81";       // 后闪光灯检测开始

        public string houShangGuang_DuQu = "72 05 14 04 03 00 81";       // 后闪光灯数据读取

        public string xinHaoDeng_JC = "72 05 14 04 04 FF 81";       // 信号灯检测开始

        public string xinHaoDeng_DuQu = "72 05 14 04 04 00 81";       // 信号灯数据读取
        #endregion

        #region    状态指示
        public string baoJin_2 = "72 05 14 05 01 00 81";       //5站报警
        public string zhengChang_2 = "72 05 14 05 01 FF 81";       //5站正常

        public string zhuangTaiZhengChang_2 = "72 05 14 05 02 00 81";  // 5站状态正常
        public string zhuangTaiYiChang_2 = "72 05 14 05 02 FF 81";  //   5站状态异常

        public string zhuangTaiZhiDeng_Pass_2 = "72 05 14 05 03 00 81";  // 5站状态指示灯pass
        public string zhuangTaiZhiDeng_Fail_2 = "72 05 14 05 03 FF 81";  //  5站状态指示灯fail
        public string zhuangTaiZhiDeng_Mie_2 = "72 05 14 05 03 AA 81";  //  5站状态指示灯灭

        public string hongWaiFaSong = "A1 F1 01 02 03";              // 红外数据发送
        #endregion

        public string ceShiZhuangTai_1 = "72 05 14 06 01 00 81"; //  3站测试状态测试中
        public string ceShiZhuangTai_2 = "72 05 14 06 01 ff 81"; //   3站测试状态测试完成



    }
    class StationData_2                 //3站数据定义   改2站
    {
        public string fuwei                 = "72 04 12 5a ff 81";
        #region 检测
        public string canPinDaoWei_JC = "72 05 12 01 01 FF 81";    // 2站产品到位检测

        public string dianJi_1_YuanDian_JC = "72 05 12 01 02 FF 81";    // 2站防抖电机1原点

        public string dianJi_2_YuanDian_JC = "72 05 12 01 03 FF 81";    // 2站防抖电机2原点

        public string dengXiangYuanLi_JC = "72 05 12 01 04 FF 81";    // 2站灯箱远离检测

        public string zhenJuYuanLi_JC = "72 05 12 01 05 FF 81";    // 2站增距远离检测

        public string jinJiaoYuanLi_JC = "72 05 12 01 06 FF 81";    // 2站近焦远离检测

        public string jieXi_JC = "72 05 12 01 07 FF 81";    // 2站解析远离检测

        public string SFR_JC = "72 05 12 01 08 FF 81";    // 2站SFR远离检测

        public string jinJiao = "72 05 12 01 09 FF 81";    // 2站近焦上升到位检测

        public string a = "72 05 12 01 0a FF 81";    //1站原点检测

        public string b = "72 05 12 01 0b FF 81";    //1站状态检测
       







        #endregion

        #region  气缸动作
        public string shouJiGuDing_QG = "72 05 12 02 01 00 81";    // 2站手机固定
        public string shouJiSongKai_QG = "72 05 12 02 01 FF 81";    // 5站手机松开

        public string jiaoZhunGuanYuan_KaoJin_QG = "72 05 12 02 02 00 81";    // 2站校准光源靠近
        public string jiaoZhunGuanYuan_YuanLi_QG = "72 05 12 02 02 FF 81";    // 2站校准光源远离

        public string zhenJu_KaoJin_QG = "72 05 12 02 03 00 81";    // 2站增距镜靠近
        public string zhenJu_YuanLi_QG = "72 05 12 02 03 FF 81";    //  2站增距镜远离

        public string jinJiao_KaoJin_QG = "72 05 12 02 04 00 81";    // 2站近焦靠近
        public string jinJiao_YuanLi_QG = "72 05 12 02 04 FF 81";    //2站近焦远离

        public string jinJiao_ShangShen_QG = "72 05 12 02 05 00 81";    // 2站近焦上升
        public string jinJiao_XiaJiang_QG = "72 05 12 02 05 FF 81";    //2站近焦下降

        public string jieXi_KaoJin_QG = "72 05 12 02 06 00 81";    // 2站解析靠近
        public string jieXi_YuanLi_QG = "72 05 12 02 06 FF 81";    //2站解析远离

        public string SFR_KaoJin_QG = "72 05 12 02 07 00 81";    //2站SFR靠近
        public string SFR_YuanLi_QG = "72 05 12 02 07 FF 81";    //2站SFR远离
        #endregion

        #region    光源调节

        public string sheKa_Shang_GuanYuanTiaoJie = "72 05 12 03 01";   // 2站色卡上光源调节

        public string sheKa_Zhaong_GuangYuanTiaoJie = "72 05 12 03 02";    // 2站色卡中光源调节

        public string sheKa_XiaGuangYuanTiaoJie = "72 05 12 03 03";   // 2站色卡下光源调节

        public string jiaoZhun_GuanYuan = "72 05 12 03 04";   // 2站校准光源调节

        public string jinJu_GuangYuan = "72 05 12 03 05";   // 2站近距内光源调节

        public string wai_GuangYuan = "72 05 12 03 06";   // 2站近距外光源调节


        #endregion

        #region   电机控制

        public string fangDou_1_TiaoJie = "72 05 12 04 01";   // 2站OIS防抖1启动

        public string fangDou_2_TiaoJie= "72 05 12 04 02";   // 2站OIS防抖2启动

        public string tianJi_ShangDian = "72 05 12 04 03 00 81";   // 2站电机电源通电
        public string tianJi_DuanDian = "72 05 12 04 03 FF 81";   // 2站电机电源断电
        #endregion

        #region    状态指示
        public string baoJin_3 = "72 05 12 05 01 00 81";       //2站报警
        public string zhengChang_3 = "72 05 12 05 01 FF 81";       //2站正常

        public string zhuangTaiZhengChang_3 = "72 05 12 05 02 00 81";  // 2站状态正常
        public string zhuangTaiYiChang_3 = "72 05 12 05 02 FF 81";  //   2站状态异常

        public string zhuangTaiZhiDeng_Pass_3 = "72 05 12 05 03 00 81"; // 2站状态指示灯pass
        public string zhuangTaiZhiDeng_Fail_3 = "72 05 12 05 03 FF 81";  //  2站状态指示灯fail
        public string zhuangTaiZhiDeng_Mie_3 = "72 05 12 05 03 AA 81";  //  2站状态指示灯灭
        #endregion
        public string ceShiZhuangTai_1 = "72 05 12 06 01 00 81"; //  3站测试状态测试中
        public string ceShiZhuangTai_2 = "72 05 12 06 01 ff 81"; //   3站测试状态测试完成




    }
    class StationData_3               //5站数据定义  3站
    {
        public string fuwei = "72 04 13 5a ff 81";

        #region 检测
        public string canPinDaoWei_JC = "72 05 13 01 01 FF 81";    // 3站产品到位检测

        public string dianJi_1_YuanDian_JC = "72 05 13 01 02 FF 81";    // 3站解析远离检测

        public string dianJi_2_YuanDian_JC = "72 05 13 01 03 FF 81";    // 3站3站45度放平检测

        public string dingQi_JC = "72 05 13 01 04 FF 81";//3站3站45度顶起检测

        public string dengXiangYuanLi_JC = "72 05 13 01 05 FF 81";    // 3站灯色卡光源靠近检测

        public string zhenJuYuanLi_JC = "72 05 13 01 06 FF 81";    // 3站增距远色卡光源远离检测

        public string quFang_SS= "72 05 13 01 07 FF 81";    // 3站取放上升检测

        public string quFang_XJ = "72 05 13 01 08 FF 81";    // 3站3站取放下降检测

        public string yuanDian_Station_one = "72 05 13 01 09 FF 81";    // 1站原点检测

        public string zhuangTai_Station_one = "72 05 13 01 0a FF 81";    // 1站状态检测


        #endregion

        #region  气缸
        public string shouJiGuDing_QG = "72 05 13 02 01 00 81";    // 3站手机固定
        public string shouJiSongKai_QG = "72 05 13 02 01 FF 81";    // 5站手机松开

        public string jiaoZhunGuanYuan_KaoJin_QG = "72 05 13 02 02 00 81";    // 3站45度顶起
        public string jiaoZhunGuanYuan_YuanLi_QG = "72 05 13 02 02 FF 81";    // 3站45度放下

        public string zhenJu_KaoJin_QG = "72 05 13 02 03 00 81";    //3站前白灯箱靠近
        public string zhenJu_YuanLi_QG = "72 05 13 02 03 FF 81";    //  3站前白灯箱远离

        public string jinJiao_KaoJin_QG = "72 05 13 02 04 00 81";    // 3站解析靠近
        public string jinJiao_YuanLi_QG = "72 05 13 02 04 FF 81";    //3站解析远离

        public string jinJiao_ShangShen_QG = "72 05 13 02 05 FF 81";    // 3站光源上升
        public string jinJiao_XiaJiang_QG = "72 05 13 02 05 00 81";    //3站3站光源下降

        public string jieXi_KaoJin_QG = "72 05 13 02 06 00 81";    // 3站取放上升
        public string jieXi_YuanLi_QG = "72 05 13 02 06 FF 81";    //3站取放下降

        public string SFR_KaoJin_QG = "72 05 13 02 07 00 81";    //3站夹具定位
        public string SFR_YuanLi_QG = "72 05 13 02 07 FF 81";    //3站夹具松开

        #endregion

        #region 光源调节
        public string sheKa_Shang_GuanYuanTiaoJie = "72 05 13 03 01";   // 3站色卡上光源调节

        public string sheKa_Zhaong_GuangYuanTiaoJie = "72 05 13 03 02";    // 3站色卡中光源调节

        public string sheKa_XiaGuangYuanTiaoJie = "72 05 13 03 03";   // 3站色卡下光源调节

        public string jiaoZhun_GuanYuan = "72 05 13 03 04";   // 3站前白光源调节

        public string jinJu_GuangYuan = "72 05 13 03 05";   // 3站后白光源调节
        #endregion

        #region 电机控制部分
    

        #endregion

        #region    状态指示
        public string baoJin_4 = "72 05 13 05 01 00 81";      //3站报警
        public string zhengChang_4 = "72 05 13 05 01 FF 81";       //3站正常

        public string zhuangTaiZhengChang_4 = "72 05 13 05 02 00 81";  // 3站状态正常
        public string zhuangTaiYiChang_4 = "72 05 13 05 02 FF 81";  //   3站状态异常

        public string zhuangTaiZhiDeng_Pass_4 = "72 05 13 05 03 00 81"; // 3站状态指示灯pass
        public string zhuangTaiZhiDeng_Fail_4 = "72 05 13 05 03 FF 81";  //  3站状态指示灯fail
        public string zhuangTaiZhiDeng_Mie_4 = "72 05 13 05 03 AA 81"; //  3站状态指示灯灭
        #endregion

        public string ceShiZhuangTai_1 = "72 05 13 06 01 00 81"; //  3站测试状态测试中
        public string ceShiZhuangTai_2 = "72 05 13 06 01 ff 81"; //   3站测试状态测试完成


    }              
    class StationData_5
    {
           #region   到位检测
           public string canPingDaoWei_JC =      "72 05 15 01 01 FF 81";                      //5站产品到位检测

           public string guanXiang_JC =          "72 05 15 01 02 FF 81";                        //5站人工嘴下降检测

           public string kaiXiang_JC =           "72 05 15 01 03 FF 81";                        //5站人工耳上升检测

           public string renEr_JC =              "72 05 15 01 04 FF 81";                          //5站耳机拨出检测

           public string renGongZui_JC =         "72 05 15 01 05 FF 81";                    //5站耳机拨出检测

           public string aa1 =               "72 05 15 01 06 FF 81"; 	                        //5站隔离关闭检测

           public string aa2 =              "72 05 15 01 07 FF 81";                        //1站原点状态检测

           public string aa3 =         "72 05 15 01 08 FF 81";                  //1站状态检测

           public string aa4 =               "72 05 15 01 09 FF 81";                        //报警清除
       


        #endregion

           #region 气缸 
           public string a1 = "72 05 15 02 01 00 81";             //5站手机固定
           public string b1 = "72 05 15 02 01 ff 81";             //5站手机松开

           public string a2 = "72 05 15 02 02 00 81";             //5站USB插入/拨出
           public string b2 = "72 05 15 02 02 ff 81";             //5站USB插入/拨出

           public string a3 = "72 05 15 02 03 00 81";             //5站隔音箱上升/下降
           public string b3 = "72 05 15 02 03 ff 81";             //5站隔音箱上升/下降

           public string a4 = "72 05 15 02 04 00 81";             //5站人工嘴上升/下降
           public string b4 = "72 05 15 02 04 ff 81";             //5站人工嘴上升/下降

           public string a5 = "72 05 15 02 05 00 81";             //5站人工嘴左移/右移
           public string b5 = "72 05 15 02 05 ff 81";             //5站人工嘴左移/右移

           public string a6 = "72 05 15 02 06 00 81";             //5站人工耳上升/下降
           public string b6 = "72 05 15 02 06 ff 81";             //5站人工耳上升/下降

           public string a7 = "72 05 15 02 07 00 81";             //5站耳机插入/拨出
           public string b7 = "72 05 15 02 07 ff 81";             //5站耳机插入/拨出



           public string A1 = "72 05 15 02 08 00 81";             //5站振动打开/关闭
           public string B1 = "72 05 15 02 08 ff 81";             //5站振动打开/关闭

           public string A2 = "72 05 15 02 09 00 81";             //5站听筒打开/关闭
           public string B2 = "72 05 15 02 09 ff 81";             //5站听筒打开/关闭

           public string A3 = "72 05 15 02 0A 00 81";             //5站喇叭打开/关闭
           public string B3 = "72 05 15 02 0A ff 81";             //5站喇叭打开/关闭

           public string A4 = "72 05 15 02 0B 00 81";             //5站备用输入打开/关闭
           public string B4 = "72 05 15 02 0B ff 81";             //5站备用输入打开/关闭

           public string A5 = "72 05 15 02 0C 00 81";             //5站MIC打开/关闭
           public string B5 = "72 05 15 02 0C ff 81";             //5站MIC打开/关闭

           public string A6 = "72 05 15 02 0D 00 81";             //5站MIC1打开/关闭
           public string B6 = "72 05 15 02 0D ff 81";             //5站MIC1打开/关闭

           public string A7 = "72 05 15 02 0E 00 81";             //5站备用输出打开/关闭
           public string B7 = "72 05 15 02 0E ff 81";             //5站备用输出打开/关闭




           #endregion

           #region    状态指示
           public string baoJin = "72 05 15 05 01 00 81";      //5站报警
           public string zhengChang = "72 05 15 05 01 FF 81";       //5站正常

           public string zhuangTaiZhengChang= "72 05 15 05 02 00 81";  // 5站状态正常
           public string zhuangTaiYiChang = "72 05 15 05 02 FF 81";  //   5站状态异常

           public string zhuangTaiZhiDeng_Pass ="72 05 15 05 03 00 81"; // 5站状态指示灯pass
           public string zhuangTaiZhiDeng_Fail ="72 05 15 05 03 FF 81";  //  5站状态指示灯fail
           public string zhuangTaiZhiDeng_Mie  ="72 05 15 05 03 AA 81"; //  5站状态指示灯灭
           #endregion
           public string fuwei = "72 04 15 5a ff 81"; //  5站状态指示灯灭
           public string ceShiZhuangTai_1 = "72 05 15 06 01 00 81"; //  3站测试状态测试中
           public string ceShiZhuangTai_2 = "72 05 15 06 01 ff 81"; //   3站测试状态测试完成


    }
    class StationData_6
    {
        public string fuwei = "72 04 16 5a ff 81";  //复位

        #region   检测 

        public string canPinDaoWei_JC = "72 05 16 01 01 FF 81";             // 6站产品到位检测					
        public string dianJi_X_JC = "72 05 16 01 02 FF 81";             // 6站电机X位置检测					
        public string dianJi_Y_JC = "72 05 16 01 03 FF 81";             // 6站电机Y位置检测			
        public string erJi_JC = "72 05 16 01 04 FF 81";             // 6站电机Z位置检测				
        public string USB_JC = "72 05 16 01 05 FF 81";             // 6站USB拨出检测
        public string erJi_BaChu_JC = "72 05 16 01 06 FF 81";             //6站耳机拨出检测
        public string quFang_XiaYa_JC = "72 05 16 01 07 FF 81";             //6站取放下压到位
        public string quFang_TaiQI_JC = "72 05 16 01 08 FF 81";             //6站取放抬起到位
        public string TpTaiQi_DaoWei_JC = "72 05 16 01 09 FF 81";             //6站TP抬起到位
        public string xiHeZhuangTai_JC = "72 05 16 01 0a FF 81";             //6站吸合状态检查
        public string quFangDaoWei_JC = "72 05 16 01 0b FF 81";             //取放到位检测

        public string zhaungtai_1 = "72 05 16 01 0c FF 81";             //6站吸合状态检查
        public string zhaungtai_2 = "72 05 16 01 0d FF 81";             //取放到位检测
        public string zhaungtai_3 = "72 05 16 01 0e FF 81";             //取放到位检测

        public string zhaungtai_4 = "72 05 16 01 0f FF 81";             //6站取放中间到位
        public string zhaungtai_5 = "72 05 16 01 10 FF 81";             //TP按下到位



        #endregion

        #region  气缸
        public string shouJiGuDing_QG = "72 05 16 02 01 00 81";    // 6站手机固定/松开
        public string shouJiSongKai_QG = "72 05 16 02 01 FF 81";    // 6站手机固定/松开

        public string jiaoZhunGuanYuan_KaoJin_QG = "72 05 16 02 02 00 81";    // 6站音量-按下/弹起
        public string jiaoZhunGuanYuan_YuanLi_QG = "72 05 16 02 02 FF 81";    // 6站音量-按下/弹起
        public string zhenJu_KaoJin_QG = "72 05 16 02 03 00 81";    //6站音量+按下/弹起
        public string zhenJu_YuanLi_QG = "72 05 16 02 03 FF 81";    // 6站音量+按下/弹起


        public string jinJiao_KaoJin_QG = "72 05 16 02 04 00 81";    // 6站TP按下/弹起
        public string jinJiao_YuanLi_QG = "72 05 16 02 04 FF 81";    //6站TP按下/弹起

        public string jinJiao_ShangShen_QG = "72 05 16 02 05 00 81";    //6站USB插入/拔出
        public string jinJiao_XiaJiang_QG = "72 05 16 02 05 FF 81";    //6站USB插入/拔出

        public string xiHe_KaoJin_QG = "72 05 16 02 06 00 81";    //6站吸盘吸合/松开
        public string xiHe_YuanLi_QG = "72 05 16 02 06 FF 81";    //6站吸盘吸合/松开

        public string xiFang_KaoJin_QG = "72 05 16 02 07 FF 81";    //6站取放靠近/远离
        public string xiFang_YuanLi_QG = "72 05 16 02 07 00 81";    //6站取放靠近/远离
        public string xiFang_zj_QG = "72 05 16 02 07 AA 81";    //6站取放靠近/远离

        public string erJi_KaoJin_QG = "72 05 16 02 08 00 81";    //6站耳机插入/拔出
        public string erJi_YuanLi_QG = "72 05 16 02 08 FF 81";    //6站耳机插入/拔出

        public string kaiJi_KaoJin_QG = "72 05 16 02 09 00 81";    //6站开机按下/弹起
        public string kaiJi_YuanLi_QG = "72 05 16 02 09 FF 81";    //6站开机按下/弹起

        public string touch_KaoJin_QG = "72 05 16 02 0a 00 81";    //6站TOUCH按下/弹起
        public string touch_YuanLi_QG = "72 05 16 02 0a FF 81";    //6站TOUCH按下/弹起

        #endregion




        #region   电机调节
        public string yunDong = "72 0D 16 06 01";           		// 运动指令	
        public string xieLv = "72 05 16 06 02";				//斜率调节									

        public string weiZhiHuoQu = "72 05 16 06 05 FF 81";             // 获取位置	
        public string huiLingDian = "72 05 16 06 03 FF 81";             // 回零点	

        #endregion

        #region 耳机调节
        public string ear_L = "72 05 16 03 01 00 81";             // 6站EAR-L/R					
        public string ear_R = "72 05 16 03 01 FF 81";             // 6站EAR-L/R				

        public string ear_Mic_Open = "72 05 16 03 02 00 81";             // 6站EAR-MIC					
        public string ear_Mic_Close = "72 05 16 03 02 FF 81";             // 6站EAR-MIC					

        public string ear_VolJia_Down = "72 05 16 03 03 00 81";             // 6站EAR-VOL+					
        public string ear_VolJia_Up = "72 05 16 03 03 FF 81";             // 6站EAR-VOL+					

        public string ear_VOLJian_Down = "72 05 16 03 04 00 81";             // 6站EAR-VOL-				
        public string ear_VOLJian_Up = "72 05 16 03 04 FF 81";             // 6站EAR-VOL-		

        public string HOOK_AnXia = "72 05 16 03 05 00 81";             // 6站HOOK					
        public string HOOK_TanQi = "72 05 16 03 05 ff 81";             // 6站HOOK					

        public string guoBiao_EAR = "72 05 16 03 06 00 81";             // 6站国/美标切换					
        public string meiBiao_EAR = "72 05 16 03 06 FF 81";             // 6站国/美标切换					


        #endregion

        #region      OTG_充电电流_电脑操作相关
        public string dianNao_LianJie = "72 05 16 04 00 FF 81";             // 电脑连接					
        public string dianNao_DuanKai = "72 05 16 04 00 00 81";             // 电脑断开					
        public string congDian_DaKai = "72 05 16 04 01 FF 81";             // 充电打开					
        public string congDian_GuanBi = "72 05 16 04 01 00 81";             // 充电关闭					
        public string OTG_LianJie = "72 05 16 04 02 FF 81";             // OTG连接					
        public string OTG_DuanKia = "72 05 16 04 02 00 81";             // OTG断开					
        public string OTG_FanCha = "72 05 16 04 03 FF 81";             // OTG反插					
        public string OTG_ZhengCha = "72 05 16 04 03 00 81";             // OTG正插					
        public string fuZai_DuanKai = "72 05 16 04 04 FF 81";             // 负载断开					
        public string fuZai_LianJie = "72 05 16 04 04 00 81";             // 负载连接					
        public string dianYa_DuQu = "72 05 16 04 10 00 81";             // 电压读取（mV）					
        public string congDian_DianLu_DuQu = "72 05 16 04 11 00 81";             // 充电电流读取（mA）					
        public string fuZai_DianLu_DuQu = "72 05 16 04 12 00 81";             // 负载电流读取（mA）					

        #endregion

        #region       状态指示

        public string baoJin_BaoJin = "72 05 16 05 01 00 81";             // 6站报警					
        public string zhengChang_BaoJin = "72 05 16 05 01 FF 81";             // 6站报警					

        public string zhenChang_ZhuangTai = "72 05 16 05 02 00 81";             // 6站状态					
        public string yiChang_ZhuangTai = "72 05 16 05 02 FF 81";             // 6站状态					

        public string pass_SanSeDeng = "72 05 16 05 03 00 81";             // 6站状态指示灯					
        public string fali_SanSeDeng = "72 05 16 05 03 FF 81";             // 6站状态指示灯					
        public string stop_SanSeDeng = "72 05 16 05 03 AA 81";             // 6站状态指示灯					




        public string ceShiZhuangTai_1 = "72 05 11 07 01 00 81"; //  3站测试状态测试中
        public string ceShiZhuangTai_2 = "72 05 11 07 01 ff 81"; //   3站测试状态测试完成

        #endregion
    }

}
