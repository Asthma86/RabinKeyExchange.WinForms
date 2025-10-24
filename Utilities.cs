using System;
using System.Numerics;
using System.Security.Cryptography;

namespace RabinKeyExchange.WinForms
{
    public static class Utilities
    {
        public static byte[] BigIntegerToBytesUnsigned(BigInteger value)
        {
            if (value < 0)
                throw new ArgumentException("Value must be non-negative");

            byte[] bytes = value.ToByteArray();

            if (bytes.Length > 1 && bytes[bytes.Length - 1] == 0)
            {
                Array.Resize(ref bytes, bytes.Length - 1);
            }

            Array.Reverse(bytes);
            return bytes;
        }

        public static BigInteger BytesToBigIntegerUnsigned(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return BigInteger.Zero;

            byte[] tmp = new byte[bytes.Length];             //копируем и переворачиваем 
            Array.Copy(bytes, tmp, bytes.Length);
            Array.Reverse(tmp);

            //если старший бит установлен, добавляем ведущий ноль для положительного числа
            if (tmp.Length > 0 && (tmp[tmp.Length - 1] & 0x80) != 0)
            {
                byte[] signed = new byte[tmp.Length + 1];
                Array.Copy(tmp, signed, tmp.Length);
                tmp = signed;
            }

            return new BigInteger(tmp);
        }


        public static BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            BigInteger m0 = m;
            BigInteger y = 0, x = 1;
            if (m == 1) return 0;
            while (a > 1)
            {
                BigInteger q = a / m;
                BigInteger t = m;
                m = a % m;
                a = t;
                t = y;
                y = x - q * y;
                x = t;
            }
            if (x < 0) x += m0;
            return x;
        }

        public static BigInteger CombineCRT(BigInteger x1, BigInteger x2, BigInteger p, BigInteger q, BigInteger s, BigInteger t)
        {
            return (x1 * q * t + x2 * p * s) % (p * q);
        }

        public static BigInteger GenerateRandomBigInteger(int bitLength)
        {
            var bytes = new byte[(bitLength + 7) / 8];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);
            bytes[bytes.Length - 1] |= 0x80; // ensure top bit is set
            return new BigInteger(bytes);
        }

        public static bool IsProbablyPrime(BigInteger n, int certainty)
        {
            return n.IsProbablePrime(certainty);
        }
    }
    public static class BigIntegerExtensions
    {
        public static bool IsProbablePrime(this BigInteger value, int certainty)
        {
            if (value <= 1) return false;
            if (value == 2 || value == 3) return true;
            if (value % 2 == 0) return false;

            BigInteger d = value - 1;
            int s = 0;
            while (d % 2 == 0) { d /= 2; s++; }

            using var rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[8];
            for (int i = 0; i < certainty; i++)
            {
                rng.GetBytes(bytes);
                BigInteger a = new BigInteger(bytes);
                a = BigInteger.Abs(a);
                if (a < 2) a = 2;
                if (a >= value - 1) a = value - 2;

                BigInteger x = BigInteger.ModPow(a, d, value);
                if (x == 1 || x == value - 1) continue;

                bool composite = true;
                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, value);
                    if (x == value - 1)
                    {
                        composite = false;
                        break;
                    }
                }
                if (composite) return false;
            }
            return true;
        }
    }
}