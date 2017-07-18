using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Station
{
    class Data
    {
        public string err = "输入的数据有误，重新输入";
        public string mokuai1_ZT_static = "55 00 F0 A5 88";
        public string mokuai2_ZT_static = "55 00 F1 A5 88";
        public string mokuai3_ZT_static = "55 00 F2 A5 88";
        public string mokuai4_ZT_static = "55 00 F3 A5 88";

        public string mokuai1_ZT_act = "F0 A5";
        public string mokuai2_ZT_act = "F1 A5";
        public string mokuai3_ZT_act = "F2 A5";
        public string mokuai4_ZT_act = "F3 A5";

        public string qidong_static = "55 00 AA 00 88";
        public string tinzhi_static = "55 00 AA FF 88";

        public string qidong_act = "AA 00";
        public string tinzhi_act = "AA FF";

        public string led1_lv_static = "55 01 00 54 88";
        public string led1_hong_static = "55 01 FF AB 88";

        public string led2_lv_static = "55 02 00 54 88";
        public string led2_hong_static = "55 02 FF AB 88";

        public string led3_lv_static = "55 04 00 54 88";
        public string led3_hong_static = "55 04 FF 81 88";

        public string led4_lv_static = "55 08 00 54 88";
        public string led4_hong_static = "55 08 FF 81 88";

        public string shangdaowei_act = "01 F0";
        public string xiadaowei_act   = "02 F0";

        public string jindaowei_act = "03 F0";
        public string chudaowei_act = "04 F0";

        public string zaibandaowei_act = "05 F0";

        public string mokuai_1_daowei_act = "0B F0";
        public string mokuai_2_daowei_act = "0D F0";
        public string mokuai_1_pzd_act = "0C F0";
        public string mokuai_2_pzd_act = "0E F0";







    }
}
