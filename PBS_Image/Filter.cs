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
            {"edge1", new double[,] {{1,0,-1}, {0,0,0} , {-1,0,1} } },
            {"edge2", new double[,] {{0,1,0}, {1,-4,1} , {0,1,0} } },
            {"edge3", new double[,] {{-1,-1,-1}, {-1,8,-1} , {-1,-1,-1} } },
        };

        public static Pixel[,] filter(Pixel[,] image, string filter, bool basic)
        {

            if (filter == "grayscale") return grayscale(image);
            try
            {
                double[,] kernel = kernels[filter];
                if (basic || kernel.GetLength(0) != 3 || kernel.GetLength(1) != 3)
                {
                    return basic_kernel_filter(image, kernel);
                }
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
            if (color > 255) color = 255;
            if (color < 0) color = 0;
            return (byte)color;

        }

        public static Pixel[,] basic_kernel_filter(Pixel[,] image, double[,] kernel)
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
        }
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


        //code for Filter by convolution. Only works for 3x3 kernel.
        public static Pixel[,] kernel_filter(Pixel[,] image, double[,] kernel)
        {
            int height = image.GetLength(0);
            int width = image.GetLength(1);
            int verif;
            int start_row = 0;
            int end_row = 0;
            int start_column = 0;
            int end_column = 0;
            double blueX;
            double greenX;
            double redX;
            Pixel[,] image2 = new Pixel[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    image2[i, j] = new Pixel(image[i, j].red, image[i, j].green, image[i, j].blue);
                }
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    blueX = 0;
                    greenX = 0;
                    redX = 0;
                    verif = 0;
                    if (i == 0 && verif == 0)
                    {
                        verif = 1;
                        if (j == 0)
                        {
                            start_row = i;
                            end_row = i + 1;
                            start_column = j;
                            end_column = j + 1;
                        }
                        else if (j == width - 1)
                        {
                            start_row = i;
                            end_row = i + 1;
                            start_column = j - 1;
                            end_column = j;
                        }
                        else
                        {
                            start_row = i;
                            end_row = i + 1;
                            start_column = j - 1;
                            end_column = j + 1;
                        }
                    }
                    if (i == height - 1 && verif == 0)
                    {
                        verif = 1;
                        if (j == 0)
                        {
                            start_row = i - 1;
                            end_row = i;
                            start_column = j;
                            end_column = j + 1;
                        }
                        else if (j == width - 1)
                        {
                            start_row = i - 1;
                            end_row = i;
                            start_column = j - 1;
                            end_column = j;
                        }
                        else
                        {
                            start_row = i - 1;
                            end_row = i;
                            start_column = j - 1;
                            end_column = j + 1;
                        }
                    }
                    if (j == 0 && verif == 0)
                    {
                        verif = 1;
                        start_row = i - 1;
                        end_row = i + 1;
                        start_column = j;
                        end_column = j + 1;
                    }
                    if (j == width - 1 && verif == 0)
                    {
                        verif = 1;
                        start_row = i - 1;
                        end_row = i + 1;
                        start_column = j - 1;
                        end_column = j;
                    }
                    if (verif == 0)
                    {
                        start_row = i - 1;
                        end_row = i + 1;
                        start_column = j - 1;
                        end_column = j + 1;
                    }
                    for (int cpt = start_row; cpt <= end_row; cpt++)
                    {
                        for (int cpt2 = start_column; cpt2 <= end_column; cpt2++)
                        {
                            blueX += image2[cpt, cpt2].blue * kernel[cpt - i + 1, cpt2 - j + 1];
                            greenX += image2[cpt, cpt2].green * kernel[cpt - i + 1, cpt2 - j + 1];
                            redX += image2[cpt, cpt2].red * kernel[cpt - i + 1, cpt2 - j + 1];
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