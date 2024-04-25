using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PBS_Image
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Demo.demoTD_34();
            //Demo.demo_mandelbrot();
            Demo.demo_stegano(1, "ref_statue.bmp", "coco.bmp");
            //Demo.demo_huffman();
        }
    }
}