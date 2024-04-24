using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TraitementImage
{
    internal class Conversion_JPEG
    {
        public double[,] image_JPEG_Y;
        public double[,] image_JPEG_Cb;
        public double[,] image_JPEG_Cr;
        public double[,][,] data_JPEG_Y;
        public double[,][,] data_JPEG_Cb;
        public double[,][,] data_JPEG_Cr;
        Pixel[,] result;
        int height;
        int width;
        int data_height;
        int data_width;

        double pi = Math.PI;

        double[,] quant_luminance = new double[,] 
        {
           { 16, 11, 10, 16, 24, 40, 51, 61 },
           { 12, 12, 14, 19, 26, 58, 60, 55 },
           { 14, 13, 16, 24, 40, 57, 69, 56 },
           { 14, 17, 22, 29, 51, 87, 80, 62 },
           { 18, 22, 37, 56, 68, 109, 103, 77 },
           { 24, 35, 55, 64, 81, 104, 113, 92 },
           { 49, 64, 78, 87, 103, 121, 120, 101 },
           { 72, 92, 95, 98, 112, 100, 103, 99 } 
        };

        double[,] quant_chrominance = new double[,] 
        { 
            { 17,18,24,47,99,99,99,99},
            {18,21,26,66,99,99,99,99},
            {24,26,56,99,99,99,99,99},
            {47,66,99,99,99,99,99,99},
            {99,99,99,99,99,99,99,99},
            {99,99,99,99,99,99,99,99},
            {99,99,99,99,99,99,99,99},
            {99,99,99,99,99,99,99,99} 
        };

        int[,] zigzag = new int[,]
        { 
            {0,1,5,6,14,15,27,28},
            {2,4,7,13,16,26,29,42},
            {3,8,12,17,25,30,41,43},
            {9,11,18,24,31,40,44,53},
            {10,19,23,32,39,45,52,54},
            {20,22,33,38,46,51,55,60},
            {21,34,37,47,50,56,59,61},
            {35,36,48,49,57,58,62,63}
        };

        

        public double[,][,] init_data_JPEG(int hauteur, int largeur, int haut_mat, int larg_mat)
        {
            double[,][,] ret_JPEG = new double[hauteur, largeur][,];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    ret_JPEG[i, j] = new double[haut_mat, larg_mat];
                }
            }
            return ret_JPEG;
        }

        public double[,][,] init_mat_bloc(int hauteur, int largeur, int haut_mat, int larg_mat)
        {
            double[,][,] ret_JPEG = new double[hauteur, largeur][,];
            for (int k = 0; k < hauteur; k++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    ret_JPEG[k, j] = new double[haut_mat, larg_mat];
                    
                }
            }
            return ret_JPEG;
        }

        public Conversion_JPEG(MyImage mi)
        {
            height = mi.Image.GetLength(0);
            width = mi.Image.GetLength(1);
            image_JPEG_Y = new double[height, width];
            image_JPEG_Cb = new double[height, width];
            image_JPEG_Cr = new double[height, width];
            Color_Transformation_x_Echantillonage(mi);
            Decoupage();
            Console.WriteLine("fait");
            DCT();
            codage_zigzag(zigzag);
            int cpt = 0;
            for (int i =0; i<data_height; i++)
            {
                for (int j = 0;j< data_width; j++)
                {
                    for( int k =0; k < 8; k++)
                    {
                        for(int l=0; l< 8; l++)
                        {
                            Console.Write((data_JPEG_Y[i, j][k, l]) + " , ");
                            cpt++;
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine(cpt);


        }

        public void Color_Transformation_x_Echantillonage(MyImage mi)
        {
            double[] pixel_y = new double[4];
            double[] pixel_cb = new double[4];
            double[] pixel_cr = new double[4];
            double pixel_cb_sum;
            double pixel_cr_sum;
            for (int i = 0; i < height; i = i + 2)
            {
                for (int j = 0; j < width; j = j + 2)
                {
                    pixel_y[0] = 0.299 * mi.Image[i, j].Red + 0.087 * mi.Image[i, j].Green + 0.114 * mi.Image[i, j].Blue;
                    pixel_cb[0] = -0.1687 * mi.Image[i, j].Red - 0.3313 * mi.Image[i, j].Green + 0.5 * mi.Image[i, j].Blue + 128;
                    pixel_cr[0] = 0.5 * mi.Image[i, j].Red - 0.4187 * mi.Image[i, j].Green - 0.0813 * mi.Image[i, j].Blue + 128;

                    if (i == height - 1)
                    {
                        if (j != width - 1)
                        {
                            pixel_y[1] = (0.299 * mi.Image[i, j + 1].Red + 0.087 * mi.Image[i, j + 1].Green + 0.114 * mi.Image[i, j + 1].Blue);
                            pixel_cb[1] = -0.1687 * mi.Image[i, j + 1].Red - 0.3313 * mi.Image[i, j + 1].Green + 0.5 * mi.Image[i, j + 1].Blue + 128;
                            pixel_cr[1] = 0.5 * mi.Image[i, j + 1].Red - 0.4187 * mi.Image[i, j + 1].Green - 0.0813 * mi.Image[i, j + 1].Blue + 128;

                            pixel_cb_sum = (pixel_cb[0] + pixel_cb[1]) / 2;
                            pixel_cr_sum = (pixel_cr[0] + pixel_cr[1]) / 2;

                            image_JPEG_Y[i, j] = pixel_y[0];
                            image_JPEG_Y[i, j + 1] = pixel_y[1];

                            image_JPEG_Cb[i, j] = pixel_cb_sum;
                            image_JPEG_Cb[i, j + 1] = pixel_cb_sum;

                            image_JPEG_Cr[i, j] = pixel_cr_sum;
                            image_JPEG_Cr[i, j + 1] = pixel_cr_sum;
                        }
                    }
                    else if (j == width - 1)
                    {
                        pixel_y[1] = 0.299 * mi.Image[i + 1, j].Red + 0.087 * mi.Image[i + 1, j].Green + 0.114 * mi.Image[i + 1, j].Blue;

                        pixel_cb[1] = -0.1687 * mi.Image[i + 1, j].Red - 0.3313 * mi.Image[i + 1, j].Green + 0.5 * mi.Image[i + 1, j].Blue + 128;
                        pixel_cr[1] = 0.5 * mi.Image[i + 1, j].Red - 0.4187 * mi.Image[i + 1, j].Green - 0.0813 * mi.Image[i + 1, j].Blue + 128;

                        pixel_cb_sum = (pixel_cb[0] + pixel_cb[1]) / 2;
                        pixel_cr_sum = (pixel_cr[0] + pixel_cr[1]) / 2;

                        image_JPEG_Y[i, j] = pixel_y[0];
                        image_JPEG_Y[i+1, j ] = pixel_y[1];

                        image_JPEG_Cb[i, j] = pixel_cb_sum;
                        image_JPEG_Cb[i+1, j ] = pixel_cb_sum;

                        image_JPEG_Cr[i, j] = pixel_cr_sum;
                        image_JPEG_Cr[i+1, j] = pixel_cr_sum;
                    }
                    else
                    {
                        pixel_y[1] = 0.299 * mi.Image[i, j + 1].Red + 0.087 * mi.Image[i, j + 1].Green + 0.114 * mi.Image[i, j + 1].Blue;
                        pixel_y[2] = 0.299 * mi.Image[i + 1, j].Red + 0.087 * mi.Image[i + 1, j].Green + 0.114 * mi.Image[i + 1, j].Blue;
                        pixel_y[3] = 0.299 * mi.Image[i + 1, j + 1].Red + 0.087 * mi.Image[i + 1, j + 1].Green + 0.114 * mi.Image[i + 1, j + 1].Blue;

                        pixel_cb[1] = -0.1687 * mi.Image[i, j + 1].Red - 0.3313 * mi.Image[i, j + 1].Green + 0.5 * mi.Image[i, j + 1].Blue + 128;
                        pixel_cb[2] = -0.1687 * mi.Image[i + 1, j].Red - 0.3313 * mi.Image[i + 1, j].Green + 0.5 * mi.Image[i + 1, j].Blue + 128;
                        pixel_cb[3] = -0.1687 * mi.Image[i + 1, j + 1].Red - 0.3313 * mi.Image[i + 1, j + 1].Green + 0.5 * mi.Image[i + 1, j + 1].Blue + 128;

                        pixel_cr[1] = 0.5 * mi.Image[i, j + 1].Red - 0.4187 * mi.Image[i, j + 1].Green - 0.0813 * mi.Image[i, j + 1].Blue + 128;
                        pixel_cr[2] = 0.5 * mi.Image[i + 1, j].Red - 0.4187 * mi.Image[i + 1, j].Green - 0.0813 * mi.Image[i + 1, j].Blue + 128;
                        pixel_cr[3] = 0.5 * mi.Image[i + 1, j + 1].Red - 0.4187 * mi.Image[i + 1, j + 1].Green - 0.0813 * mi.Image[i + 1, j + 1].Blue + 128;

                        pixel_cb_sum = (pixel_cb[0] + pixel_cb[1] + pixel_cb[2] + pixel_cb[3]) / 4;
                        pixel_cr_sum = (pixel_cr[0] + pixel_cr[1] + pixel_cr[2] + pixel_cr[3]) / 4;

                        image_JPEG_Y[i, j] = pixel_y[0];
                        image_JPEG_Y[i, j + 1] = pixel_y[1];
                        image_JPEG_Y[i+1,j] = pixel_y[2];
                        image_JPEG_Y[i + 1, j + 1] = pixel_y[3];

                        image_JPEG_Cb[i, j] = pixel_cb_sum;
                        image_JPEG_Cb[i, j + 1] = pixel_cb_sum;
                        image_JPEG_Cb[i+1,j]= pixel_cb_sum;
                        image_JPEG_Cb[i + 1,j+1] = pixel_cb_sum;

                        image_JPEG_Cr[i, j] = pixel_cr_sum;
                        image_JPEG_Cr[i, j + 1] = pixel_cr_sum;
                        image_JPEG_Cr[i + 1, j] = pixel_cr_sum;
                        image_JPEG_Cr[i+1,j+1] = pixel_cr_sum;
                    }
                }
            }
        }

        public void Decoupage()
        {
            if (height % 8 == 0) data_height = height / 8;
            else data_height = height / 8 + 1;
            if (width % 8 == 0) data_width = width / 8;
            else data_width = width / 8 + 1;

            data_JPEG_Y = new double[data_height, data_width][,];
            data_JPEG_Cb = new double[data_height, data_width][,];
            data_JPEG_Cr = new double[data_height, data_width][,];

            for (int i = 0; i < data_height; i++)
            {
                for (int j = 0; j < data_width; j++)
                {
                    data_JPEG_Y[i,j] = new double[8, 8];
                    data_JPEG_Cb[i, j] = new double[8, 8];
                    data_JPEG_Cr[i, j] = new double[8, 8];
                    for (int k = 0; k < 8; k = k + 2)
                    {
                        for (int l = 0; l < 8; l = l + 2)
                        {
                            if ((k + i) < height && (j + l) < width)
                            {
                                data_JPEG_Y[i, j][k, l] = image_JPEG_Y[i+k,j+ l];
                                data_JPEG_Cb[i, j][k, l] = image_JPEG_Cb[i+k,j+ l];
                                data_JPEG_Cr[i, j][k, l] = image_JPEG_Cr[i + k, j + l];
                                if ((k + i) != height - 1)
                                {
                                    data_JPEG_Y[i, j][k+1, l] = image_JPEG_Y[i + k + 1, j + l];
                                    data_JPEG_Cb[i, j][k+1, l] = image_JPEG_Cb[i + k + 1, j + l];
                                    data_JPEG_Cr[i, j][k+1, l] = image_JPEG_Cr[i + k + 1, j + l];
                                    if ((j + k) != width - 1)
                                    {
                                        data_JPEG_Y[i, j][k+1, l+1] = image_JPEG_Y[i + k+1, j + l+1];
                                        data_JPEG_Cb[i, j][k+1, l+1] = image_JPEG_Cb[i + k+1, j + l+1];
                                        data_JPEG_Cr[i, j][k+1, l+1] = image_JPEG_Cr[i + k+1, j + l+1];
                                    }
                                    else 
                                    {
                                        data_JPEG_Y[i, j][k + 1, l + 1] =0;
                                        data_JPEG_Cb[i, j][k + 1, l + 1] = 128;
                                        data_JPEG_Cr[i, j][k + 1, l + 1] = 128;
                                    }
                                }
                                else
                                {
                                    data_JPEG_Y[i, j][k + 1, l] = 0;
                                    data_JPEG_Cb[i, j][k + 1, l] = 128;
                                    data_JPEG_Cr[i, j][k + 1, l] = 128;
                                }
                                if ((j + k) != width - 1)
                                {
                                    data_JPEG_Y[i, j][k, l + 1] = image_JPEG_Y[i + k, j + l + 1];
                                    data_JPEG_Cb[i, j][k, l + 1] = image_JPEG_Cb[i + k, j + l + 1];
                                    data_JPEG_Cr[i, j][k, l + 1] = image_JPEG_Cr[i + k, j + l + 1];
                                }
                                else
                                {
                                    data_JPEG_Y[i, j][k, l+1] = 0;
                                    data_JPEG_Cb[i, j][k, l+1] = 128;
                                    data_JPEG_Cr[i, j][k, l+1] = 128;
                                }
                            }
                            else
                            {
                                for(int x=k;x<k+2;x++)
                                {
                                    for(int y=j;y<2;y++)
                                    {
                                        data_JPEG_Y[i, j][x, y] = 0;
                                        data_JPEG_Cb[i, j][x, y] = 128;
                                        data_JPEG_Cr[i, j][x, y] = 128;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DCT()
        {
            double ck;
            double cl;
            double somme_Y = 0;
            double somme_Cb = 0;
            double somme_Cr = 0;
            double[,][,] res_Y = init_mat_bloc(data_height, data_width, 8, 8);
            double[,][,] res_Cb = init_mat_bloc(data_height, data_width, 8, 8);
            double[,][,] res_Cr = init_mat_bloc(data_height, data_width, 8, 8);

            for (int i = 0; i < data_height; i += 8)
            {
                for (int j = 0; j < data_width; j += 8)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        for (int l = 0; l < 8; l++)
                        {
                            somme_Y = 0.0;
                            somme_Cb = 0;
                            somme_Cr = 0;

                            if (l == 0) cl = 1.0 / Math.Sqrt(2);
                            else cl = 1.0;
                            if (k == 0) ck = 1.0 / Math.Sqrt(2);
                            else ck = 1.0;

                            for (int x = 0; x < 8; x++)
                            {
                                for (int y = 0; y < 8; y++)
                                {
                                    somme_Y += data_JPEG_Y[i, j][x, y] * Math.Cos((2 * x + 1) * pi * k / 16) * Math.Cos((2 * y + 1) * pi * l / 16);
                                    somme_Cb += data_JPEG_Cb[i, j][x, y] * Math.Cos((2 * x + 1) * pi * k / 16) * Math.Cos((2 * y + 1) * pi * l / 16);
                                    somme_Cr += data_JPEG_Cr[i, j][x, y] * Math.Cos((2 * x + 1) * pi * k / 16) * Math.Cos((2 * y + 1) * pi * l / 16);
                                }
                            }

                            res_Y[i, j][k, l] = 0.25 * cl * ck * somme_Y / quant_luminance[k, l];
                            res_Cb[i, j][k, l] = 0.25 * cl * ck * somme_Cb / quant_chrominance[k, l];
                            res_Cr[i, j][k, l] = 0.25 * cl * ck * somme_Cr / quant_chrominance[k, l];
                            

                        }
                    }
                }
            }
            data_JPEG_Y = res_Y;
            data_JPEG_Cb = res_Cb;
            data_JPEG_Cr = res_Cr;
        }
        public void codage_zigzag(int[,] mat_zigzag)
        {
            result = new Pixel[data_height * 8, data_width * 8];
            int index_i;
            int index_j;
            for (int i = 0; i < data_height; i += 8)
            {
                for (int j = 0; j < data_width; j += 8)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        for (int l = 0; l < 8; l++)
                        {
                            index_j = mat_zigzag[k, l] % 8;
                            index_i = mat_zigzag[k, l] / 8;
                            result[i + index_i, j + index_j] = new Pixel((byte)(data_JPEG_Y[i, j][k, l]), (byte)(data_JPEG_Cb[i, j][k, l]), (byte)(data_JPEG_Cr[i, j][k, l]));
                        }
                    }
                }
            }

        }

    }


}
