﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PBS_Image
{
    public class Pixel
    {
        public byte red;
        public byte green;
        public byte blue;
        public static int min = 0;
        public static int max = 255;
        public Pixel(byte red = 0, byte green = 0, byte blue = 0)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        public Pixel(Pixel p)
        {
            red = p.red;
            green = p.green;
            blue = p.blue;
        }

        public string Tostring()
        {
            return $"({red},{green},{blue})";
        }
        public byte[] toByte()
        {
            return new byte[] { blue, green, red };
        }

        public static Pixel operator * (Pixel p, int n)
        {
            return new Pixel((byte)Math.Clamp(p.red * n, min, max), (byte)Math.Clamp(p.green * n, min, max), (byte)Math.Clamp(p.blue * n, min, max));
        }
    }
}