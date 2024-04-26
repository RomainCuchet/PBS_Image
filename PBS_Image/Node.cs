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
        /// Create a node with a pixel and a frequency, and two potential children
        /// </summary>
        /// <param name="pixel">Pixel value</param>
        /// <param name="frequency">Frequency at which the specified Pixel appears in the image</param>
        /// <param name="left">Left child</param>
        /// <param name="right">Right child</param>
        public Node(Pixel pixel, int frequency, Node left = null, Node right = null)
        {
            Pixel = pixel;
            Frequency = frequency;
            Left = left;
            Right = right;
        }

        /// <summary>
        /// Creates a node with two children and a potential pixel
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

        /// <summary>
        /// Check if the node is a leaf
        /// </summary>
        /// <returns>True if the current Node has no childrer</returns>
        public bool IsLeaf()
        {
            return Left == null && Right == null;
        }
    }
}