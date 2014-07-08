namespace TFSDeployService
{
    public class DummyRunner : IServiceRunnable
    {
        public bool IsRunning { get; private set; }

        public void Dispose()
        {
        }

        public void Start()
        {
            IsRunning = true;
            // This method should not block, so start a thread or something :)
        }

        public void Stop()
        {
            IsRunning = false;
        }
    }
}