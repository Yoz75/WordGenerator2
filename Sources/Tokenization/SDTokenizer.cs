
using System;
using System.Collections.Generic;

namespace WG2.Tokenization
{
    /// <summary>
    /// Standard Dictionary Tokenizer. Uses Dictionary of tokens to add next tokens to them. Does not
    /// support different token sizes.
    /// </summary>
    public class SDTokenizer : ITokenizer
    {
        public Token[] Tokenize(TokenizerSettings settings, string text)
        {
            if(settings.MinimalTokenSize != settings.MaximalTokenSize)
            {
                throw new ArgumentException("MinimalTokenSize and MaximalToken size must have " +
                    $"same values, but MinimalTokenSize is {settings.MinimalTokenSize} and " +
                    $"MaximalTokenSize is {settings.MaximalTokenSize}!!!!!!!");
            }
            int tokenSize = settings.MinimalTokenSize; //actually you can use settings.MaximalToken size,
                                                       //they are equals
            List<Token> result = new List<Token>(settings.ResultCapacity);
            Dictionary<string, Token> tokens = new Dictionary<string, Token>();

            for(int i = 0; i <= text.Length - tokenSize; i += tokenSize)
            {
               string tokenValue = text.Substring(i, tokenSize);

                Token token;
                if(tokens.ContainsKey(tokenValue))
                {
                    token = tokens[tokenValue];
                }
                else
                {
                    token = new Token();
                    token.Value = tokenValue;
                    token.SubsequentTokens = new List<Token>[settings.SubsequentTokensCount];
                    for(int j = 0; j < settings.SubsequentTokensCount; j++)
                    {
                        token.SubsequentTokens[j] = new List<Token>();
                    }
                    tokens[tokenValue] = token;
                }
                result.Add(token);
                if(i >= tokenSize)
                {
                    for(int j = 0; j < token.SubsequentTokens.Length; j++)
                    {
                        var startPrevTokenIndex = (i - tokenSize) - tokenSize * j;
                        if(startPrevTokenIndex < 0) continue; //we just not far from start, there is no
                        //previous tokens at this position and subsequent depth
                        string prevTokenValue = text.Substring(startPrevTokenIndex, tokenSize);
                        Token prevToken = tokens[prevTokenValue];
                        prevToken.SubsequentTokens[j].Add(token);
                    }
                }
            }
            return result.ToArray();
        }
    }
}