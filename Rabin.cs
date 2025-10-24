using System;
using System.Collections;
using System.Numerics;
using System.Security.Cryptography;

namespace RabinKeyExchange.WinForms
{
    public class Rabin
    {
        public BigInteger P { get; }
        public BigInteger Q { get; }
        public BigInteger N { get; }

        private Rabin(BigInteger p, BigInteger q)
        {
            P = p;
            Q = q;
            N = p * q;
        }
        public static Rabin GenerateKeys(int bitsPerPrime = 256) //генерация ключевой пары Рабина
        {
            var p = GeneratePrimeCongruent3Mod4(bitsPerPrime);
            var q = GeneratePrimeCongruent3Mod4(bitsPerPrime);
            return new Rabin(p, q);
        }

        public static byte[] CreatePaddedPlaintext(byte[] data)
        {
            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(data);
            byte[] result = new byte[2 + data.Length + hash.Length];
            result[0] = 0xAA;
            result[1] = 0x55;
            Array.Copy(data, 0, result, 2, data.Length);
            Array.Copy(hash, 0, result, 2 + data.Length, hash.Length);
            return result;
        }

        public static byte[]? TryDecryptRabin(Rabin keypair, BigInteger c)
        {
            var p = keypair.P; var q = keypair.Q; var n = keypair.N;
            BigInteger mp = BigInteger.ModPow(c % p, (p + 1) / 4, p);
            BigInteger mq = BigInteger.ModPow(c % q, (q + 1) / 4, q);

            BigInteger s = Utilities.ModInverse(p, q);
            BigInteger t = Utilities.ModInverse(q, p);

            BigInteger r1 = Utilities.CombineCRT(mp, mq, p, q, s, t);
            BigInteger r2 = n - r1;
            BigInteger r3 = Utilities.CombineCRT(mp, (q - mq) % q, p, q, s, t);
            BigInteger r4 = n - r3;

            BigInteger[] candidates = new[] { r1, r2, r3, r4 };
            foreach (var cand in candidates)
            {
                byte[] bytes = Utilities.BigIntegerToBytesUnsigned(cand);
                if (bytes.Length < 2) continue;

                if (bytes[0] != 0xAA || bytes[1] != 0x55) continue; //проверяем префикс

                int totalDataLen = bytes.Length - 2; //извлекаем данные и хеш
                if (totalDataLen < 32) continue; //хеш должен быть 32 байта

                byte[] data = new byte[totalDataLen - 32];
                Array.Copy(bytes, 2, data, 0, data.Length);

                byte[] providedHash = new byte[32];
                Array.Copy(bytes, 2 + data.Length, providedHash, 0, 32);

                using var sha = SHA256.Create(); //проверка целостности
                byte[] computedHash = sha.ComputeHash(data);
                if (StructuralComparisons.StructuralEqualityComparer.Equals(computedHash, providedHash))
                {
                    return data;
                }
            }
            return null;
        }
        private static BigInteger CombineCRT(BigInteger x1, BigInteger x2, BigInteger p, BigInteger q)
        {
            BigInteger m1 = p;
            BigInteger m2 = q;
            BigInteger M = m1 * m2;
            BigInteger M1 = m2;
            BigInteger M2 = m1;
            BigInteger y1 = ModInverse(M1, m1);
            BigInteger y2 = ModInverse(M2, m2);
            return (x1 * M1 * y1 + x2 * M2 * y2) % M;
        }

        private static BigInteger ModInverse(BigInteger a, BigInteger m) //обратный элемент по модулю
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

        private static BigInteger GeneratePrimeCongruent3Mod4(int bits) //генерирует простое число p = 3 (mod 4) заданной битности
        {
            using var rng = RandomNumberGenerator.Create();
            int byteCount = (bits + 7) / 8;

            while (true)
            {
                byte[] bytes = new byte[byteCount];
                rng.GetBytes(bytes);

                bytes[bytes.Length - 1] |= 0x80; //старший бит = 1
                bytes[0] |= 0x03; //два младших бита = 11 = 3 mod 4

                //делаем положительным
                byte[] signed = new byte[bytes.Length + 1];
                Array.Copy(bytes, 0, signed, 1, bytes.Length);
                Array.Reverse(signed);

                BigInteger candidate = new BigInteger(signed);

                if (candidate.GetBitLength() == bits &&
                    candidate % 4 == 3 &&
                    Utilities.IsProbablyPrime(candidate, 10))
                {
                    return candidate;
                }
            }
        }
    }
}