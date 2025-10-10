

namespace WG2.Tokenization
{
    public interface ITokenizer
    {
        public DirectedGraph<Token, int> Tokenize(TokenizerSettings settings, string text);
    }
}
