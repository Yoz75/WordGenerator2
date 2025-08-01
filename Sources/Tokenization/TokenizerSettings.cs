
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

        public bool LogDebugInfo;

        public TokenizerSettings()
        {
        }
    }
}
