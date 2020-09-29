namespace ML.Algorithms.Models
{
    // Namespaces
    #region Namespaces
    using System.Collections.Generic;
    #endregion

    public class Word2Vector
    {
        public Word2Vector()
        {
            this.allChars = new char[]
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h',
                'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
                'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
                'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
                'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
                'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
                'Y', 'Z',
                '0', '1', '2', '3', '4', '5', '6', '7',
                '8', '9',
                '-', '_'
            };

            this.indexMappings = new Dictionary<char, int>();
            for (int i = 0; i < this.allChars.Length; i++)
            {
                this.indexMappings.Add(this.allChars[i], i);
            }
        }

        public double[] ToOneHotVector(string word, bool caseSensitive = true, bool useIndex = false)
        {
            if (!caseSensitive)
            {
                word = word.ToLower();
            }

            double[] vector = new double[this.allChars.Length];
            for (int i = 0; i < word.Length; i++)
            {
                if (useIndex)
                {
                    vector[this.indexMappings[word[i]]] += (i + 1.0);
                }
                else
                {
                    vector[this.indexMappings[word[i]]] += 1.0;
                }
            }

            return vector;
        }

        private char[] allChars;
        private Dictionary<char, int> indexMappings;
    }
}
