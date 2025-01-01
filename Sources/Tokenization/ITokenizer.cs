

namespace WG2.Tokenization
{
    public interface ITokenizer
    {
        public Token[] Tokenize(TokenizerSettings settings, string text);
    }
}
