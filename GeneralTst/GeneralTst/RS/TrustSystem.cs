using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GeneralTst.RS
{
    public class TrustSystem
    {
        [DllImport("TrustSystem.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TSEnd(string path);

        [DllImport("TrustSystem.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Demo(string path);

        [DllImport("TrustSystem.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short FLogin(string path, string login);

        [DllImport("TrustSystem.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetDataLen(string path, string curveName);

        [DllImport("TrustSystem.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Goto(string path, string command);

        [DllImport("TrustSystem.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LoadPro(string path, string pro);

        [DllImport("TrustSystem.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LoadProOver(string path);

        [DllImport("TrustSystem.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Result(string path);

        [DllImport("TrustSystem.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SN_Number(string path, string sn);

        [DllImport("TrustSystem.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TSCommand(string path,string command);

        [DllImport("TrustSystem.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ShowTheWindow(string path, string show);


    }
}
