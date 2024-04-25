using System.Drawing;

namespace PBS_Image
{
    public class SizeException : Exception
    {
        public SizeException(string message) : base(message) { }
    }
    public class NoChannelException : Exception
    {
        public NoChannelException(string message) : base(message) { }
    }
}