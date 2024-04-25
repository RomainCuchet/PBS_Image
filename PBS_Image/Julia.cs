using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBS_Image
{
    internal class Julia
    {
        // https://zestedesavoir.com/tutoriels/329/dessiner-la-fractale-de-mandelbrot/
        double x1 = -1;
        double x2 = 1;
        double y1 = -1.2;
        double y2 = 1.2;
        int nb_iteration;
        double treshold;
        double zoom_x;
        double zoom_y;
        int height;
        int width;
        Pixel default_p = new Pixel();
        Pixel default_color_p = new Pixel(255, 255, 255);
        double c_real;
        double c_imaginary;

        public Julia(int height, int width, double c_real, double c_imaginary, int nb_iteration = 150, double treshold = 4)
        {
            this.c_real = c_real;
            this.c_imaginary = c_imaginary;
            zoom_y = height / (y2 - y1);
            zoom_x = width / (x2 - x1);
            this.height = height;
            this.treshold = treshold;
            if (width % 4 == 0) this.width = width;
            else
            {
                this.width = width - width % 4;
            }

            this.nb_iteration = nb_iteration;
        }

        public MyImage create(bool color = true, byte c_red = 0, byte c_green = 0, byte c_blue = 255)
        {
            Pixel color_p = new Pixel(c_red, c_green, c_blue);
            Pixel[,] image = new Pixel[height, width];
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    Imaginaire z = new Imaginaire(x / zoom_x + x1, y / zoom_y + y1); // Translation vers le repère de l'ensemble de Julia
                    Imaginaire c = new Imaginaire(c_real, c_imaginary);
                    int i = 0;
                    do
                    {
                        z = z * z + c;
                        i++;
                    } while (z.module() < treshold && i < nb_iteration);
                    if (color)
                    {
                        if (i != nb_iteration) image[x, y] = new Pixel((byte)Math.Clamp(color_p.red * i / nb_iteration, (byte) Pixel.min, (byte) Pixel.max), (byte)Math.Clamp(color_p.green * i / nb_iteration, (byte) Pixel.min, (byte) Pixel.max), (byte)Math.Clamp(color_p.blue * i / nb_iteration, (byte)Pixel.min, (byte)Pixel.max));
                        else image[x, y] = default_p;
                    }
                    else
                    {
                        if (i == nb_iteration) image[x, y] = default_color_p;
                        else image[x, y] = default_p;
                    }
                }
            }
            return new MyImage(image);
        }
    }
}
