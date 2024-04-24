using PBS_Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBS_Image
{
    internal class Demo
    {
        public static void demoTD_34()
        {
            MyImage image = new MyImage("ref_statue.bmp");
            image.rotate(15, true).resize(1.3).save();
            image.filter("sharpness").save();
        }

        public static void mandelbrot()
        {
           new Mandelbrot(3000,3000).create().save();
        }
    }
}