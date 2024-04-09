using PBS_Image;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PBS_Image
{
    internal class Modification
    {
        /// <summary>
        /// rotation with the basic angles 90,180,270
        /// </summary>
        /// <param name="myimage">Myimage instance to rotate</param>
        /// <param name="angle">rotation angle, will corespond to one of the basic angles</param>
        /// <returns></returns>
        public static MyImage basic_rotate_image(MyImage myimage, int angle = 90)
        {
            if (angle <= 0 || angle > 170) return myimage;

            MyImage rotatedImage = new MyImage(myimage);
            int rows = myimage.height;
            int cols = myimage.width;

            if (angle <= 90)
            {
                {

                    rotatedImage = new MyImage(myimage);
                    rotatedImage.image = new Pixel[cols, rows];
                    rotatedImage.width += rotatedImage.height;
                    rotatedImage.height = rotatedImage.width - rotatedImage.height;
                    rotatedImage.width -= rotatedImage.height;
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            rotatedImage.image[j, rows - 1 - i] = myimage.image[i, cols - 1 - j];
                        }
                    }
                }
            }
            else if (angle <= 180)
            {
                for (int i = 0; i < myimage.image.GetLength(0); i++)
                {
                    for (int j = 0; j < myimage.image.GetLength(1); j++)
                    {
                        rotatedImage.image[i, j] = myimage.image[i, cols - 1 - j];
                    }
                }
            }
            else if (angle <= 270)
            {

                rotatedImage = new MyImage(myimage);
                rotatedImage.image = new Pixel[cols, rows];
                rotatedImage.width += rotatedImage.height;
                rotatedImage.height = rotatedImage.width - rotatedImage.height;
                rotatedImage.width -= rotatedImage.height;
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        rotatedImage.image[j, rows - 1 - i] = myimage.image[i, j];
                    }
                }
            }

            return rotatedImage;
        }

        /// <summary>
        /// resize the image with the given factor
        /// </summary>
        /// <param name="myimage">Myimage instance to extend</param>
        /// <param name="factor">extend's factor</param>
        /// <returns></returns>
        public static MyImage resize(MyImage myimage, double factor = 2)
        {
            if (factor <= 0)
            {
                Console.WriteLine("tried to resize an image with a negative or null factor");
                return myimage;
            }
            MyImage extendedImage = new MyImage(myimage);
            extendedImage.width = (int)(extendedImage.width * factor);
            extendedImage.width -= extendedImage.width % 4; // must be multiple of 4
            extendedImage.height = (int)(extendedImage.height * factor);
            extendedImage.image = new Pixel[extendedImage.height, extendedImage.width];
            extendedImage.taille_image = extendedImage.height * extendedImage.width * 3; // one piwel is composed of 3 byte
            for (int i = 0; i < extendedImage.height; i++)
            {
                for (int j = 0; j < extendedImage.width; j++)
                {
                    extendedImage.image[i, j] = myimage.image[(int)(i / factor), (int)(j / factor)];
                }
            }
            return extendedImage;
        }
        public static MyImage rotate(MyImage image, double angle, bool interpo, bool optimal_dim)
        {
            if (angle == 0 || angle == 90 || angle == 180 || angle == 270) return basic_rotate_image(image, (int)angle);
            angle = angle * Math.PI / 180.0;
            int rotatedWidth;
            int rotatedHeight;
            if (optimal_dim) (rotatedHeight, rotatedWidth) = get_optimal_dim(image, angle);
            else
            {
                rotatedWidth = image.width * 2;
                rotatedHeight = image.height * 2;
            }
            rotatedWidth -= rotatedWidth % 4;
            MyImage rotatedImage = new MyImage(image);
            rotatedImage.width = rotatedWidth;
            rotatedImage.height = rotatedHeight;
            rotatedImage.taille_image = rotatedImage.height * rotatedImage.width * 3;
            rotatedImage.image = new Pixel[rotatedImage.height, rotatedImage.width];


            // compute offset to center the image in the new matrixg
            int offsetX = (rotatedHeight - image.height) / 2;
            int offsetY = (rotatedWidth - image.width) / 2;

            for (int x = 0; x < rotatedHeight; x++)
            {
                for (int y = 0; y < rotatedWidth; y++)
                {

                    // convert polar coordinate into carthesian's one of the initial image
                    int oX = (int)(Math.Cos(angle) * (x - offsetX - image.height / 2) + Math.Sin(angle) * (y - offsetY - image.width / 2) + image.height / 2);
                    int oY = (int)(-Math.Sin(angle) * (x - offsetX - image.height / 2) + Math.Cos(angle) * (y - offsetY - image.width / 2) + image.width / 2);
                    if (oX >= 0 && oX < image.height && oY >= 0 && oY < image.width)
                    {
                        if (interpo) rotatedImage.image[x, y] = Tools.interpolation(image.image, oY, oX);
                        else rotatedImage.image[x, y] = image.image[oX, oY];
                    }

                }
            }

            // set the remaining pixels to default
            for (int i = 0; i < rotatedImage.image.GetLength(0); i++)
            {
                for (int j = 0; j < rotatedImage.image.GetLength(1); j++)
                {
                    if (rotatedImage.image[i, j] == null) rotatedImage.image[i, j] = new Pixel();
                }
            }

            return rotatedImage;
        }

        static (int, int) get_optimal_dim(MyImage image, double angle)
        {
            // get corners' coordinates
            int center_i = image.height / 2;
            int center_j = image.width / 2;
            int i1 = 0;
            int j1 = 0;
            int j2 = image.width;
            int i2 = 0;
            int j3 = image.width;
            int i3 = image.height;
            int i4 = image.height;
            int j4 = 0;

            // rotation matrix by the trigo way :
            // cos(angle)   sin(angle)
            // -sin(angle)  cos(angle)

            // fix center at origin
            j1 -= center_j;
            j2 -= center_j;
            j3 -= center_j;
            j4 -= center_j;
            i1 -= center_i;
            i2 -= center_i;
            i3 -= center_i;
            i4 -= center_i;

            // get new corners' coordinates
            double j1_new = j1 * Math.Cos(angle) - i1 * Math.Sin(angle);
            double i1_new = j1 * Math.Sin(angle) + i1 * Math.Cos(angle);
            double j2_new = j2 * Math.Cos(angle) - i2 * Math.Sin(angle);
            double i2_new = j2 * Math.Sin(angle) + i2 * Math.Cos(angle);
            double j3_new = j3 * Math.Cos(angle) - i3 * Math.Sin(angle);
            double i3_new = j3 * Math.Sin(angle) + i3 * Math.Cos(angle);
            double j4_new = j4 * Math.Cos(angle) - i4 * Math.Sin(angle);
            double i4_new = j4 * Math.Sin(angle) + i4 * Math.Cos(angle);

            double min_j = Math.Min(j1_new, Math.Min(j2_new, Math.Min(j3_new, j4_new)));
            double max_j = Math.Max(j1_new, Math.Max(j2_new, Math.Max(j3_new, j4_new)));
            double min_i = Math.Min(i1_new, Math.Min(i2_new, Math.Min(i3_new, i4_new)));
            double max_i = Math.Max(i1_new, Math.Max(i2_new, Math.Max(i3_new, i4_new)));

            // get dimensions in byte (un pixel is 3 bytes)
            int rotatedWidth = ((int)(max_j - min_j));
            int rotatedHeight = ((int)(max_i - min_i));

            return (rotatedHeight, rotatedWidth);
        }
    }
}