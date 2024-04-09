using PBS_Image;
using System;
using System.Diagnostics;
using System.IO;

namespace PBS_Image
{
    class Tools
    {
        public static void PrintInfo(string folder = "../../Images/", string file_name = "Test.bmp", bool p_image = false)
        {
            // http://wxfrantz.free.fr/index.php?p=format-bmp


            byte[] myfile = File.ReadAllBytes(folder + file_name);
            //myfile est un vecteur composé d'octets représentant les métadonnées et les données de l'image

            //Métadonnées du fichier
            Console.WriteLine("\n Header \n");
            for (int i = 0; i < 14; i++)
                Console.Write(myfile[i] + " ");
            //Métadonnées de l'image
            Console.WriteLine("\n HEADER INFO \n");
            for (int i = 14; i < 54; i++)
                Console.Write(myfile[i] + " ");
            if (p_image)
            {
                //L'image elle-même
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

        public static byte Int32toByte(int n, bool little_endian = true)
        {
            return IntToBytes(n, 4, true, 256)[0];
        }

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

        public static string ArrayToString<T>(T[] array)
        {
            string str = "";
            foreach (var item in array) str += item + " ";
            return str;
        }


        public static int get_counter(string filePath = "../../../counter.txt", bool inc = true)
        {
            int value = 0;

            try
            {
                // Lire la valeur actuelle du fichier
                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath);
                    if (int.TryParse(content, out value))
                    {
                        if (inc)
                        {
                            // Incrémenter la valeur
                            value++;

                            // Écrire la nouvelle valeur dans le fichier
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

        public static byte[] get_bytes_from(byte[] bytes, int start, int stop)
        {
            byte[] res = new byte[stop - start + 1];
            for (int i = start; i <= stop; i++)
            {
                res[i - start] = bytes[i];
            }
            return res;
        }


        // Fonction d'interpolation bilinéaire
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