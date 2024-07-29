using System.Threading;

namespace SerialCommands
{
    public interface ICancellationTokenSourceManager
    {
        CancellationTokenSource Instance { get; }

        void Cancel();
    }

    public class CancellationTokenSourceManager : ICancellationTokenSourceManager
    {

        private CancellationTokenSource _instance;
        public CancellationTokenSource Instance => _instance ??= new CancellationTokenSource();

        public void Cancel() => _instance?.Cancel();
    }
}
