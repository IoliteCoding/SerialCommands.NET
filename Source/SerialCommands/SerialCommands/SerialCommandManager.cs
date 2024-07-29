using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace SerialCommands
{
    public interface ISerialCommandManager : IDisposable
    {
        event CommandReceivedDelegate CommandReceived;

        int CommandCount { get; }
        int BufferCount { get; }
        Stream Stream { get; set; }

        void AddCommand(int address, CommandDelegate command);
        void RemoveCommand(int address);
        bool TryExecuteCommand(byte[] data);
    }

    public class SerialCommandManager : ISerialCommandManager
    {
        private bool _disposedValue;
        private readonly ICommandEncryptor _encryptor;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ICommandExecutionManager _commandExecuter;
        protected readonly ConcurrentDictionary<int, CommandDelegateHolder> _commands;

        private CancellationTokenSource _cancellationTokenSourceReadStream;
        private Thread _readStreamThread;

        public event CommandReceivedDelegate CommandReceived;

        public int CommandCount => _commands?.Count ?? 0;

        public int BufferCount => _commandExecuter?.BufferCount ?? 0;

        private Stream _stream;
        public Stream Stream
        {
            get => _stream;
            set
            {
                if (_stream != value)
                {
                    if (_readStreamThread != null)
                    {
                        _cancellationTokenSourceReadStream.Cancel();
                        _cancellationTokenSourceReadStream.Dispose();
                    }
                    _stream = value;
                    _cancellationTokenSourceReadStream = new CancellationTokenSource();
                    _readStreamThread = new Thread(() => ReadStream().Wait());
                    _readStreamThread.IsBackground = true;
                    _readStreamThread.Start();
                }
            }
        }

        public SerialCommandManager(ICommandEncryptor encryptor, ICommandExecutionManager commandExecuter,
            ICancellationTokenSourceManager cancellationTokenSourceManager)
        {
            _commands = new ConcurrentDictionary<int, CommandDelegateHolder>();
            _encryptor = encryptor;
            _commandExecuter = commandExecuter;
            _cancellationTokenSource = cancellationTokenSourceManager?.Instance;
            _cancellationTokenSource?.Token.Register(CancelReadStream);
        }

        public SerialCommandManager() : this(new CommandEncryptor(), null, null)
        {
        }

        public virtual void AddCommand(int address, CommandDelegate command)
        {
            if (_disposedValue) throw new ObjectDisposedException(nameof(SerialCommandManager));

            CommandDelegateHolder holder = new CommandDelegateHolder(address, command);
            _commands.AddOrUpdate(address, holder, (key, value) => holder);
        }

        public void RemoveCommand(int address)
        {
            if (_disposedValue) throw new ObjectDisposedException(nameof(SerialCommandManager));

            _commands.TryRemove(address, out CommandDelegateHolder holder);
        }

        public bool TryExecuteCommand(byte[] data)
        {
            if (_disposedValue) throw new ObjectDisposedException(nameof(SerialCommandManager));

            var result = _encryptor.Decode(data);
            if (!_commands.ContainsKey(result.address))
            {
                CommandReceivedEventArgs e = new CommandReceivedEventArgs(result.address, result.data, null);
                OnCommandReceived(e);
            }

            if (_commands.TryGetValue(result.address, out CommandDelegateHolder holder))
            {
                CommandReceivedEventArgs e = new CommandReceivedEventArgs(result.address, result.data, holder?.Command);
                OnCommandReceived(e);

                if (_commandExecuter != null)
                {
                    return _commandExecuter.TryAddCommandExecution(new CommandExecutionContext(result.address, result.data, holder.Command));
                }

                holder.Command.DynamicInvoke(result.address, result.data);
                return true;
            }
            return false;
        }

        protected virtual void OnCommandReceived(CommandReceivedEventArgs e)
        {
            CommandReceived?.Invoke(this, e);
        }

        protected virtual void CancelReadStream()
        {
            _cancellationTokenSourceReadStream?.Cancel();
        }

        protected virtual async Task ReadStream()
        {
            try
            {
                byte[] bytes = null;
                int index = 0;
                while (!_cancellationTokenSourceReadStream.IsCancellationRequested)
                {
                    if (_stream != null && _stream.CanRead)
                    {
                        //int oneByte=_stream.ReadByte();
                        int oneByte = await GetNextByte(_stream, _cancellationTokenSourceReadStream.Token);

                        if (oneByte >= 0)
                        {
                            if (oneByte == _encryptor.StartByte)
                            {
                                bytes = null;
                                index = 0;
                            }

                            else if (bytes == null && oneByte != _encryptor.StartByte && oneByte != _encryptor.EndByte)
                            {
                                bytes = new byte[oneByte + 3];
                                bytes[index++] = _encryptor.StartByte;
                                bytes[index++] = (byte)oneByte;
                            }

                            else if (oneByte == _encryptor.EndByte || index + 4 == bytes.Length)
                            {
                                bytes[index] = (byte)oneByte;
                                Task.Run(() => { TryExecuteCommand(bytes); }).Start();
                                bytes = null;
                                index = 0;
                            }

                            else if (index < bytes.Length)
                            {
                                bytes[index++] = (byte)oneByte;
                            }
                        }
                    }
                }

            }
            catch (Exception) { }
        }

        private async Task<int> GetNextByte(Stream stream, CancellationToken cancellationToken)
        {
            CancellationTokenSource innerCancellationToken = new CancellationTokenSource();

            int result = -1;
            Task readByteTask = new Task(() => result = stream.ReadByte(), innerCancellationToken.Token);
            readByteTask.Start();
            Task delayTask = Task.Delay(3000, cancellationToken);

            Task completedTask = await Task.WhenAny(readByteTask, delayTask);
            innerCancellationToken.Cancel();
            if (completedTask == delayTask)
            {
                return -1;
            }
            else
                return result;

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
