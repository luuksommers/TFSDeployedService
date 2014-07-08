using System;

namespace TFSDeployService
{
    public interface IServiceRunnable : IDisposable
    {
        bool IsRunning { get; }
        void Start();
        void Stop();
    }
}