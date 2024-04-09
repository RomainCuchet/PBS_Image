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
            image.rotate(15, true).resize(1.3).Save();
            image.filter("sharpness").Save();
        }

        public static void compare_kernel_filter()
        {
            MyImage image = new MyImage("coco.bmp");
            image.filter("box blur").Save();
            image.filter("box blur", true).Save();
            image.filter("sharpness").Save();
            image.filter("sharpness", true).Save();
        }
        public static void t1()
        {
            MyImage image = new MyImage("coco.bmp");
            image = image.rotate(1, true, true);
            image.Save();
        }
        public static void t2()
        {
            MyImage image = new MyImage("coco.bmp");
            image = image.resize(1.11);
            image.Save();
        }
        public static void compare()
        {
            Tools.PrintInfo("../../Images/", "coco.bmp");
            Tools.PrintInfo("../../Images/Save/", $"Save{Tools.get_counter("../../counter.txt", false)}.bmp");
            MyImage i1 = new MyImage("coco.bmp");
            MyImage i2 = new MyImage($"Save{Tools.get_counter("../../counter.txt", false)}.bmp", "../../Images/Save/");
            Console.WriteLine(i1.Tostring());
            Console.WriteLine();
            Console.WriteLine(i2.Tostring());
        }
    }
}