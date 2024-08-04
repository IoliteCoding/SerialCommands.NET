using System.Threading;

namespace IoliteCoding.SerialCommands.Abstraction
{
    public interface ICancellationTokenSourceManager
    {
        CancellationTokenSource Instance { get; }

        void Cancel();
    }
}
