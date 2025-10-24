using System;
using System.IO;
using System.Net.Sockets;
using System.Numerics;
using System.Threading.Tasks;

namespace RabinKeyExchange.WinForms
{
    public class Message
    {
        public byte Type;
        public byte[] Payload;
    }

    public static class NetworkHelpers
    {
        public static async Task SendMessageAsync(NetworkStream stream, byte type, byte[] payload)
        {
            if (stream == null)
                throw new InvalidOperationException("Stream is null");

            byte[] lenBytes = BitConverter.GetBytes(payload.Length);
            if (BitConverter.IsLittleEndian) Array.Reverse(lenBytes);

            await stream.WriteAsync(new[] { type }, 0, 1);
            await stream.WriteAsync(lenBytes, 0, 4);
            await stream.WriteAsync(payload, 0, payload.Length);
        }

        public static async Task<Message> ReadMessageAsync(NetworkStream stream)
        {
            byte[] type = new byte[1];
            await stream.ReadExactlyAsync(type, 0, 1);
            byte[] lenBytes = new byte[4];
            await stream.ReadExactlyAsync(lenBytes, 0, 4);
            if (BitConverter.IsLittleEndian) Array.Reverse(lenBytes);
            int len = BitConverter.ToInt32(lenBytes, 0);
            byte[] payload = new byte[len];
            await stream.ReadExactlyAsync(payload, 0, len);
            return new Message { Type = type[0], Payload = payload };
        }

        public static async Task SendBigIntegerAsync(NetworkStream stream, BigInteger value)
        {
            byte[] bytes = Utilities.BigIntegerToBytesUnsigned(value);
            await SendMessageAsync(stream, 0x00, bytes);
        }

        public static async Task<BigInteger> ReadBigIntegerAsync(NetworkStream stream)
        {
            var msg = await ReadMessageAsync(stream);
            return Utilities.BytesToBigIntegerUnsigned(msg.Payload);
        }
    }

    public static class StreamExtensions
    {
        public static async Task ReadExactlyAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            int total = 0;
            while (total < count)
            {
                int read = await stream.ReadAsync(buffer, offset + total, count - total);
                if (read == 0) throw new EndOfStreamException();
                total += read;
            }
        }
    }
}