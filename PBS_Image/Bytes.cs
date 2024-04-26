using System;

namespace PBS_Image
{

    public static class Bytes
    {
        /// <summary>
        /// Returns the maximum value for n bits
        /// </summary>
        /// <param name="n">Number of bits that can store value</param>
        /// <returns>the byte representing the maximum for n bits (string of "1")</returns>
        /// <exception cref="Exception">If n < 0</exception>
        public static byte GetMaxForNBits(int n)
        {
            if (n < 0)
                throw new Exception();
            else
            {
                if (n == 0)
                    return 0;
                else
                {
                    byte bite = 1;
                    for (int i = 1; i < n; i++)
                    {
                        bite = (byte)(bite << 1);
                        bite += 1;
                    }

                    return bite;
                }
            }
        }

        /// <summary>
        /// Reset the n least significant bits of a byte
        /// </summary>
        /// <param name="toReset">Byte that needs to be reset</param>
        /// <param name="n">number of LSBs to reset</param>
        /// <exception cref="Exception">If n < 0</exception>
        public static void ResetLeastSignificantBits(ref byte toReset, int n)
        {
            if (n < 0)
            {
                throw new Exception();
            }
            else
            {
                toReset = (byte)((toReset >> n) << n);
            }
        }

        /// <summary>
        /// Gets the n least significant bits of a byte
        /// </summary>
        /// <param name="toGet">byte to retrieve from</param>
        /// <param name="n">number of bits to retrieve</param>
        /// <returns>the N least significant bits</returns>
        /// <exception cref="Exception">If n < 0</exception>
        public static byte GetLeastSignificantBits(byte toGet, int n)
        {
            if (n < 0)
                throw new Exception();
            else
            {
                byte copy = toGet;
                copy = (byte)(copy >> n);
                copy = (byte)(copy << n);
                return ((byte)(toGet - copy));
            }
        }

        /// <summary>
        /// Sets the n least significant bits of a byte
        /// </summary>
        /// <param name="toSet">byte that will me modified</param>
        /// <param name="val">new LSB values</param>
        /// <param name="n">Number of bits that will be modified</param>
        /// <exception cref="Exception">If n < 0</exception>
        public static void SetLeastSignificantBits(ref byte toSet, byte val, int n)
        {
            if (n < 0)
                throw new Exception();
            else
            {
                toSet = (byte)(toSet >> n);
                toSet = (byte)(toSet << n);
                val = GetLeastSignificantBits(val, n);
                toSet += val;

            }
        }

        /// <summary>
        /// Compress a byte of 8 bits into a byte of n bits
        /// </summary>
        /// <param name="toCompress">Byte that needs compressing</param>
        /// <param name="n">number of bits on which the byte will be compressed</param>
        /// <returns>A n bits byte that has been compressed </returns>
        /// <exception cref="Exception">If n < 0</exception>
        public static byte CompressBits(byte toCompress, int n)
        {
            if (n < 0)
            {
                throw new Exception();
            }

            int maxValue = GetMaxForNBits(n);
            int compressedValue = toCompress * maxValue / 255;

            return (byte)compressedValue;
        }


        /// <summary>
        /// Decompress a byte of n bits into a byte of 8 bits
        /// </summary>
        /// <param name="toDecompress">n bits byte that has to be decompressed</param>
        /// <param name="n">size of toDecompress</param>
        /// <returns>an 8 bits byte</returns>
        /// <exception cref="Exception">If n < 0</exception>
        public static byte DecompressBits(byte toDecompress, int n)
        {
            if (n <= 0)
            {
                throw new Exception();
            }

            if (toDecompress > GetMaxForNBits(n))
                return toDecompress;

            byte maxValue = GetMaxForNBits(n);

            return (byte)(toDecompress * 255 / maxValue);
        }
    }
}