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
            new Mandelbrot(2000, 2000).create().Save();
        }

        /// <summary>
        /// Demo de la stéganographie
        /// </summary>
        /// <param name="scale">Paramètre d'agrandissement de l'image de support: permet de démontrer la fonctionnalité pluri taille de l'image</param>
        /// <param name="carrierImage">Image de support</param>
        /// <param name="toHideImage">Image à cacher</param>
        public static void demo_stegano(double scale = 1, string carrierImage = "ref_statue.bmp", string toHideImage = "ref_hideItInStatueOn4Bits.bmp")
        {
            MyImage toHide = new MyImage(toHideImage);
            MyImage carrier = new MyImage(carrierImage);
            carrier = carrier.resize(scale);

            carrier.HideImage(4, 0b111, [toHide]);
            carrier.Save();

            (MyImage hidden1, MyImage hidden2, MyImage hidden3) = carrier.GetHiddenImage(4, 0b111, false);
            hidden1.Save();
            hidden2.Save();
            hidden3.Save();

        }

        public static void demo_huffman()
        {
            MyImage myimage = new MyImage("Test.bmp");
            var freq = Tree.BuildFrequencyDictionary(myimage.image);
            Node root = Tree.BuildTree(freq);
            Tree tree = new Tree(root, freq);
            Dictionary<Pixel, string> encodingTable = tree.BuildEncodingTable(root, "", new Dictionary<Pixel, string>());
            string encoded = tree.Encode(myimage.image, encodingTable);

            Console.WriteLine(tree.Frequencies.Count);
            Console.WriteLine(Tree.TreeToString(tree.Root, ""));
            Console.WriteLine(encoded);
            //Console.WriteLine($"Header:\n{Tree.StructureDHTHeader(tree.Root).ToString()}");

            myimage.image = tree.Decode(encoded, myimage.width, myimage.height);
            myimage.Save();
        }
    }
}