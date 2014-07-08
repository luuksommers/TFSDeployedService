using System.Configuration;
using System.ServiceProcess;

namespace TFSDeployService
{
    public abstract class BaseServiceInstaller : System.Configuration.Install.Installer
    {
        public BaseServiceInstaller()
        {
            Installers.Add(new ServiceProcessInstaller
            {
                Account = ServiceAccount.NetworkService
            });

            Installers.Add(new ServiceInstaller
            {
                ServiceName = ConfigurationManager.AppSettings["ServiceName"],
                StartType = ServiceStartMode.Automatic
            });
        }
    }
}