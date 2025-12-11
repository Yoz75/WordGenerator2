using System;
using System.Collections.Generic;
using WG2.Logging;

namespace WG2.Tokenization
{
    /// <summary>
    /// Standard Dictionary Tokenizer. Uses Dictionary of tokens to add next tokens to them. Does not
    /// support different token sizes.
    /// </summary>
    public class SDTokenizer : ITokenizer
    {
        public DirectedGraph<Token, int> Tokenize(TokenizerSettings settings, string text)
        {
            if(settings.MinimalTokenSize != settings.MaximalTokenSize)
            {

                throw new ArgumentException("MinimalTokenSize and MaximalToken size must have " +
                    $"same values, but MinimalTokenSize is {settings.MinimalTokenSize} and " +
                    $"MaximalTokenSize is {settings.MaximalTokenSize}!!!!!!!");
            }

            int tokenSize = settings.MinimalTokenSize; // actually you can use settings.MaximalToken size, they are equals
            DirectedGraph<Token, int> graph = new(settings.ResultCapacity);
            Dictionary<string, Token> tokens = [];
            Dictionary<(Token, Token), int> pairCounts = [];

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
                    token = new Token
                    {
                        Value = tokenValue
                    };

                    tokens[tokenValue] = token;
                    graph.AddVertex(token);
                }

                if(i < tokenSize) continue;

                #region makeEdge
                var prevTokenIndex = i - tokenSize;

                string prevTokenValue = text.Substring(prevTokenIndex, tokenSize);
                Token prevToken = tokens[prevTokenValue];

                var edge = (prevToken, token);
                if(!pairCounts.ContainsKey(edge)) pairCounts[edge] = 0;
                pairCounts[edge]++;

                graph.AddEdge(prevToken, token, pairCounts[edge]); ;

                const int baseFrequency = 20;
                if(settings.LogDebugInfo)
                {
                    if(i % baseFrequency == 0)
                    {
                        Logger.Log($"token: {token.Value} prevtoken: {prevToken.Value}", LogType.Debug);
                    }
                }
                #endregion
            }

            if(settings.LogDebugInfo)
            {
                Logger.Log($"Total tokens created: {tokens.Count}\n", LogType.Debug);
            }

            return graph;
        }
    }
}