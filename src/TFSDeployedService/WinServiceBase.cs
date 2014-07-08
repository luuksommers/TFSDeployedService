using System;
using System.Configuration;
using System.Configuration.Install;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using NLog;

namespace TFSDeployService
{
    public abstract class WinServiceBase : ServiceBase
    {
        private readonly Logger _logger;
        private readonly string _serviceName = ConfigurationManager.AppSettings["ServiceName"];
        private readonly IServiceRunnable _worker;

        protected WinServiceBase(IServiceRunnable worker, Logger logger)
        {
            _logger = logger;
            _worker = worker;
            ServiceName = _serviceName;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var ci = new CultureInfo(CultureInfo.InvariantCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Fatal(string.Format("{0} caused an unhandled exception", _serviceName),
                e.ExceptionObject as Exception);
            Stop();
        }

        public void Run(string[] startupArguments)
        {
            if (startupArguments != null && startupArguments.Length > 0)
            {
                switch (startupArguments[0].ToUpper())
                {
                    case "-I":
                        InstallService();
                        return;
                    case "-U":
                        UnInstallService();
                        return;
                    case "-D":
                        DebugMode();
                        return;
                    case "-H":
                        PrintUsage();
                        return;
                }
            }

            Run(this);
        }

        private void PrintUsage()
        {
            Console.Write("Usage: {0} \t -I = Install service {0} \t -U = Uninstall service {0} \t -D = Debug mode",
                Environment.NewLine);
        }

        private void DebugMode()
        {
            OnStart(null);

            Console.WriteLine("Service started...");
            Console.WriteLine("<press any key to exit>");
            Console.ReadKey();

            Stop();
        }

        private void UnInstallService()
        {
            ManagedInstallerClass.InstallHelper(new[] {"/u", Assembly.GetEntryAssembly().Location});
        }

        private void InstallService()
        {
            if (IsServiceInstalled())
                UnInstallService();

            ManagedInstallerClass.InstallHelper(new[] {Assembly.GetEntryAssembly().Location});
        }

        private bool IsServiceInstalled()
        {
            return ServiceController.GetServices().Any(s => s.ServiceName == _serviceName);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _logger.Info("{0} starting", _serviceName);
                _worker.Start();
            }
            catch (Exception ex)
            {
                _logger.Fatal(string.Format("{0} stopped with exception", _serviceName), ex);
                Stop();
            }
        }

        protected override void OnStop()
        {
            _logger.Info("{0} stopping", _serviceName);
            if (_worker.IsRunning)
                _worker.Stop();

            _worker.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_worker != null)
                _worker.Dispose();
        }
    }
}