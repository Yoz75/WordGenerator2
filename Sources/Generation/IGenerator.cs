
namespace WG2.Generation
{
    public interface IGenerator
    {
        public string Generate(GeneratorSettings settings, Token[] tokens);
    }
}
