using System.Configuration;

namespace HAL.Framework
{
    public static class Config
    {
        public static string AppId
        {
            get { return "Z1CJPK7V34FdznfpAd331_1F3roYDPHpt0S6o9gfiG44WzYZVzNAyHCOJyBt1FRho0qEnzwc9HwdLuLF1HxfRxKyv4487u0-"; }
        }

        public static string UserName
        {
            get { return ConfigurationManager.AppSettings["UserName"]; }
        }

        public static string ComputerName
        {
            get { return ConfigurationManager.AppSettings["ComputerName"]; }
        }

        public static string StopListening
        {
            get { return ConfigurationManager.AppSettings["StopListening"]; }
        }

        public static string Location
        {
            get { return ConfigurationManager.AppSettings["Location"]; }
        }
    }
}
