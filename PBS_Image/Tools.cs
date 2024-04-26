using PBS_Image;
using System;
using System.Diagnostics;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace PBS_Image
{
    class Tools
    {
        public class Header
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
            public byte[] data;
            #endregion

            /// <summary>
            /// Print the length of the image
            /// </summary>
            public void test_length_image()
            {
                Console.WriteLine(data.Length-offset-width*height*3);
                Console.WriteLine(data.Length-offset);
            }

            /// <summary>
            /// Get the metadata of the image
            /// </summary>
            /// <param name="data">byte array of the image</param> 
            public void get_meta(byte[] data)
            {
                this.data = data;
                // 14 first bytes
                type = "" + Convert.ToChar(data[0]) + Convert.ToChar(data[1]); // gets the type of the image
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
                if (offset == 58) // La palette est optionelle
                {
                    p_blue = Tools.BytesToInt(new byte[] { data[54] });
                    p_green = Tools.BytesToInt(new byte[] { data[55] });
                    p_red = Tools.BytesToInt(new byte[] { data[56] });
                    p_r = Tools.BytesToInt(new byte[] { data[57] });
                }
            }

            /// <summary>
            /// Display the header of the image
            /// </summary>
            public void display_Header()
            {
                string res = "Header\n";
                for(int i=0;i<offset;i++)
                {
                    res += data[i] + " ";
                }
                Console.WriteLine(res+"\n");
            }


            /// <summary>
            /// Constructor of the header
            /// </summary>
            /// <param name="file_name">name of the file</param> 
            /// <param name="folder"> path of the folder</param>
            public Header(string file_name, string folder = "../../../Images/")
            {
                byte[] data = File.ReadAllBytes(folder + file_name);
                get_meta(data);
            }

            /// <summary>
            /// return the header as a string
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
                if (offset == 58)
                {
                    str += $"Palette bleue : {p_blue}\n" +
                        $"Palette vert : {p_green}\n" +
                        $"Palette rouge : {p_red}\n" +
                        $"Palette réservé : {p_r}\n";
                }
                else str += "La palette n'est pas définie";
                return str;
            }


            /// <summary>
            /// Display the header
            /// </summary>
            public void display()
            {
                Console.WriteLine(Tostring());
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Print the information of the image
        /// </summary>
        /// <param name="file_name">name of the file</param> 
        /// <param name="folder">path of the folder</param> 
        /// <param name="p_image">enables the print of the image</param> 
        public static void PrintInfo(string file_name = "Test.bmp", string folder = "../../../Images/", bool p_image = false)
        {
            // http://wxfrantz.free.fr/index.php?p=format-bmp


            byte[] myfile = File.ReadAllBytes(folder + file_name);
            //myfile is a vector composed of bytes representing the metadata and the data of the image

            //metadata of the image
            Console.WriteLine("\n Header \n");
            for (int i = 0; i < 14; i++)
                Console.Write(myfile[i] + " ");
            //metadata of the image
            Console.WriteLine("\n HEADER INFO \n");
            for (int i = 14; i < 54; i++)
                Console.Write(myfile[i] + " ");
            if (p_image)
            {
                //Image itself
                Console.WriteLine("\n IMAGE \n");
                for (int i = 54; i < myfile.Length; i = i + 60)
                {
                    for (int j = i; j < i + 60; j++)
                    {
                        Console.Write(myfile[j] + " ");


                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine();

        }
        
        /// <summary>
        /// Convert a byte array to an integer 32
        /// </summary>
        /// <param name="bytes_taille">length of the array</param> 
        /// <param name="little_endian">enables conversion in little endian</param> 
        /// <param name="modulo">specifies the modulo to use</param> 
        /// <returns></returns>
        public static int BytesToInt(byte[] bytes_taille, bool little_endian = true, int modulo = 256)
        {
            double n = 0;
            if (little_endian)
            {
                // 230 4 0 0 little endian 230*256^0 + 4*256^1+0*256^2+0*256^3
                for (int i = 0; i < bytes_taille.Length; i++)
                {
                    n += bytes_taille[i] * Math.Pow(modulo, i);
                }
                return Convert.ToInt32(n);
            }
            for (int i = bytes_taille.Length - 1; i > -1; i--)
            {
                n += bytes_taille[i] * Math.Pow(modulo, i);
            }
            return Convert.ToInt32(n);
        }


        /// <summary>
        /// Calculte the number of byte needed to represent a long
        /// </summary>
        /// <param name="number">long to represent</param> 
        /// <returns></returns>
        public static int CalculateNumberOfBytes(long number)
        {
            if (number == 0)
                return 1;

            int numberOfBytes = 0;

            while (number > 0)
            {
                numberOfBytes++;
                number /= 256;
            }

            return numberOfBytes;
        }

        /// <summary>
        /// Convert an integer to a byte
        /// </summary>
        /// <param name="n">integer to convert</param> 
        /// <param name="little_endian">enables conversion in little endian</param> 
        /// <returns></returns>
        public static byte Int32toByte(int n, bool little_endian = true)
        {
            return IntToBytes(n, 4, true, 256)[0];
        }


        /// <summary>
        /// Convert an integer to a byte array
        /// </summary>
        /// <param name="n"></param>
        /// <param name="numberOfBytes"></param>
        /// <param name="little_endian">enables conversio in little endian</param> 
        /// <param name="modulo">specifies the modulo to use</param> 
        /// <returns></returns>
        /// <exception cref="OverflowException"></exception>
        public static byte[] IntToBytes(long n, int numberOfBytes = 4, bool little_endian = true, int modulo = 256)
        {
            {
                byte[] resultBytes = new byte[numberOfBytes];

                for (int i = 0; i < numberOfBytes; i++)
                {
                    resultBytes[i] = (byte)(n % 256);
                    n /= 256;
                }

                if (n != 0)
                    throw new OverflowException("L'entier ne peut pas être représenté avec le nombre d'octets spécifié.");

                if (!little_endian) Array.Reverse(resultBytes); // La conversion inverse pour obtenir l'ordre correct des octets

                return resultBytes;
            }
        }

        /// <summary>
        /// Convert an array to a string 
        /// </summary>
        /// <typeparam name="T">the type of the array</typeparam> 
        /// <param name="array">the array to convert</param> 
        /// <returns></returns>
        public static string ArrayToString<T>(T[] array)
        {
            string str = "";
            foreach (var item in array) str += item + " ";
            return str;
        }

        /// <summary>
        /// Get the counter to build the name of the new file getting save.
        /// </summary>
        /// <param name="filePath"></param> 
        /// <param name="inc">enables the incrementation</param> 
        /// <returns></returns>
        public static int get_counter(string filePath = "../../../counter.txt", bool inc = true)
        {
            int value = 0;

            try
            {
                // Read the actual value of the file
                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath);
                    if (int.TryParse(content, out value))
                    {
                        if (inc)
                        {
                            // incremente the value
                            value++;

                            // Write the new value in the file
                            File.WriteAllText(filePath, value.ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Le contenu du fichier n'est pas un nombre valide.");
                    }
                }
                else
                {
                    Console.WriteLine("Le fichier n'existe pas.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite : " + ex.Message);
            }

            return value;
        }

        /// <summary>
        /// Substring method for byte array
        /// </summary>
        /// <param name="bytes">the main array</param> 
        /// <param name="start">start index</param> 
        /// <param name="stop">stop index</param> 
        /// <returns></returns>
        public static byte[] get_bytes_from(byte[] bytes, int start, int stop)
        {
            byte[] res = new byte[stop - start + 1];
            for (int i = start; i <= stop; i++)
            {
                res[i - start] = bytes[i];
            }
            return res;
        }


        /// <summary>
        /// method for the bilineary interpolation of the image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x">x coordonate of the pixel</param> 
        /// <param name="y">y coordonate of the pixel</param> 
        /// <returns></returns>
        public static Pixel interpolation(Pixel[,] image, double x, double y)
        {
            int x1 = (int)Math.Floor(x);
            int x2 = (int)Math.Ceiling(x);
            int y1 = (int)Math.Floor(y);
            int y2 = (int)Math.Ceiling(y);
            Pixel topLeft = new Pixel();
            Pixel topRight = new Pixel();
            Pixel bottomLeft = new Pixel();
            Pixel bottomRight = new Pixel();
            try
            {
                topLeft = image[y1, x1];
                topRight = image[y2, x1];
                bottomLeft = image[y1, x2];
                bottomRight = image[y2, x2];
            }
            catch
            {
                Console.WriteLine("error");
                Console.WriteLine(x1);
                Console.WriteLine(x2);
                Console.WriteLine(y1);
                Console.WriteLine(y2);
            }


            double dx = x - x1;
            double dy = y - y1;

            Pixel newp = new Pixel();

            newp.red = (byte)((1 - dx) * (1 - dy) * topLeft.red + dx * (1 - dy) * topRight.red + (1 - dx) * dy * bottomLeft.red + dx * dy * bottomRight.red);
            newp.green = (byte)((1 - dx) * (1 - dy) * topLeft.green + dx * (1 - dy) * topRight.green + (1 - dx) * dy * bottomLeft.green + dx * dy * bottomRight.green);
            newp.blue = (byte)((1 - dx) * (1 - dy) * topLeft.blue + dx * (1 - dy) * topRight.blue + (1 - dx) * dy * bottomLeft.blue + dx * dy * bottomRight.blue);

            return newp;
        }
    }
}