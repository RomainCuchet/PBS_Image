using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PBS_Image
{
    class MyImage
    {
        #region parameters
        // https://web.maths.unsw.edu.au/~lafaye/CCM/video/format-bmp.htm
        public string type;    /*The signature (2 bytes), indicating that it is a BMP file using both characters.
                    BM, 424D in hexadecimal, indicates that it is a Windows Bitmap.
                    BA indicates that it is an OS/2 Bitmap.
                    CI indicates that this is an OS/2 color icon.
                    CP indicates that it is an OS/2 color pointer.
                    IC indicates that this is an OS/2 icon.
                    PT indicates that this is an OS/2 pointer.*/
        public int taille_fichier; //The total size of the file in bytes (encoded to 4 bytes)
        public int id_ap; // A reserved field for the image creator (4 bytes)
        public int offset; // The offset of the image (4 bytes), i.e. the relative address of the beginning of the information about the image compared to the beginning of the file
        public int taille_entete; //The size of the image header in bytes (encoded to 4 bytes). The following hexadecimal values are possible depending on the type of BMP format: 28 for Windows 3.1x, 95, NT, ... 0C for OS/2 1.x F0 for OS/2 2.x
        public int width; // The width of the image (about 4 bytes), i.e. the number of pixels horizontally
        public int height; //The height of the image (4 bytes), i.e. the number of pixels vertically
        public int nb_plans; // The number of plans (2 bytes). This value is always 1
        public int nb_bits_color; //The color encoding depth (over 2 bytes), i.e. the number of bits used to encode the color. This value can be equal to 1, 4, 8, 16, 24, or 32
        public int comp_format; //The compression method (4 bytes). This value is 0 when the image is not compressed, or 1, 2 or 3 depending on the type of compression used:1 for an 8-bit-per-pixel RLE encoding2 for a 4-bit-per-pixel RLE encoding3 for a bitfield encoding, meaning that the color is encoded by a triple mask represented by the palette
        public int taille_image; //size of the image (on 4 bytes).
        public int hor_res; //horizontal resolution (on 4 bytes). It is the number of pixels by meter horizontally
        public int vert_res; //vertical resolution (on 4 bytes). It is the number of pixels by meter vertically
        public int nb_color; // number of color on the pallet (on 4 bytes).
        public int nb_color_imp; //nummber of important colors on the pallet (on 4 bytes). It might be 0 if each color has its importance. pallet is optional                       
        public int p_blue; // blue pallet
        public int p_green; // green pallet
        public int p_red; // red pallet
        public int p_r; // reserved pallet
        public Pixel[,] image { get; set; }
        #endregion

        #region constructors
        /// <summary>
        /// constructor by copy
        /// </summary>
        /// <param name="myimage">myimage instance to copy</param>
        public MyImage(MyImage myimage)
        {
            type = myimage.type;
            taille_fichier = myimage.taille_fichier;
            id_ap = myimage.id_ap;
            offset = myimage.offset;
            taille_entete = myimage.taille_entete;
            width = myimage.width;
            height = myimage.height;
            nb_plans = myimage.nb_plans;
            nb_bits_color = myimage.nb_bits_color;
            comp_format = myimage.comp_format;
            taille_image = myimage.taille_image;
            hor_res = myimage.hor_res;
            vert_res = myimage.vert_res;
            nb_color = myimage.nb_color;
            nb_color_imp = myimage.nb_color_imp;
            p_blue = myimage.p_blue;
            p_green = myimage.p_green;
            p_red = myimage.p_red;
            p_r = myimage.p_r;
            image = new Pixel[myimage.image.GetLength(0), myimage.image.GetLength(1)];
            for (int i = 0; i < myimage.image.GetLength(0); i++)
            {
                for (int j = 0; j < myimage.image.GetLength(1); j++)
                {
                    image[i, j] = new Pixel(myimage.image[i, j]);
                }
            }
        }

        /// <summary>
        /// constructor by copy with a new image
        /// </summary>
        /// <param name="image">Image</param>
        public MyImage(Pixel[,] image)
        {
            this.image = image;
            type = "BM";
            offset = 54;
            taille_entete = 40;
            width = this.image.GetLength(1);
            height = this.image.GetLength(0);
        }

        /// <summary>
        /// constructor of MyImage by reading a file
        /// </summary>
        /// <param name="path">path of the image</param>
        public MyImage(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            get_meta(data);
            image = new Pixel[height, width];
            get_image_basic(data);
        }

        /// <summary>
        /// get the header of the image
        /// </summary>
        /// <param name="data">file written as byte array</param>
        public void get_meta(byte[] data)
        {
            // les 14 premies bytes constituent l'entête
            type = "" + Convert.ToChar(data[0]) + Convert.ToChar(data[1]); // obtention du type
            taille_fichier = Tools.BytesToInt(Tools.get_bytes_from(data, 2, 5));
            id_ap = Tools.BytesToInt(Tools.get_bytes_from(data, 6, 9));
            offset = Tools.BytesToInt(Tools.get_bytes_from(data, 10, 13));
            taille_entete = Tools.BytesToInt(Tools.get_bytes_from(data, 14, 17));
            width = Tools.BytesToInt(Tools.get_bytes_from(data, 18, 21));
            height = Tools.BytesToInt(Tools.get_bytes_from(data, 22, 25));
            nb_plans = Tools.BytesToInt(Tools.get_bytes_from(data, 26, 27));
            nb_bits_color = Tools.BytesToInt(Tools.get_bytes_from(data, 28, 29));
            comp_format = Tools.BytesToInt(Tools.get_bytes_from(data, 30, 33));
            taille_image = Tools.BytesToInt(Tools.get_bytes_from(data, 34, 37));
            hor_res = Tools.BytesToInt(Tools.get_bytes_from(data, 38, 41));
            vert_res = Tools.BytesToInt(Tools.get_bytes_from(data, 42, 45)); ;
            nb_color = Tools.BytesToInt(Tools.get_bytes_from(data, 46, 49)); ;
            nb_color_imp = Tools.BytesToInt(Tools.get_bytes_from(data, 50, 53));
        }
        /// <summary>
        /// get a pixel matrix to represent the image, only works without palette and remplissage when nb_bits_image = height*width*3
        /// </summary>
        /// <param name="data">array of bytes</param>
        public void get_image_basic(byte[] data)
        {
            if(offset != 54) throw new Exception("get_image_basic() only works with offset = 54, consider using get_image() to read them? Some functionalities work with offset unequal to 54 but our code doesn't support it completly and errors occur");
            int i = 0;
            int j = 0;
            if (nb_bits_color == 24) // cas encodage rgb standart
            {
                for (int data_i = offset; data_i < data.Length; data_i += 3) // boucle d'incrément la longeur de 3 pixels
                {

                    image[i, j] = new Pixel(data[data_i + 2], data[data_i + 1], data[data_i]);
                    j++;
                    if (j % width == 0)
                    {
                        i++;
                        j = 0;
                    }
                }
            }

        }

        /// <summary>
        /// write the image in the byte array named data
        /// </summary>
        /// <param name="data">byte array representing the image</param>
        public void get_image(byte[] data)
        {
            int i = 0;
            int j = 0;
            int nb_bits_pixel = nb_bits_color / 8;

            if (nb_bits_color == 24)
            {
                int bytesPerRow = ((width * nb_bits_pixel) + 3) & ~3;
                int dataIndex = offset;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte blue = data[dataIndex++];
                        byte green = data[dataIndex++];
                        byte red = data[dataIndex++];
                        image[i, j] = new Pixel(red, green, blue);
                        j++;
                    }

                    // Aller au début de la prochaine ligne, en sautant les octets de remplissage
                    dataIndex += (bytesPerRow - (width * nb_bits_pixel));
                    j = 0;
                    i++;
                }
            }
        }




        /// <summary>
        /// useful if we need to implement other formats
        /// </summary>
        /// <returns>the extention corresponding to the file format</returns>
        public string get_type()
        {
            if (type == "BM") return ".bmp";
            return ".txt";
        }

        public int get_int_type()
        {
            if (type == "BM") return 19778;
            return 0;
        }

        /// <summary>
        /// get a list of bytes that represents MyImage, required to save it
        /// </summary>
        /// <returns>the list of bytes corresponding to MyImage with the bmp format</returns>
        public List<byte> Get_Bytes_bmp()
        {
            List<byte> bytes = new List<byte>();
            save_var_byte(get_int_type(), 2);
            save_var_byte(taille_fichier);
            save_var_byte(id_ap);
            save_var_byte(offset);
            save_var_byte(taille_entete);
            save_var_byte(width);
            save_var_byte(height);
            save_var_byte(nb_plans, 2);
            save_var_byte(nb_bits_color, 2);
            save_var_byte(comp_format);
            save_var_byte(taille_image);
            save_var_byte(hor_res);
            save_var_byte(vert_res);
            save_var_byte(nb_color);
            save_var_byte(nb_color_imp);
            if (offset > 54)
            {
                for (int i = 0; i < offset; i++)
                {
                    bytes.Add(0);
                }
            }
            foreach (Pixel p in image)
            {
                foreach (byte b in p.toByte())
                {
                    bytes.Add(b);
                }
            }
            void save_var_byte(int var, int nb_bytes = 4)
            {
                foreach (byte b in Tools.IntToBytes(var, nb_bytes))
                {
                    bytes.Add(b);
                }
            }
            return bytes;
        }
        #endregion

        #region affichage

        /// <summary>
        /// Convert the image to a string
        /// </summary>
        /// <returns></returns>
        public string imageTostring()
        {
            if (image == null) return "vide";
            string str = "";
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    str += image[i, j].Tostring() + " ";
                }
            }
            return str;
        }

        /// <summary>
        /// Display a string
        /// </summary>
        public void display()
        {
            Console.WriteLine(Tostring());
        }

        /// <summary>
        /// Display the image
        /// </summary>
        public void display_image()
        {
            Console.WriteLine(imageTostring());
        }

        /// <summary>
        /// Convert the header to a string
        /// </summary>
        /// <returns></returns>
        public string Tostring()
        {
            string str = $"Image {type}\n" +
                $"taille_fichier : {taille_fichier}\n" +
                $"taille entête : {taille_entete}\n" +
                $"Offset : {offset}\n" +
                $"width : {width}\n" +
                $"height :{height}\n" +
                $"id_ap :{id_ap}\n" +
                $"nombre de plans  : {nb_plans}\n" +
                $"nombre de bits par couleur : {nb_bits_color}\n" +
                $"Format de compression : {comp_format}\n" +
                $"Taille image : {taille_image}\n" +
                $"Résolution horizontale : {hor_res}\n" +
                $"Résolution verticale : {vert_res}\n" +
                $"nombre de couleur : {nb_color}\n" +
                $"nombre de couleurs importantes : {nb_color_imp}\n" +
                $"nombre de couleurs : {nb_color}\n" +
                $"nombre de couleurs importantes : {nb_color_imp}\n";
            return str;
        }
        #endregion



        /// <summary>
        /// save MyImage to a file
        /// </summary>
        /// <param name="folder">the destination folder</param>
        /// <param name="file_name">the name of the desstination file</param>
        /// <param name="random_name">wether to generate a new name or not</param>
        public void save(string folder = "../../../Images/Save/", string file_name = "Save", bool random_name = true)
        {

            List<byte> bytes = Get_Bytes_bmp();
            if (random_name) file_name = folder + file_name + Tools.get_counter();
            else file_name = folder + file_name;
            file_name += get_type();
            using (var fileStream = new FileStream(file_name, FileMode.Create))
            using (var binaryWriter = new BinaryWriter(fileStream))
            {
                // Écrivez les octets dans le fichier
                binaryWriter.Write(bytes.ToArray());
            }
        }



        /// <summary>
        /// this function rotate the image by angle degrees 
        /// </summary>
        /// <param name="angle">the angle to rotate the image to</param>
        /// <param name="interpo">wether to use an interpolation algorithm for better rendering</param>
        /// <returns>an rotated instance of MyImage</returns>
        /// <remaks>using an interpolation algorithm is a must for big images. For small one deactivate it enables to reduce computation's time without loosing much quality.
        /// For basics angles (0,90,180,270) an other algorithm is lauched to optimize rotation without any quality loss  </remaks>
        public MyImage rotate(double angle = 90, bool interpo = true, bool optimal_dim = true)
        {
            return Modification.rotate(this, angle, interpo, optimal_dim);
        }

        /// <summary>
        /// this function resize the image on a unique dimension
        /// </summary>
        /// <param name="factor">height and width will be multiplied by the factor </param>
        /// <returns></returns>
        public MyImage resize(double factor = 2)
        {
            return Modification.resize(this, factor);
        }


        /// <summary>
        /// trigger the filter function from the Filter class to apply a filter on the image
        /// </summary>
        /// <param name="kernel">the matrix used for convolution matrix</param>
        /// <param name="basic">enables redirecting in filter.filter function</param>
        /// <returns></returns>
        public MyImage filter(string kernel, bool basic = false)
        {
            MyImage filtered = new MyImage(this);
            filtered.image = Filter.filter(filtered.image, kernel, basic);
            return filtered;
        }


        #region stegano
        /// <summary>
        /// Enumeration of channels on which an image can be hidden, allows for better readability if needed
        /// </summary>
        public enum Hideout
        {
            Red = 0b001,
            Green = 0b010,
            Blue = 0b100,
            All = 0b111,
            Nothing
        }

        /// <summary>
        /// Get the hidden image from the current image
        /// </summary>
        /// <param name="n">Number of bits on which the image was encoded</param>
        /// <param name="hideout">Channels on which the image was encoded</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public MyImage GetHiddenImage(int n=4, byte hideout=0b111)
        {
            byte lsb = Bytes.GetLeastSignificantBits(hideout, 3);

            bool red = (Bytes.GetLeastSignificantBits(lsb, 1) == 1);
            bool green = (Bytes.GetLeastSignificantBits(((byte)(lsb >> 1)), 1) == 1);
            bool blue = (Bytes.GetLeastSignificantBits(((byte)(lsb >> 2)), 1) == 1);

            if (!red && !blue && !green)
                throw new Exception("No channel selected");
            else
            {
                Pixel[,] newImage = new Pixel[height, width];

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        byte newR = red ? Bytes.DecompressBits(Bytes.GetLeastSignificantBits(image[i, j].red, n), n) : (byte)0;
                        byte newG = green ? Bytes.DecompressBits(Bytes.GetLeastSignificantBits(image[i, j].green, n), n) : (byte)0;
                        byte newB = blue ? Bytes.DecompressBits(Bytes.GetLeastSignificantBits(image[i, j].blue, n), n) : (byte)0;

                        if (!red && blue && green) newR = (byte)((newB + newG) / 2);
                        else if (red && !blue && green) newB = (byte)((newR + newG) / 2);
                        else if (red && blue && !green) newG = (byte)((newR + newB) / 2);
                        else if (red && !blue && !green) newG = newB = newR;
                        else if (!red && green && !blue) newR = newB = newG;
                        else if (!red && !green && blue) newR = newG = newB;

                        Pixel newPixel = new Pixel(newR, newG, newB);
                        newImage[i, j] = newPixel;
                    }
                }

                return new MyImage(newImage);
            }
        }


        /// <summary>
        /// Hide an image in another image
        /// </summary>
        /// <param name="n">Number of bits in which the image will be encoded</param>
        /// <param name="hideout">Selecting cache channels: refer to Hideout for instructions on how to use it</param>
        /// <param name="toHide">Image to hide</param>
        /// <exception cref="NoChannelException">If no channel to hide is selected</exception>
        /// <exception cref="SizeException">If the image to be hidden is larger than the supporting image</exception>

        public void HideImage(MyImage toHide,int n =4,byte hideout = 0b111)
        {
            if (hideout == 0)
            {
                throw new NoChannelException("No channel selected");
            }

            if (toHide.width > width || toHide.height > height)
            {
                throw new SizeException("hideImage(): The image to hide is too big");
            }
            int randomX = new Random().Next(0, width - toHide.width);
            int randomY = new Random().Next(0, height - toHide.height);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    (byte R, byte G, byte B) toHidePixels;

                    if (j >= randomX && j < randomX + toHide.width && i >= randomY && i < randomY + toHide.height)
                    {
                        toHidePixels = (toHide.image[i-randomY, j-randomX].red, toHide.image[i-randomY, j-randomX].green, toHide.image[i-randomY, j-randomX].blue);
                    }
                    else
                    {
                        toHidePixels = ((byte) new Random().Next(0, 255), (byte) new Random().Next(0, 255), (byte) new Random().Next(0, 255));
                    }
                    
                    Pixel myPixel = new Pixel(image[i, j].red, image[i, j].green, image[i, j].blue);

                    byte compressed;
                    // we will choose an arbitrary color to hide a grayscale so red green and then blue but it doesnt matter
                    if ((hideout & (byte)Hideout.Red) != 0)
                    {
                        compressed = Bytes.CompressBits(toHidePixels.R, n);
                        Bytes.SetLeastSignificantBits(ref myPixel.red, compressed, n);
                    }

                    if ((hideout & (byte)Hideout.Green) != 0)
                    {
                        compressed = Bytes.CompressBits(toHidePixels.G, n);
                        Bytes.SetLeastSignificantBits(ref myPixel.green, compressed, n);
                    }

                    if ((hideout & (byte)Hideout.Blue) != 0)
                    {
                        compressed = Bytes.CompressBits(toHidePixels.B, n);
                        Bytes.SetLeastSignificantBits(ref myPixel.blue, compressed, n);
                    }

                    image[i, j] = myPixel;
                }
            }
        }

        /// <summary>
        /// Get three hidden images from the current image, each hidden image corresponds to a specific color channel (red, green, blue)
        /// </summary>
        /// <param name="n">Number of bits on which the images were encoded</param>
        /// <returns>A tuple containing three MyImage instances, each representing a hidden image</returns>
        public (MyImage, MyImage, MyImage) GetTripleHiddenImage(int n=4)
        {
            // Get the hidden image for each color channel using the GetHiddenImage method
            MyImage redHiddenImage = GetHiddenImage(n, 0b001);
            MyImage greenHiddenImage = GetHiddenImage(n, 0b010);
            MyImage blueHiddenImage = GetHiddenImage(n, 0b100);

            // Return the three hidden images as a tuple
            return (redHiddenImage, greenHiddenImage, blueHiddenImage);
        }

        /// <summary>
        /// Hide three images in the current image, each image corresponding to a specific color channel (red, green, blue)
        /// </summary>
        /// <param name="red">Image to hide in the red channel</param>
        /// <param name="green">Image to hide in the green channel</param>
        /// <param name="blue">Image to hide in the blue channel</param>
        /// <param name="n">Number of bits in which the images will be encoded</param>
        public void HideTripleImage(MyImage red, MyImage green, MyImage blue, int n = 4)
        {
            HideImage(toHide: red, hideout: 0b001, n: n);
            HideImage(toHide: green, hideout: 0b010, n: n);
            HideImage(toHide: blue, hideout: 0b100, n: n);
        }

        #endregion
    }
}
