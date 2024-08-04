using System.Threading;
using IoliteCoding.SerialCommands.Abstraction;

namespace IoliteCoding.SerialCommands
{

    public class CancellationTokenSourceManager : ICancellationTokenSourceManager
    {

        private CancellationTokenSource _instance;
        public CancellationTokenSource Instance => _instance ??= new CancellationTokenSource();

        public CancellationTokenSourceManager() { }

        public CancellationTokenSourceManager(CancellationTokenSource cancellationTokenSource)
        {
            _instance = cancellationTokenSource;
        }

        public void Cancel() => _instance?.Cancel();
    }
}
