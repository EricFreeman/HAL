using System.Configuration;

namespace HAL.Framework
{
    public static class Config
    {
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
    }
}
