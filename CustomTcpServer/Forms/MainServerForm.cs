using InfinityServer.App;
using InfinityServer.Classes.App.UI;

namespace CustomTcpServer
{
    public partial class MainServerForm : CustomForm
    {
        public MainServerForm()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, System.EventArgs e)
        {
            InfinityApplication.Instance.Shutdown();
        }

        private void MaximizeButton_Click(object sender, System.EventArgs e)
        {
            if(WindowState == System.Windows.Forms.FormWindowState.Normal)
            {
                WindowState = System.Windows.Forms.FormWindowState.Maximized;
                return;
            }

            WindowState = System.Windows.Forms.FormWindowState.Normal;
        }

        private void MinimizeButton_Click(object sender, System.EventArgs e)
        {
            WindowState = System.Windows.Forms.FormWindowState.Minimized;
        }
    }
}