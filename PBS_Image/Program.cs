using System;
using System.Collections.Generic;
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
        }

        private static void test_save29()
        {
            MyImage image = new("ref_statue.bmp");
            Tools.Header header = new("/Save/Save29.bmp");
            //MyImage image2 = new MyImage("/Save/Save29.bmp");
            image.display();
            header.display();
        }
    }
}