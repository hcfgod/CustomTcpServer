using InfinityServer.App;
using System;
using System.Windows.Forms;

namespace CustomTcpServer.Classes.App.EntryPoint
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            InfinityApplication app = new InfinityApplication();
            app.Initialize();

            try
            {
                Application.Run();
            }
            catch(InvalidOperationException ioexp) { }
        }
    }
}
