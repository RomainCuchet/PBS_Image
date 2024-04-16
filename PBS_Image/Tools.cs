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
            public string type;    /*La signature(sur 2 octets), indiquant qu'il s'agit d'un fichier BMP à l'aide des deux caractères.
                        BM, 424D en hexadécimal, indique qu'il s'agit d'un Bitmap Windows.
                        BA indique qu'il s'agit d'un Bitmap OS/2.
                        CI indique qu'il s'agit d'une icone couleur OS/2.
                        CP indique qu'il s'agit d'un pointeur de couleur OS/2.
                        IC indique qu'il s'agit d'une icone OS/2.
                        PT indique qu'il s'agit d'un pointeur OS/2.*/
            public int taille_fichier; //La taille totale du fichier en octets (codée sur 4 octets)
            public int id_ap; // Un champ réservé pour le créteur de l'image (sur 4 octets)
            public int offset; // L'offset de l'image (sur 4 octets), en français décalage, c'est-à-dire l'adresse relative du début des informations concernant l'image par rapport au début du fichier
            public int taille_entete; //La taille de l'entête de l'image en octets(codée sur 4 octets). Les valeurs hexadécimales suivantes sont possibles suivant le type de format BMP :28 pour Windows 3.1x, 95, NT, ... 0C pour OS/2 1.x F0 pour OS/2 2.x
            public int width; // La largeur de l'image (sur 4 octets), c'est-à-dire le nombre de pixels horizontalement
            public int height; //La hauteur de l'image (sur 4 octets), c'est-à-dire le nombre de pixels verticalemen
            public int nb_plans; // Le nombre de plans (sur 2 octets). Cette valeur vaut toujours 1
            public int nb_bits_color; //La profondeur de codage de la couleur(sur 2 octets), c'est-à-dire le nombre de bits utilisés pour coder la couleur. Cette valeur peut-être égale à 1, 4, 8, 16, 24 ou 32
            public int comp_format; // La méthode de compression (sur 4 octets). Cette valeur vaut 0 lorsque l'image n'est pas compressée, ou bien 1, 2 ou 3 suivant le type de compression utilisé  :1 pour un codage RLE de 8 bits par pixel2 pour un codage RLE de 4 bits par pixel3 pour un codage bitfields, signifiant que la couleur est codé par un triple masque représenté par la palette
            public int taille_image; //(sur 4 octets)
            public int hor_res; //La résolution horizontale(sur 4 octets), c'est-à-dire le nombre de pixels par mètre horizontalement
            public int vert_res; // La résolution verticale (sur 4 octets), c'est-à-dire le nombre de pixels par mètre verticalement
            public int nb_color; // Le nombre de couleurs de la palette (sur 4 octets)
            public int nb_color_imp; // Le nombre de couleurs importantes de la palette (sur 4 octets). Ce champ peut être égal à 0 lorsque chaque couleur a son importance.
                                     // la palette est optionelle
            public int p_blue; // palette blue
            public int p_green; // palette green
            public int p_red; // palette red
            public int p_r; // palette réservé
            public byte[] data;
            #endregion

            public void test_length_image()
            {
                Console.WriteLine(data.Length-offset-width*height*3);
                Console.WriteLine(data.Length-offset);
            }

            public void get_meta(byte[] data)
            {
                this.data = data;
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
                if (offset == 58) // La palette est optionelle
                {
                    p_blue = Tools.BytesToInt(new byte[] { data[54] });
                    p_green = Tools.BytesToInt(new byte[] { data[55] });
                    p_red = Tools.BytesToInt(new byte[] { data[56] });
                    p_r = Tools.BytesToInt(new byte[] { data[57] });
                }
            }

            public void display_Header()
            {
                string res = "Header\n";
                for(int i=0;i<offset;i++)
                {
                    res += data[i] + " ";
                }
                Console.WriteLine(res+"\n");
            }

            public Header(string file_name, string folder = "../../../Images/")
            {
                byte[] data = File.ReadAllBytes(folder + file_name);
                get_meta(data);
            }
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



            public void display()
            {
                Console.WriteLine(Tostring());
                Console.WriteLine();
            }
        }
        public static void PrintInfo(string file_name = "Test.bmp", string folder = "../../../Images/", bool p_image = false)
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