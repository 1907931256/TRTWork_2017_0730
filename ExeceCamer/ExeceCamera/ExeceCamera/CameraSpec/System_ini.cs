
namespace ExeceCamera.CameraSpec
{
    class System_ini
    {
        private string maxRG;
        private string minRG;
        private string mAXBG;
        private string minBG;
        private string maxRG1;
        private string minRG1;
        private string mAXBG1;
        private string minBG1;

        private string colorMaxStdev;
        private string colorMinStdev;

        private string colorFrontMaxStdev;
        private string colorFrontMinStdev;



        public string MaxRG { get => maxRG; set => maxRG = value; }
        public string MinRG { get => minRG; set => minRG = value; }
        public string MAXBG { get => mAXBG; set => mAXBG = value; }
        public string MinBG { get => minBG; set => minBG = value; }
        public string MaxRG1 { get => maxRG1; set => maxRG1 = value; }
        public string MinRG1 { get => minRG1; set => minRG1 = value; }
        public string MAXBG1 { get => mAXBG1; set => mAXBG1 = value; }
        public string MinBG1 { get => minBG1; set => minBG1 = value; }
        public string ColorMaxStdev { get => colorMaxStdev; set => colorMaxStdev = value; }
        public string ColorMinStdev { get => colorMinStdev; set => colorMinStdev = value; }
        public string ColorFrontMaxStdev { get => colorFrontMaxStdev; set => colorFrontMaxStdev = value; }
        public string ColorFrontMinStdev { get => colorFrontMinStdev; set => colorFrontMinStdev = value; }
    }
}



