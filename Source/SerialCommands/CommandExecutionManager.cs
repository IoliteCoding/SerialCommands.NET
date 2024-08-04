using System;
using System.Collections.Concurrent;
using System.Threading;
using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Models;

namespace IoliteCoding.SerialCommands
{

    public class CommandExecutionManager : ICommandExecutionManager
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;
        private readonly Thread _executionThread;
        private readonly BlockingCollection<CommandExecutionContext> _commandExecutions;
        private bool _disposedValue;

        public int BufferCount => _commandExecutions.Count;

        public CommandExecutionManager()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            _commandExecutions = new BlockingCollection<CommandExecutionContext>();

            _executionThread = new Thread(Start);
            _executionThread.IsBackground = true;
            _executionThread.Start();
        }

        public CommandExecutionManager(ICancellationTokenSourceManager cancellationTokenSourceManager)
        {
            if (cancellationTokenSourceManager == null) throw new ArgumentNullException(nameof(cancellationTokenSourceManager));

            _cancellationToken = cancellationTokenSourceManager.Instance.Token;

            _commandExecutions = new BlockingCollection<CommandExecutionContext>();

            _executionThread = new Thread(Start);
            _executionThread.IsBackground = true;
            _executionThread.Start();
        }

        public bool TryAddCommandExecution(CommandExecutionContext execution)
        {
            if (_disposedValue) throw new ObjectDisposedException(nameof(CommandExecutionManager));

            if (_cancellationToken.IsCancellationRequested) return false;
            if (_commandExecutions.IsAddingCompleted) return false;

            _commandExecutions.Add(execution);
            return true;
        }

        public void Start()
        {
            try
            {
                foreach (var command in _commandExecutions.GetConsumingEnumerable(_cancellationToken))
                {
                    command.Delegate.DynamicInvoke(command.Address, command.Data);
                }

            }
            catch (ObjectDisposedException) { }
            catch (OperationCanceledException) { }
            catch { }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _commandExecutions.CompleteAdding();
                    _cancellationTokenSource?.Cancel();
                    _commandExecutions.Dispose();
                    try { _executionThread.Abort(); } catch { }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CommandExecutionManager()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
