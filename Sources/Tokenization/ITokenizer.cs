

namespace WG2.Tokenization
{
    public interface ITokenizer
    {
        public DirectedGraph<Token> Tokenize(TokenizerSettings settings, string text);
    }
}
