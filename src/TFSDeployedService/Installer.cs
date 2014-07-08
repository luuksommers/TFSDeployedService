using System.ComponentModel;

namespace TFSDeployService
{
    [RunInstaller(true)]
    public class Installer : BaseServiceInstaller
    {
        //Installer needs to be in the assembly of that contains the Main method. Otherwise the wrong
        //assembly gets installed as the startpoint for the windows service
    }
}