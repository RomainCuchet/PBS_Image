using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBS_Image
{
    internal class MyImage
    {
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
        public Pixel[,] image;

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

        public MyImage(string file_name, string folder = "../../../Images/")
        {
            byte[] data = File.ReadAllBytes(folder + file_name);
            get_meta(data);
            image = new Pixel[height, width];
            get_image(data);
        }

        /// <summary>
        /// permet d'obtenir les métadatas d'une image au format bitmap
        /// </summary>
        /// <param name="data">les bits de l'image</param>
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
            if (offset == 58) // La palette est optionelle
            {
                p_blue = Tools.BytesToInt(new byte[] { data[54] });
                p_green = Tools.BytesToInt(new byte[] { data[55] });
                p_red = Tools.BytesToInt(new byte[] { data[56] });
                p_r = Tools.BytesToInt(new byte[] { data[57] });
            }
        }
        /// <summary>
        /// get a pixel matrix to represent the image
        /// </summary>
        /// <param name="data">array of bytes</param>
        public void get_image(byte[] data)
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
                        i++;
                        j = 0;
                    }
                }
            }

        }
        /// <summary>
        /// Print the pixels of the image, too long too be aplied to big images 
        /// </summary>
        /// <returns></returns>
        public string ImageToString()
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
            if (offset == 58)
            {
                save_var_byte(p_blue, 1);
                save_var_byte(p_green, 1);
                save_var_byte(p_red, 1);
                save_var_byte(p_r, 1);
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
    }
}
