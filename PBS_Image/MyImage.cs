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
        public string type = "BM";    /*La signature(sur 2 octets), indiquant qu'il s'agit d'un fichier BMP à l'aide des deux caractères.
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
        public int nb_plans = 1; // Le nombre de plans (sur 2 octets). Cette valeur vaut toujours 1
        public int nb_bits_color = 24; //La profondeur de codage de la couleur(sur 2 octets), c'est-à-dire le nombre de bits utilisés pour coder la couleur. Cette valeur peut-être égale à 1, 4, 8, 16, 24 ou 32
        public int comp_format; // La méthode de compression (sur 4 octets). Cette valeur vaut 0 lorsque l'image n'est pas compressée, ou bien 1, 2 ou 3 suivant le type de compression utilisé  :1 pour un codage RLE de 8 bits par pixel2 pour un codage RLE de 4 bits par pixel3 pour un codage bitfields, signifiant que la couleur est codé par un triple masque représenté par la palette
        public int taille_image; //(sur 4 octets)
        public int hor_res = 11811; //La résolution horizontale(sur 4 octets), c'est-à-dire le nombre de pixels par mètre horizontalement
        public int vert_res = 11811; // La résolution verticale (sur 4 octets), c'est-à-dire le nombre de pixels par mètre verticalement
        public int nb_color; // Le nombre de couleurs de la palette (sur 4 octets)
        public int nb_color_imp; // Le nombre de couleurs importantes de la palette (sur 4 octets). Ce champ peut être égal à 0 lorsque chaque couleur a son importance.
        // la palette est optionelle
        public int p_blue; // palette blue
        public int p_green; // palette green
        public int p_red; // palette red
        public int p_r; // palette réservé
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

        public MyImage(Pixel[,] image)
        {
            this.image = image;
            type = "BM";
            offset = 54;
            taille_entete = 40;
            width = this.image.GetLength(1);
            height = this.image.GetLength(0);
        }


        public MyImage(string file_name, string folder = "../../../Images/")
        {
            byte[] data = File.ReadAllBytes(folder + file_name);
            get_meta(data);
            image = new Pixel[height, width];
            get_image(data);
        }

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
                        Console.WriteLine(j);
                        i++;
                        j = 0;
                    }
                }
            }

        }

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

        public void display()
        {
            Console.WriteLine(Tostring());
        }

        public void display_image()
        {
            Console.WriteLine(imageTostring());
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
            return str;
        }
        #endregion



        /// <summary>
        /// save MyImage to a file
        /// </summary>
        /// <param name="folder">the destination folder</param>
        /// <param name="file_name">the name of the desstination file</param>
        /// <param name="random_name">wether to generate a new name or not</param>
        public void Save(string folder = "../../../Images/Save/", string file_name = "Save", bool random_name = true)
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

        public MyImage resize(double factor = 2)
        {
            return Modification.resize(this, factor);
        }

        public MyImage filter(string kernel, bool basic = false)
        {
            MyImage filtered = new MyImage(this);
            filtered.image = Filter.filter(filtered.image, kernel, basic);
            return filtered;
        }


        #region stegano
        /// <summary>
        /// Enumération des channels sur lesquels on peut cacher une image, permet une meilleure lisibilité si besoin
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
        /// <param name="n">Nombres de bits sur lesquels l'image a été encodée</param>
        /// <param name="hideout">Channels sur lesquels l'image a été encodée</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public (MyImage, MyImage, MyImage) GetHiddenImage(int n, byte hideout, bool triple = false)
        {
            byte lsb = Bytes.GetLeastSignificantBits(hideout, 3);

            bool red = (Bytes.GetLeastSignificantBits(lsb, 1) == 1);
            bool green = (Bytes.GetLeastSignificantBits(((byte)(lsb >> 1)), 1) == 1);
            bool blue = (Bytes.GetLeastSignificantBits(((byte)(lsb >> 2)), 1) == 1);

            if (!red && !blue && !green)
                throw new Exception("No channel selected");
            else
            {
                Pixel[,] newImage1 = new Pixel[height, width];
                Pixel[,] newImage2 = new Pixel[height, width];
                Pixel[,] newImage3 = new Pixel[height, width];

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

                        if (triple)
                        {
                            newImage1[i, j] = new Pixel(newR, newR, newR);
                            newImage2[i, j] = new Pixel(newG, newG, newG);
                            newImage3[i, j] = new Pixel(newB, newB, newB);
                        }
                        else
                        {
                            newImage1[i, j] = new Pixel(newR, newG, newB);
                            newImage2[i, j] = new Pixel(newR, newG, newB);
                            newImage3[i, j] = new Pixel(newR, newG, newB);
                        }

                    }
                }

                return (new MyImage(newImage1), new MyImage(newImage2), new MyImage(newImage3));
            }
        }


        /// <summary>
        /// Hide an image in another image
        /// </summary>
        /// <param name="n">nombre de bits dans lesquels l'image sera encodée</param>
        /// <param name="hideout">sélection des channels de cache: se réferer à Hideout pour savoir comment s'en servir</param>
        /// <param name="toHide">image à cacher</param>
        /// <exception cref="NoChannelException">Si aucun channel pour cacher n'est sélectionné</exception>
        /// <exception cref="SizeException">Si l'image à cacher est plus grande que l'image de support</exception>

        public void HideImage(int n, byte hideout, MyImage[] toHides)
        {
            if (hideout == 0)
            {
                throw new NoChannelException("No channel selected");
            }
            if (toHides.Length == 0 || toHides.Length > 3)
            {
                throw new InvalideArgumentException("HideImage(): toHides must be an array of 1 to 3 images");
            }

            foreach (MyImage toHide in toHides)
            {
                if (toHide.width > width || toHide.height > height)
                {
                    throw new SizeException("hideImage(): The image to hide is too big");
                }
            }

            (int, int)[] randomPos = new (int, int)[toHides.Length];
            for (int i = 0; i < toHides.Length; i++)
            {
                randomPos[i] = (new Random().Next(0, width - toHides[i].width), new Random().Next(0, height - toHides[i].height));
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    (byte R, byte G, byte B) toHidePixels = (0, 0, 0);

                    if (toHides.Length == 1)
                    {
                        if (j >= randomPos[0].Item1 && j < randomPos[0].Item1 + toHides[0].width && i >= randomPos[0].Item2 && i < randomPos[0].Item2 + toHides[0].height)
                        {
                            toHidePixels = (toHides[0].image[i - randomPos[0].Item2, j - randomPos[0].Item1].red, toHides[0].image[i - randomPos[0].Item2, j - randomPos[0].Item1].green, toHides[0].image[i - randomPos[0].Item2, j - randomPos[0].Item1].blue);
                        }

                        else
                        {
                            toHidePixels = ((byte)new Random().Next(0, 255), (byte)new Random().Next(0, 255), (byte)new Random().Next(0, 255));
                        }
                    }

                    else if(toHides.Length == 2){
                        
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
        #endregion
    }
}
