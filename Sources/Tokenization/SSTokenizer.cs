
using System;
using System.Collections.Generic;

namespace WG2.Tokenization
{
    /// <summary>
    /// Space Slice Tokenizer, slices input string by spaces and creates tokens. Does not use token size.
    /// </summary>
    public class SSTokenizer : ITokenizer
    {
        public DirectedGraph<Token> Tokenize(TokenizerSettings settings, string text)
        {
            DirectedGraph<Token> result = new();

            Dictionary<string, Token> tokens = new Dictionary<string, Token>();

            string[] rawTokens = text.Split(' ');

            for(int i = 0; i < rawTokens.Length; i++)
            {
                string tokenValue = rawTokens[i];
                Token token;

                if(tokens.ContainsKey(tokenValue))
                {
                    token = tokens[tokenValue];
                }
                else
                {
                    token = new Token();
                    token.Value = tokenValue + ' '; //we separated text by ' ' so every space was deleted
                    tokens[tokenValue] = token;
                    result.AddVertex(token);
                }


                if(i >= 1)
                {
                    var startPrevTokenIndex = i - 1;

                    ReadOnlySpan<char> prevTokenValue = rawTokens[startPrevTokenIndex];
                    Token prevToken = tokens[prevTokenValue.ToString()];

                    result.AddEdge(prevToken, token);

                    const int baseFrequency = 20;
                    int tokenLogFrequency = baseFrequency * settings.SubsequentTokensCount;
                    if(settings.LogDebugInfo)
                    {
                        if(i % tokenLogFrequency == 0)
                        {
                            Logger.LogDebug($"token: {token.Value} prevtoken: {prevToken.Value}");
                        }
                    }
                }
            }

            if(settings.LogDebugInfo)
            {
                Logger.LogDebug($"Total tokens created: {result.Count}");
            }
            return result;
        }
    }
}