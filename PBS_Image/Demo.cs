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
            image.filter("edge1").save();
        }

        public static void demo_mandelbrot()
        {
           new Mandelbrot(2000,2000).create().save();
        }

        public static void demo_stegano()
        {
            MyImage toHide = new MyImage(@"ref_stegano\ref_hideItInStatueOn4Bits.bmp");
            MyImage carrier = new MyImage(@"ref_stegano\ref_statue.bmp");

            carrier.HideImage(4, 0b111, toHide);
            carrier.save();

            MyImage hidden = carrier.GetHiddenImage(4, 0b111);
            hidden.save();

            Console.WriteLine(toHide.Tostring());
        }
        
    }
}