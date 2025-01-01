
namespace WG2.Generation
{
    public struct GeneratorSettings
    {
        /// <summary>
        /// how much tokens generate
        /// </summary>
        public int TokensGenerateCount;
        /// <summary>
        /// how much next tokens use. For example at value 2
        /// generator will use first 2 tokens in "Token.SubsequentTokens[i]
        /// </summary>
        public int NextTokensCount;
        /// <summary>
        /// 0..100 chance that next token will be chosen randomly from tokens array
        /// </summary>
        public byte RandomNextTokenChance;

        public int SubsequentTokensCount;
        public int MaxNextTokenSeekIterations = 15;

        /// <summary>
        /// At least this count of next token appearances should be to add it to result.
        /// </summary>
        public int MinimalNextTokenAppearances;

        public GeneratorSettings()
        {
        }
    }
}
