using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System;
using Serilog;
using InfinityServer.App.UI;
using InfinityServer.Classes.Utils;
using Newtonsoft.Json;
using InfinityServer.Classes.Server;
using InfinityServer.Classes.Utils.InfinityServer.Classes.Utils;

namespace InfinityServer.App
{
    public class InfinityApplication
    {
        public static InfinityApplication Instance { get; private set; }

        private readonly FormHandler _formHandler;
        private readonly ILogger _logger;

        private readonly InfinityTcpServer _infinityTcpServer;
        private readonly Database _database;

        public InfinityApplication()
        {
            if (Instance == null)
                Instance = this;

            _formHandler = new FormHandler();

            _infinityTcpServer = new InfinityTcpServer(GetPortFromConfig());

            CryptoUtilityOld.Initialize();
            //CryptoUtility.Initialize();

            _database = new Database(GetConnectionStringFromConfig());

            _logger = new LoggerConfiguration()
                        .WriteTo.File("..\\..\\InfinityServer-logs.txt", rollingInterval: RollingInterval.Day)
                        .CreateLogger();

            _logger.Information($"(InfinityApplication.cs) - InfinityApplication(): App Started!");
        }

        public void Initialize()
        {
            _infinityTcpServer.StartServer();
            ShowForm(_formHandler.MainServerForm);
        }

        public void ShowForm(Form form)
        {
            form.Show();
        }

        public void HideForm(Form form)
        {
            form.Hide();
        }

        public void CloseForm(Form form)
        {
            form.Close();
        }

        public void Shutdown()
        {
            _infinityTcpServer.StopServer();

            Application.Exit();
            Environment.Exit(0);
        }

        public ILogger Logger { get { return _logger; } }

        public FormHandler FormHandler { get { return _formHandler; } }

        public InfinityTcpServer InfinityTcpServer { get { return _infinityTcpServer; } }

        public Database Database { get { return _database; } }

        private string GetConnectionStringFromConfig()
        {
            var config = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Configs\\config.json")));
            return config["Database"]["ConnectionString"];
        }

        private int GetPortFromConfig()
        {
            var config = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Configs\\config.json")));
            return int.Parse(config["Server"]["Port"]);
        }
    }
}
