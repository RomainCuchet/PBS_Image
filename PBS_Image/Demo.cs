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
            image.rotate(15, true).resize(1.3).Save();
            image.filter("edge1").Save();
        }

        public static void demo_mandelbrot()
        {
           new Mandelbrot(2000,2000).create().Save();
        }

        public static void demo_stegano()
        {
            MyImage toHide = new MyImage("ref_hideItInStatueOn4Bits.bmp");
            MyImage carrier = new MyImage("ref_statue.bmp");
            carrier = carrier.resize(2);

            carrier.HideImage(4, 0b111, toHide);
            carrier.Save();

            MyImage hidden = carrier.GetHiddenImage(4, 0b111);
            hidden.Save();

            Console.WriteLine(toHide.Tostring());
        }
        
        public static void demo_huffman()
        {
            MyImage myimage = new MyImage("coco.bmp");
            var freq = Tree.BuildFrequencyDictionary(myimage.image);
            Node root = Tree.BuildTree(freq);
            Tree tree = new Tree(root, freq);
            Dictionary<Pixel, string> encodingTable = tree.BuildEncodingTable(root,"", new Dictionary<Pixel, string>());
            string encoded = tree.Encode(myimage.image, encodingTable);
            //Console.WriteLine(tree.Frequencies.Count);
            //Console.WriteLine(Tree.TreeToString(tree.Root, ""));
            //Console.WriteLine(encoded);
            myimage.image = tree.Decode(encoded, myimage.width, myimage.height);
            myimage.Save();
        }
    }
}