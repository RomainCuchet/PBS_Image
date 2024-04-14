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
            MyImage image = new MyImage("coco.bmp");
            image.rotate(15, true).resize(1.3).save();
            image.filter("sharpness").save();
        }

        public static void demo_mandelbrot()
        {
           new Mandelbrot(2000,2000).create().save();
        }
    }
}