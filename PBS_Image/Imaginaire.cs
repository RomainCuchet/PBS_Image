using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBS_Image
{
    internal class Imaginaire
    {
        public double a;
        public double b;

        public Imaginaire(double a=0,double b=0)
        {
            this.a = a;
            this.b= b;
        }

        public double module ()
        {
            return Math.Sqrt(a*a+b*b);
        }

        public static Imaginaire operator + (Imaginaire c,Imaginaire c2)
        {
            return new Imaginaire(c.a+c2.a, c.b+c2.b);
        }

        public static Imaginaire operator -(Imaginaire c, Imaginaire c2)
        {
            return new Imaginaire(c.a - c2.a, c.b - c2.b);
        }

        public static bool operator ==(Imaginaire c, Imaginaire c2)
        {
            return c.a == c2.a && c.b == c2.b;
        }
        public static bool operator !=(Imaginaire c, Imaginaire c2)
        {
            return !(c.a == c2.a && c.b == c2.b);
        }
        public static Imaginaire operator * (Imaginaire c, Imaginaire c2)
        {
            return new Imaginaire(c.a * c2.a-c.b*c2.b,c.b*c2.a+c.a*c2.b);
        }
        public string ToString()
        {
            return $"{a}+i*{b}";
        }
    }
}
