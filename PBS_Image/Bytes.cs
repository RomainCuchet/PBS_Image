using System;

namespace PBS_Image
{

    public static class Bytes
    {
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
        /*public static byte CompressBits(byte toCompress, int n)
        {
            if (n < 0)
                throw new ForbiddenValueException();
            else
            {
                if (n == 0)
                    return 0;
                byte max = GetMaxForNBits(n) ;
                if (max <= toCompress)
                    return toCompress;
                else
                {
                    byte val = (byte)((toCompress * max) / 255);
                    return val;
                }
            }
        }*/

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

        /*public static byte DecompressBits(byte toDecompress, int n)
        {

            if (n <= 0)
                throw new ForbiddenValueException();
            else
            {
                byte max = GetMaxForNBits(n);
                byte val = (byte)((toDecompress * 255) / max);
                return val;
            }
        }*/

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