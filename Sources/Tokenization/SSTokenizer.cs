
using System;
using System.Collections.Generic;
using WG2.Logging;

namespace WG2.Tokenization
{
    /// <summary>
    /// Space Slice Tokenizer, slices input string by spaces and creates tokens. Does not use token size.
    /// </summary>
    public class SSTokenizer : ITokenizer
    {
        public DirectedGraph<Token, int> Tokenize(TokenizerSettings settings, string text)
        {
            DirectedGraph<Token, int> result = new(settings.ResultCapacity);
            Dictionary<string, Token> tokens = [];
            Dictionary<(Token, Token), int> pairCounts = [];

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


                if(i < 1) continue;

                var startPrevTokenIndex = i - 1;

                string prevTokenValue = rawTokens[startPrevTokenIndex];
                Token prevToken = tokens[prevTokenValue];

                var edge = (prevToken, token);
                if(!pairCounts.ContainsKey(edge)) pairCounts[edge] = 0;
                pairCounts[edge]++;

                result.AddEdge(prevToken, token, pairCounts[edge]);

                const int baseFrequency = 20;
                if(settings.LogDebugInfo)
                {
                    if(i % baseFrequency == 0)
                    {
                        Logger.Log($"token: {token.Value} prevtoken: {prevToken.Value}", LogType.Debug);
                    }
                }
            }

            if(settings.LogDebugInfo)
            {
                Logger.Log($"Total tokens created: {result.Count}", LogType.Debug);
            }
            return result;
        }
    }
}