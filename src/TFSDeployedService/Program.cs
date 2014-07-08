using NLog;

namespace TFSDeployService
{
    public class Program : WinServiceBase
    {
        public Program()
            : base(new DummyRunner(), LogManager.GetCurrentClassLogger())
        {
        }

        private static void Main(string[] args)
        {
            new Program().Run(args);
        }
    }
}