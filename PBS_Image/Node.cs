using System;

namespace PBS_Image
{
    public class Node
    {
        public Node Left { get; set; }
        public Node Right { get; set; }
        public Pixel Value { get; set; }
        public int Frequency { get; set; }

        public Node(Pixel value, int frequency)
        {
            Value = value;
            Frequency = frequency;
        }

        public Node(Node left, Node right)
        {
            Left = left;
            Right = right;
            Frequency = left.Frequency + right.Frequency;
        }

        public bool IsLeaf()
        {
            return Left == null && Right == null;
        }
    }
}