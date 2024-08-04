using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Models;
using System;
using System.IO;
using System.IO.Ports;

namespace IoliteCoding.SerialCommands
{
    public class SerialPortManager : ICommandWriter, ICommandReader, IDisposable
    {
        private readonly SerialPort _serialPort;
        private readonly StreamManager _streamManager;
        private bool _disposedValue;

        public SerialPortManager(string portName, int baudRate, ICommandEncryptor encryptor)
        {
            _serialPort = new SerialPort(portName, baudRate);
            _serialPort.Open();

            _streamManager = new StreamManager(_serialPort.BaseStream, encryptor)
            {
                MessageReceivedHandler = x => MessageReceivedHandler?.Invoke(x),
                ExceptionHandler = x => ExceptionHandler?.Invoke(x)
            };
        }

        public Stream Stream => _streamManager.Stream;

        public Action<Exception> ExceptionHandler { get; set; }

        public bool IsRunning => _streamManager.IsRunning;

        public Action<byte[]> MessageReceivedHandler { get; set; }

        public bool TryStart()
        {
            return _streamManager.TryStart();
        }

        public bool TryStop()
        {
            return !_streamManager.TryStop();
        }

        public bool Write(int address, byte[] bytes)
        {
            return _streamManager.Write(address, bytes);
        }

        public bool Write(int address, byte[] bytes, EncryptorOptions encryptorOptions)
        {
            return _streamManager.Write(address, bytes, encryptorOptions);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _streamManager.Dispose();
                    _serialPort.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
