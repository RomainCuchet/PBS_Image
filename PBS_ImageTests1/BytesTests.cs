using Microsoft.VisualStudio.TestTools.UnitTesting;
using PBS_Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBS_Image.Tests
{
    [TestClass()]
    public class BytesTests
    {
        [TestMethod()]
        [DataRow(0, (byte)0)]
        [DataRow(1, (byte)1)]
        [DataRow(4, (byte)15)]
        [DataRow(8, (byte)255)]
        public void GetMaxForNBitsTest(int n, byte expected)
        {
            byte actual = Bytes.GetMaxForNBits(n);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetMaxForNBitsTest_Negative()
        {
            Assert.ThrowsException<Exception>(() => Bytes.GetMaxForNBits(-1));
        }

        [TestMethod()]
        [DataRow((byte)0, 0, (byte)0)]
        [DataRow((byte)0b1111111, 4, (byte)0b1111)]
        [DataRow((byte)0b1110101, 4, (byte)0b0101)]
        public void GetLeastSignificantBitsTest(byte toget, int n, byte expected)
        {
            byte actual = Bytes.GetLeastSignificantBits(toget, n);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        [DataRow((byte)0, 4, (byte)0)]
        [DataRow((byte)255, 4, (byte)15)]
        [DataRow((byte)255, 3, (byte)7)]
        [DataRow((byte)42, 4, (byte)2)]
        public void CompressBitsTests(byte toCompress, int n, byte expected)
        {
            byte actual = Bytes.CompressBits(toCompress, n);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        [DataRow((byte)0, 4, (byte)0)]
        [DataRow((byte)15, 4, (byte)255)]
        [DataRow((byte)7, 3, (byte)255)]
        public void DeCompressBitsTests(byte toDecompress, int n, byte expected)
        {
            byte actual = Bytes.DecompressBits(toDecompress, n);
            Assert.AreEqual(expected, actual);
        }


    }
}