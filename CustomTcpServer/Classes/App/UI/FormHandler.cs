using CustomTcpServer;
using System.Windows.Forms;

namespace InfinityServer.App.UI
{
    public class FormHandler
    {
        public MainServerForm MainServerForm { get; }

        public FormHandler()
        {
            MainServerForm = new MainServerForm();
        }
    }
}
