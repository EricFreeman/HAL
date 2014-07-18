using HAL.Framework;
using HAL.Framework.Modules;
using HAL.Implementation.Modules;

namespace HAL.Implementation
{
    public partial class MainWindow
    {
        #region Private Properties

        private Computer _hal { get; set; }

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            _hal = new Computer();
            _hal.LoadModule<DateAndTimeModule>();
            _hal.LoadModule<WeatherModule>();
        }

        #endregion

        #region Helper Methods

        private void Log(string message)
        {
            LogViewer.Text += string.Format("{0} \r\n", message) + "\r\n";
        }

        #endregion
    }
}