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
        public DirectedGraph<Token> Tokenize(TokenizerSettings settings, string text)
        {
            if(settings.MinimalTokenSize != settings.MaximalTokenSize)
            {

                throw new ArgumentException("MinimalTokenSize and MaximalToken size must have " +
                    $"same values, but MinimalTokenSize is {settings.MinimalTokenSize} and " +
                    $"MaximalTokenSize is {settings.MaximalTokenSize}!!!!!!!");
            }

            int tokenSize = settings.MinimalTokenSize; // actually you can use settings.MaximalToken size, they are equals
            DirectedGraph<Token> graph = new(settings.ResultCapacity);
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
                    token = new Token(tokenValue);

                    tokens[tokenValue] = token;
                    graph.AddVertex(token);
                }

                if(i < tokenSize) continue;

                var prevTokenIndex = i - tokenSize;

                string prevTokenValue = text.Substring(prevTokenIndex, tokenSize);
                Token prevToken = tokens[prevTokenValue];

                graph.AddEdge(prevToken, token);

                const int baseFrequency = 20;
                if(settings.LogDebugInfo)
                {
                    if(i % baseFrequency == 0)
                    {
                        Logger.LogDebug($"token: {token.Value} prevtoken: {prevToken.Value}");
                    }
                }
            }

            if(settings.LogDebugInfo)
            {
                Logger.LogDebug($"Total tokens created: {tokens.Count}\n");
            }

            return graph;
        }
    }
}