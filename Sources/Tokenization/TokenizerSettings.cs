
namespace WG2.Tokenization
{
    public struct TokenizerSettings
    {
        /// <summary>
        /// Minimal token size. Usual maximal and minimal sizes are the same
        /// </summary>
        public int MinimalTokenSize;
        /// <summary>
        /// Maximal token size. Usual maximal and minimal sizes are the same
        /// </summary>
        public int MaximalTokenSize;

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
