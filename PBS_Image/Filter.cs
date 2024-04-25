using PBS_Image;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PBS_Image
{
    internal class Filter
    {

        static Dictionary<string, double[,]> kernels = new Dictionary<string, double[,]>()
        {

            {"sharpness", new double[,] { { 0,-1,0}, {-1,5,-1 }, {0,-1,0 } } },
            {"box blur", new double[,] {{1.0/9,1.0/9,1.0/9}, {1.0/9,1.0/9,1.0/9} , {1.0/9,1.0/9,1.0/9} } }, // c# division of two inteers return an integer hence we use 1.0 instead of 1
            {"embossing", new double[,] { { -1, -1, 0 }, { -1, 0, 1 }, { 0, 1, 1 } }},
            {"edge1", new double[,] {{1,0,-1}, {0,0,0} , {-1,0,1} } },
            {"edge2", new double[,] {{0,1,0}, {1,-4,1} , {0,1,0} } },
            {"edge3", new double[,] {{-1,-1,-1}, {-1,8,-1} , {-1,-1,-1} } },
        };

        public static Pixel[,] filter(Pixel[,] image, string filter, bool basic)
        {

            if (filter == "grayscale") return grayscale(image);
            if (filter == "reflect") return reflect(image);
            try
            {
                double[,] kernel = kernels[filter];
                /*if (basic || kernel.GetLength(0) != 3 || kernel.GetLength(1) != 3)
                {
                    return basic_kernel_filter(image, kernel);
                }*/
                return kernel_filter(image, kernel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return image;
            }
        }

        public static byte get_color(double color)
        {
            return (byte)Math.Clamp(color, Pixel.min, Pixel.max);

        }

        /*public static Pixel[,] basic_kernel_filter(Pixel[,] image, double[,] kernel)
        {
            int width = image.GetLength(1);
            int height = image.GetLength(0);
            Pixel[,] filter_image = new Pixel[height, width];
            double new_r;
            double new_g;
            double new_b;
            int k_width = kernel.GetLength(0);
            int k_height = kernel.GetLength(1);
            int k_icenter = k_width / 2;
            int k_jcenter = k_height / 2;
            int oi; // Offset i
            int oj; // Offset j
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    new_r = 0;
                    new_g = 0;
                    new_b = 0;
                    for (int i_k = 0; i_k < k_height; i_k++)
                    {
                        for (int j_k = 0; j_k < k_width; j_k++)
                        {
                            oi = i + i_k - k_icenter;
                            oj = j + j_k - k_jcenter;
                            if (oj >= 0 && oj < width && oi >= 0 && oi < height)
                            {
                                new_r += image[oi, oj].red * kernel[i_k, j_k];
                                new_g += image[oi, oj].green * kernel[i_k, j_k];
                                new_b += image[oi, oj].blue * kernel[i_k, j_k];
                            }
                        }
                    }
                    filter_image[i, j] = new Pixel(get_color(new_r), get_color(new_g), get_color(new_b));
                }
            }
            return filter_image;
        }*/
        
        // Convert image to grayscale
        public static Pixel[,] grayscale(Pixel[,] image)
        {
            int height = image.GetLength(0);
            int width = image.GetLength(1);
            int average;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    average = image[i, j].red + image[i, j].green + image[i, j].blue;
                    if (average % 3 == 2)
                    {
                        average = average / 3 + 1;
                    }
                    else average = average / 3;
                    image[i, j].blue = (byte)average;
                    image[i, j].green = (byte)average;
                    image[i, j].red = (byte)average;
                }
            }
            return image;
        }

        // Reflect image horizontally
        public static Pixel[,] reflect(Pixel[,] image)
        {
            int height = image.GetLength(0);
            int width = image.GetLength(1);
            byte Blue;
            byte Green;
            byte Red;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width / 2; j++)
                {
                    Blue = image[i, j].blue;
                    Green = image[i, j].green;
                    Red = image[i, j].red;
                    image[i, j].blue = image[i, width - 1 - j].blue;
                    image[i, j].green = image[i, width - 1 - j].green;
                    image[i, j].red = image[i, width - 1 - j].red;
                    image[i, width - 1 - j].blue = Blue;
                    image[i, width - 1 - j].green = Green;
                    image[i, width - 1 - j].red = Red;
                }
            }
            return image;
        }

        //code for Filter by convolution. Only works for 3x3 kernel.
        public static Pixel[,] kernel_filter(Pixel[,] image, double[,] kernel)
        {
            int heigth = image.GetLength(0);
            int width = image.GetLength(1);
            int start_row = 0;
            int end_row = 0;
            int start_column = 0;
            int end_column = 0;
            double blueX;
            double greenX;
            double redX;
            Pixel[,] image2 = new Pixel[heigth, width];
            for (int i = 0; i < heigth; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    image2[i, j] = new Pixel(image[i, j].red, image[i, j].green, image[i, j].blue);
                }
            }
            for (int i = 0; i < heigth; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    blueX = 0;
                    greenX = 0;
                    redX = 0;
                    if (i - kernel.GetLength(0) / 2 < 0) start_row = 0; else start_row = i - kernel.GetLength(0) / 2;
                    if (i + kernel.GetLength(0) / 2 >= heigth) end_row = heigth - 1; else end_row = i + kernel.GetLength(0) / 2;
                    if (j - kernel.GetLength(1) / 2 < 0) start_column = 0; else start_column = j - kernel.GetLength(1) / 2;
                    if (j + kernel.GetLength(1) / 2 >= width) end_column = width - 1; else end_column = j + kernel.GetLength(0) / 2;

                    for (int m = start_row; m <= end_row; m++)
                    {
                        for (int n = start_column; n <= end_column; n++)
                        {
                            blueX += image2[m, n].blue * kernel[m - (i - kernel.GetLength(0) / 2), n - (j - kernel.GetLength(1) / 2)];
                            greenX += image2[m, n].green * kernel[m - (i - kernel.GetLength(0) / 2), n - (j - kernel.GetLength(1) / 2)];
                            redX += image2[m, n].red * kernel[m - (i - kernel.GetLength(0) / 2), n - (j - kernel.GetLength(1) / 2)];
                        }
                    }
                    image[i, j].blue = get_color(blueX);
                    image[i, j].green = get_color(greenX);
                    image[i, j].red = get_color(redX);
                }
            }
            return image;
        }
    }
}
