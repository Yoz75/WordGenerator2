
namespace WG2.Tokenization
{
    public struct TokenizerSettings
    {
        /// <summary>
        /// Minimal token size. Usual maximal and minimal sizes are the same
        /// </summary>
        public int MinimalTokenSize = 3;
        /// <summary>
        /// Maximal token size. Usual maximal and minimal sizes are the same
        /// </summary>
        public int MaximalTokenSize = 3;

        /// <summary>
        /// Count of RDTokenizer's iterations (every time adds new random-sized tokens).
        /// </summary>
        public int RandomIterations = 1;

        /// <summary>
        /// Capacity of the result tokens graph.
        /// </summary>
        public int ResultCapacity = 100;

        /// <summary>
        /// Count of iterations for iterative tokenizer
        /// </summary>
        public int ItTokenizerSamples = 3;

        /// <summary>
        /// Value of top K for iterative tokenizer (ItTokenizer selects random pair of K popularest).
        /// </summary>
        public int ItTokenizerTopK = 1;

        /// <summary>
        /// Minimal count of pair in text to be merged by ItTokenizer.
        /// </summary>
        public int ItTokenizerMinMergeCount = 2;

        public bool LogDebugInfo;

        public TokenizerSettings()
        {
        }
    }
}
