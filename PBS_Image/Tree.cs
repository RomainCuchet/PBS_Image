using System;
using System.Reflection.Metadata.Ecma335;

/// namespace PBS_Image <summary>
/// namespace PBS_Image
/// </summary>
/// 
namespace PBS_Image
{
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
        /// Rebuild a Huffman tree from an encoding table
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
        /// Builds a frequency dictionary from a pixel matrix
        /// </summary>
        /// <param name="pixelMatrix">Input Pixel matrix</param>
        /// <returns>A dictionnary with key being the pixel, and value being its occurences number</returns>
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
        /// Build a huffman tree
        /// </summary>
        /// <param name="frequencies">frequency dictionnary to build the tree from</param>
        /// <returns>returns the root of a huffman tree</returns>
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
        /// Build an encoding table from a Huffman tree
        /// </summary>
        /// <param name="root">Current root of the huffman tree</param>
        /// <param name="code">binary encoded path traversed: "0"= left child, "1"= right child</param>
        /// <param name="codes">Encoding table that needs to be initialised</param>
        /// <returns>A Dictionnary with each key being a pixel, and each value being its new string representation</returns>

        public Dictionary<Pixel, string> BuildEncodingTable(Node root, string code, Dictionary<Pixel, string> codes) //signature to rework
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
        /// Translate a matrix of pixels into a string of bits, with the help of the encoding translation
        /// </summary>
        /// <param name="pixelMatrix">Pixel matrix to be compressed</param>
        /// <param name="codes">Encoding table for the specified pixel matrix</param>
        /// <returns>A bit string representing the new compressed image</returns>
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
        /// Decompress a string of bits into a matrix of pixels, with the help of the Huffman tree  
        /// </summary>
        /// <param name="encoded">bit string that represents an image</param>
        /// <param name="width">width of the image to recompose, accessible through the header</param>
        /// <param name="height">height of the image to recompose, accessible through the header</param>
        /// <returns>The reconstructed matrix of pixel, usable by any codec</returns>
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
        /// Save the encoding table to a file in order to be able to retrieve an image later
        /// </summary>
        /// <param name="codes">Encoding table</param>
        /// <param name="path">Path to the txt to save</param>
        /// <exception cref="NotImplementedException"></exception>
        public void SaveEncodingTable(Dictionary<Pixel, string> codes, string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a high-level string representation of the Huffman tree
        /// </summary>
        /// <param name="root">The root of the Huffman tree</param>
        /// <param name="path">The path to the current node</param>
        /// <returns>A string representation of the Huffman tree</returns>
        public static string TreeToString(Node root, string path = "") //pk static ?
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
        // this does not work, I wanna kill myself

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