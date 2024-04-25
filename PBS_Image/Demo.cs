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
            myimage.save();
        }
    }
}