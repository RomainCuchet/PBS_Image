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

        public static void mandelbrot()
        {
            new Mandelbrot(2000, 2000).create().save();
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

            carrier.HideImage(toHide);
            carrier.save();

            MyImage hidden = carrier.GetHiddenImage();
            hidden.save();

        }

        public static void demo_huffman()
        {
            MyImage myimage = new("../../../Images/Default/Test.bmp");
            var freq = Tree.BuildFrequencyDictionary(myimage.image);
            Node root = Tree.BuildTree(freq);
            Tree tree = new Tree(root, freq);
            Dictionary<Pixel, string> encodingTable = tree.BuildEncodingTable(root, "", new Dictionary<Pixel, string>());
            string encoded = tree.Encode(myimage.image, encodingTable);

            Console.WriteLine(tree.Frequencies.Count);
            Console.WriteLine(Tree.TreeToString(tree.Root, ""));
            Console.WriteLine(encoded);
            Console.WriteLine($"Header:\n{Tree.StructureDHTHeader(tree.Root).ToString()}");

            myimage.image = tree.Decode(encoded, myimage.width, myimage.height);
            myimage.save(file_name:"huffman",random_name: false);
        }

        public static void demo_conversion_jpeg() //On ne sait pas si le résultat est viable parceque pas réussi sauvegarde JPEG.
        {
            Conversion_JPEG jj = new Conversion_JPEG(new MyImage("../../../Images/Default/coco.bmp"));
            int cpt = 0;
            for (int i = 0; i < jj.data_height; i++)
            {
                for (int j = 0; j < jj.data_width; j++)
                {
                    for (int k = 0; k < 8;k++)
                    {
                        for(int l =0; l< 8;l++)
                        {
                            cpt++;
                            Console.Write(jj.result_Cb[i*8 + k, j*8 + l] +" , "); //Peut montrer Y ou Cr. Juste changer la matrice
                        }Console.WriteLine();
                    }Console.WriteLine();
                }
            }
            Console.WriteLine(cpt); //Montre le nombre de pixels parcourus
        }
    }
}
