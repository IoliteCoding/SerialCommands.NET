using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Delegates;
using IoliteCoding.SerialCommands.EventArguments;
using IoliteCoding.SerialCommands.InternalCommands;
using IoliteCoding.SerialCommands.Models;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace IoliteCoding.SerialCommands
{

    public class CommandsManager : ISerialCommandManager
    {
        private bool _disposedValue;
        private readonly ICommandEncryptor _encryptor;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ICommandExecutionManager _commandExecuter;
        private readonly ICommandReader _commandReader;
        private readonly ICommandWriter _commandWriter;
        private readonly ICancellationTokenSourceManager _cancellationTokenSourceManager;
        protected readonly ConcurrentDictionary<int, CommandDelegateHolder> _commands;

        public event CommandReceivedDelegate CommandReceived;
        public event CommandExecutionExceptionThrownDelegate CommandExecutionExceptionThrown;

        public int CommandCount => _commands?.Count ?? 0;

        public int BufferCount => _commandExecuter?.BufferCount ?? 0;

        public Stream Stream
        {
            get => _commandReader?.Stream ?? _commandWriter.Stream;
        }

        public ICommandReader CommandReader => _commandReader;

        public ICommandWriter CommandWriter => _commandWriter;

        public CommandsManager(ICommandEncryptor encryptor, ICommandReader commandReader, ICommandWriter commandWriter,
                               ICommandExecutionManager commandExecuter, ICancellationTokenSourceManager cancellationTokenSourceManager)
        {
            _commands = new ConcurrentDictionary<int, CommandDelegateHolder>();

            _encryptor = encryptor ?? new CommandEncryptor();

            _commandExecuter = commandExecuter;

            _commandReader = commandReader;
            if (_commandReader != null)
            {
                _commandReader.MessageReceivedHandler = x => TryExecuteCommand(x);
                _commandReader.ExceptionHandler = HandelErrors;
            }

            _commandWriter = commandWriter;
            if (_commandWriter != null)
            {
                _commandWriter.ExceptionHandler = HandelErrors;
            }

            _cancellationTokenSourceManager = cancellationTokenSourceManager;
            _cancellationTokenSource = cancellationTokenSourceManager?.Instance;
            _cancellationTokenSource?.Token.Register(() => _commandReader?.TryStop());

            InitializeInternalCommands();
        }

        protected virtual void InitializeInternalCommands()
        {
            AddCommand(SetAddressLength.Address, new SetAddressLength(_encryptor).Delegate);
            AddCommand(SetAddressFactor.Address, new SetAddressFactor(_encryptor).Delegate);
            AddCommand(InternalCommand2.Address, new InternalCommand2().Delegate);
            AddCommand(InternalCommand3.Address, new InternalCommand3().Delegate);
            AddCommand(InternalCommand4.Address, new InternalCommand4().Delegate);
            AddCommand(InternalCommand5.Address, new InternalCommand5().Delegate);
            AddCommand(InternalCommand6.Address, new InternalCommand6().Delegate);
            AddCommand(InternalCommand7.Address, new InternalCommand7().Delegate);
            AddCommand(SetLog.Address, new SetLog().Delegate);
            AddCommand(GetStatus.Address, new GetStatus(_commandWriter, _encryptor).Delegate);
        }

        public virtual void AddCommand(int address, CommandDelegate command)
        {
            if (_disposedValue) throw new ObjectDisposedException(nameof(CommandsManager));

            CommandDelegateHolder holder = new(address, command);
            _commands.AddOrUpdate(address, holder, (key, value) => holder);
        }

        public void RemoveCommand(int address)
        {
            if (_disposedValue) throw new ObjectDisposedException(nameof(CommandsManager));

            _commands.TryRemove(address, out CommandDelegateHolder holder);
        }

        public bool TryExecuteCommand(byte[] data)
        {
            if (_disposedValue) throw new ObjectDisposedException(nameof(CommandsManager));

            var result = _encryptor.Decode(data);

            if (_commands.TryGetValue(result.address, out CommandDelegateHolder holder))
            {
                CommandReceivedEventArgs e = new(result.address, result.data, holder?.Command);
                OnCommandReceived(e);

                if (e.Handeled) return true;

                if (_commandExecuter != null)
                {
                    return _commandExecuter.TryAddCommandExecution(new CommandExecutionContext(result.address, result.data, holder.Command));
                }
                else
                {
                    holder.Command.Invoke(result.address, result.data);
                }

                return true;
            }
            else
            {
                CommandReceivedEventArgs e = new(result.address, result.data, null);
                OnCommandReceived(e);

                return e.Handeled;
            }
        }

        protected virtual void OnCommandReceived(CommandReceivedEventArgs e)
        {
            CommandReceived?.Invoke(this, e);
        }

        protected virtual void OnCommandExecutionExceptionThrown(CommandExecutionExceptionEventArgs e)
        {
            CommandExecutionExceptionThrown?.Invoke(this, e);
        }

        private void HandelErrors(Exception exception)
        {
            // Todo
        }

        internal protected class CommandDelegateHolder
        {
            public int Address { get; set; }
            public CommandDelegate Command { get; set; }

            public CommandDelegateHolder(int address, CommandDelegate command)
            {
                Address = address;
                Command = command;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _cancellationTokenSource?.Cancel();
                    _commandExecuter?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SerialCommandManager()
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
