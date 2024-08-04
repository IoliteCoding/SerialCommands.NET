using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IoliteCoding.SerialCommands
{
    public class StreamManager : ICommandReader, ICommandWriter, IDisposable
    {
        private Thread _thread;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ICommandEncryptor _encryptor;
        private bool _disposedValue;

        private readonly Stream _stream;
        public Stream Stream => _stream;

        public bool IsRunning => _thread?.IsAlive ?? false;

        public Action<byte[]> MessageReceivedHandler { get; set; }
        public Action<Exception> ExceptionHandler { get; set; }

        public StreamManager(Stream stream, ICommandEncryptor encryptor)
        {
            _stream = stream;
            _encryptor = encryptor;
        }

        public bool TryStart()
        {
            if (_thread != null && _thread.IsAlive)
            {
                return true;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _thread = new Thread(() => ReadStream().Wait());
            _thread.IsBackground = true;
            _thread.Start();
            return true;
        }

        protected virtual async Task ReadStream()
        {
            byte[] buffer = null;
            int index = 0;
            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    if (_stream != null && _stream.CanRead)
                    {
                        int messageByte = await GetNextByte(_stream, _cancellationTokenSource.Token);

                        if (messageByte >= 0)
                        {
                            if (messageByte == _encryptor.StartByte)
                            {
                                buffer = null;
                                index = -1;
                            }

                            else if (buffer == null && index == -1 && messageByte > 0)
                            {
                                buffer = new byte[messageByte + 3];
                                index = 0;
                                buffer[index++] = _encryptor.StartByte;
                                buffer[index++] = (byte)messageByte;
                            }

                            else if (buffer != null && messageByte == _encryptor.EndByte)
                            {
                                buffer[index] = (byte)messageByte;
                                TryProcessBuffer(buffer);
                                buffer = null;
                                index = 0;
                            }

                            else if (buffer != null && index < buffer.Length)
                            {
                                buffer[index++] = (byte)messageByte;
                            }

                            else
                                Debug.Write((char)messageByte);

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(ex);
            }
        }

        private void TryProcessBuffer(byte[] buffer)
        {
            Task.Run(() =>
            {
                try
                {
                    MessageReceivedHandler(buffer);
                }
                catch (Exception ex)
                {
                    ExceptionHandler?.Invoke(ex);
                }
            });
        }

        protected virtual async Task<int> GetNextByte(Stream stream, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return stream.ReadByte();
                }
                catch (Exception ex)
                {
                    ExceptionHandler?.Invoke(ex);
                }
                return -1;
            }, cancellationToken);
        }


        public bool TryStop()
        {
            _cancellationTokenSource?.Cancel();
            return true;

        }

        public bool Write(int address, byte[] bytes)
        {
            if (Stream == null) return false;
            try
            {
                byte[] message = _encryptor.Encode(address, bytes ?? new byte[] { });
                Stream.Write(message, 0, message.Length);
                Stream.Flush();
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public bool Write(int address, byte[] bytes, EncryptorOptions encryptorOptions)
        {
            if (Stream == null) return false;
            try
            {
                byte[] message = _encryptor.Encode(address, bytes ?? new byte[] { }, encryptorOptions);
                Stream.Write(message, 0, message.Length);
                Stream.Flush();
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _cancellationTokenSource?.Cancel();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~StreamManager()
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
