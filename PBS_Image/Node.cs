using System;

namespace PBS_Image
{
    public class Node
    {
        public Node Left { get; set; }
        public Node Right { get; set; }
        public Pixel Pixel { get; set; }
        public int Frequency { get; set; }

        /// <summary>
        /// Crée un noeud à partir d'un pixel et d'une fréquence, éventuellement avec des enfants
        /// </summary>
        /// <param name="pixel"></param>
        /// <param name="frequency"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public Node(Pixel pixel, int frequency, Node left = null, Node right = null)
        {
            Pixel = pixel;
            Frequency = frequency;
            Left = left;
            Right = right;
        }

        /// <summary>
        /// Crée un noeud à partir de deux enfants et d'un pixel éventuellement 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="pixel"></param>
        public Node(Node left, Node right, Pixel pixel = null)
        {
            Left = left;
            Right = right;
            Frequency = left.Frequency + right.Frequency;
            Pixel = pixel;
        }

        public bool IsLeaf()
        {
            return Left == null && Right == null;
        }
    }
}