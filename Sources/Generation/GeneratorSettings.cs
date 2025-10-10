
namespace WG2.Generation
{
    public struct GeneratorSettings
    {
        /// <summary>
        /// how much tokens generate
        /// </summary>
        public int TokensGenerateCount = 200;
        /// <summary>
        /// 0..100 chance that next token will be chosen randomly from tokens array
        /// </summary>
        public double RandomNextTokenChance;
        
        public bool LogDebugInfo;

        public int TopK = 5;

        public GeneratorSettings()
        {
        }
    }
}
