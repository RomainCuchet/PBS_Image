using System;
using System.Reflection.Metadata.Ecma335;

namespace PBS_Image
{
    //création d'un arbre huffman
    public class Tree
    {
        #region Paramètres
        public Node Root { get; set; }
        Dictionary<Pixel, int> _frequencies { get; set; }
        #endregion

        #region Propriétés
        public Dictionary<Pixel, int> Frequencies
        {
            get
            {
                return _frequencies;
            }
        }
        #endregion

        #region Constructeurs
        /// <summary>
        /// Reconstruit un arbre de Huffman à partir d'une table de codage
        /// </summary>
        /// <param name="encodingTable"></param>
        /// <exception cref="NotImplementedException"></exception>
        public Tree(Dictionary<Pixel, string> encodingTable)
        {
            throw new NotImplementedException();
        }

        public Tree(Node root, Dictionary<Pixel, int> frequencies)
        {
            Root = root;
            _frequencies = frequencies;
        }
        #endregion

        #region Construction de l'arbre
        /// <summary>
        /// Construit un dictionnaire de fréquences à partir d'une matrice de pixels
        /// </summary>
        /// <param name="pixelMatrix"></param>
        /// <returns>Un dictionnaire trié par fréquence de pixel</returns>
        public static Dictionary<Pixel, int> BuildFrequencyDictionary(Pixel[,] pixelMatrix)
        {
            Dictionary<Pixel, int> Frequencies = new Dictionary<Pixel, int>();

            for (int i = 0; i < pixelMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < pixelMatrix.GetLength(1); j++)
                {
                    Pixel pixel = pixelMatrix[i, j];

                    if (Frequencies.ContainsKey(pixel))
                    {
                        Frequencies[pixel]++;
                    }
                    else
                    {
                        Frequencies[pixel] = 1;
                    }
                }
            }

            //return Frequencies.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            return Frequencies;
        }


        /// <summary>
        /// Construit un arbre de Huffman à partir d'un dictionnaire de fréquences
        /// </summary>
        /// <param name="frequencies"></param>
        /// <returns>retourne la racine de l'arbre</returns>
        public static Node BuildTree(Dictionary<Pixel, int> frequencies)
        {

            // Construction d'un heap approximatif
            PriorityQueue<Node, int> nodes = new PriorityQueue<Node, int>();

            foreach (KeyValuePair<Pixel, int> pair in frequencies)
            {
                nodes.Enqueue(new Node(pair.Key, pair.Value), pair.Value);
            }

            // Construction de l'arbre
            while (nodes.Count > 1)
            {

                Node left = nodes.Dequeue();
                Node right = nodes.Dequeue();

                Node parent = new Node(left, right);

                nodes.Enqueue(parent, left.Frequency + right.Frequency);
            }

            return nodes.Dequeue();

        }

        /// <summary>
        /// Construit la table de codage à partir de l'arbre de Huffman
        /// </summary>
        /// <param name="root">Arbre de Huffman actuel</param>
        /// <param name="code">chemin parcouru encodé en binaire: "0"= Fgauche, "1"= Fdroit</param>
        /// <param name="codes">table de codage, à initialiser avant</param>
        /// <returns></returns>

        public Dictionary<Pixel, string> BuildEncodingTable(Node root, string code, Dictionary<Pixel, string> codes)
        {
            if (root.IsLeaf())
            {
                codes[root.Pixel] = code;
            }
            else
            {
                BuildEncodingTable(root.Left, code + "0", codes);
                BuildEncodingTable(root.Right, code + "1", codes);
            }

            return codes;
        }
        #endregion

        #region Encodage et décodage
        /// <summary>
        /// Encode une matrice de pixels en une chaine de bits suivant la table de codage prédéfinie
        /// </summary>
        /// <param name="pixelMatrix"></param>
        /// <param name="codes"></param>
        /// <returns></returns>
        public string Encode(Pixel[,] pixelMatrix, Dictionary<Pixel, string> codes)
        {
            string encoded = "";

            for (int i = 0; i < pixelMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < pixelMatrix.GetLength(1); j++)
                {
                    Pixel pixel = pixelMatrix[i, j];
                    encoded += codes[pixel];
                }
            }

            return encoded;
        }

        /// <summary>
        /// Décode une chaine de bits en une matrice de pixels suivant l'arbre de Huffman fourni
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public Pixel[,] Decode(string encoded, int width, int height)
        {
            Pixel[,] decoded = new Pixel[width, height];//modif ça c'est pas bon (on espère qu'on stocke la taille qq part, snn c la merde)

            Node current = Root;
            int row = 0;
            int col = 0;

            for (int i = 0; i < encoded.Length; i++)
            {
                if (encoded[i] == '0')
                {
                    current = current.Left; //descendre à gauche si la string à décoder est un 0
                }
                else
                {
                    current = current.Right; //descendre à droite si la string à décoder est un 1
                }

                if (current.IsLeaf()) //fin du chemin => valeur trouvée, le pixel stockée est donc celui cherché
                {
                    decoded[row, col] = current.Pixel; //attribution de la valeur du pixel décodé à la position correcte dans la matrice décodée
                    current = Root; //reset au début de l'arbre
                    col++;
                    if (col == height) //si on a atteint la fin de la ligne, on passe à la suivante
                    {
                        col = 0;
                        row++;
                    }
                }
            }
            return decoded;
        }
        #endregion

        #region Sauvegarde et chargement
        /// <summary>
        /// Sauvegarde la table de codage dans un fichier pour éviter une utilisation trop intensive de la mémoire (256^3 valeurs enregistrées en mémoire)
        /// </summary>
        /// <param name="codes"></param>
        /// <param name="path"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SaveEncodingTable(Dictionary<Pixel, string> codes, string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string representation of the Huffman tree
        /// </summary>
        /// <param name="root">The root of the Huffman tree</param>
        /// <param name="path">The path to the current node</param>
        /// <returns>A string representation of the Huffman tree</returns>
        public static string TreeToString(Node root, string path = "")
        {
            if (root == null)
            {
                return string.Empty;
            }

            if (root.IsLeaf())
            {
                return $"{path}: R={root.Pixel.red}, G={root.Pixel.green}, B={root.Pixel.blue}\n";
            }

            var left = TreeToString(root.Left, path + "0");
            var right = TreeToString(root.Right, path + "1");

            return left + right;
        }

        

        #endregion
    
    #region JPEG DHT Header
    /// <summary>
    /// Structures the DHT (Define Huffman Table) header for a JPEG image from a Huffman tree
    /// </summary>
    /// <param name="root">The root of the Huffman tree</param>
    /// <returns>A byte array representing the DHT header</returns>
    public static byte[] StructureDHTHeader(Node root)
    {
        if (root == null)
        {
            throw new ArgumentNullException(nameof(root));
        }

        List<byte> dhtHeader = new List<byte>();

        // Add DHT marker
        dhtHeader.AddRange(new byte[] { 0xFF, 0xC4 });

        // Placeholder for length, will be filled later
        dhtHeader.AddRange(new byte[] { 0x00, 0x00 });

        // Add Huffman table information
        dhtHeader.Add(0x00); // Table class (0 = DC, 1 = AC) and identifier (0-3)

        // Placeholder for number of symbols of lengths 1-16, will be filled later
        dhtHeader.AddRange(new byte[16]);

        // Placeholder for symbols, will be filled later
        List<byte> symbols = new List<byte>();

        // Traverse the Huffman tree and fill the symbols and their lengths
        TraverseTree(root, "", (symbol, length) =>
        {
            // Convert symbol to byte and add to symbols list
            symbols.Add(Convert.ToByte(symbol, 2));

            // Increment the count of symbols of this length
            dhtHeader[4 + length - 1]++;
        });

        // Add symbols to DHT header
        dhtHeader.AddRange(symbols);

        // Fill the length (excluding the marker)
        int length = dhtHeader.Count - 2;
        dhtHeader[2] = (byte)(length >> 8);
        dhtHeader[3] = (byte)(length & 0xFF);

        return dhtHeader.ToArray();
    }

    /// <summary>
    /// Traverses the Huffman tree and executes an action for each leaf node
    /// </summary>
    /// <param name="node">The current node</param>
    /// <param name="path">The path to the current node</param>
    /// <param name="action">The action to execute for each leaf node</param>
    private static void TraverseTree(Node node, string path, Action<string, int> action)
    {
        if (node == null)
        {
            return;
        }

        if (node.IsLeaf())
        {
            action(path, path.Length);
        }
        else
        {
            TraverseTree(node.Left, path + "0", action);
            TraverseTree(node.Right, path + "1", action);
        }
    }
    #endregion
}
}