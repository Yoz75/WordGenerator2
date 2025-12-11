
using System;
using System.Collections.Generic;
using WG2.Logging;

namespace WG2.Tokenization
{
    /// <summary>
    /// Random [token size] Dictionary Tokenizer. 
    /// Uses Dictionary of tokens to add next tokens to them. Supports different token sizes.
    /// </summary>
    public class RDTokenizer : ITokenizer
    {
        public DirectedGraph<Token, int> Tokenize(TokenizerSettings settings, string text)
        {
            DirectedGraph<Token, int> result = new(settings.ResultCapacity);
            Dictionary<(Token, Token), int> pairCounts = [];

            for(int j = 0; j < settings.RandomIterations; j++)
            {
                Dictionary<string, Token> tokens = new Dictionary<string, Token>();

                Random random = new Random();

                //we need this to know sizes of previous tokens (cuz it random every time)
                int prevTokenSize = 0;

                int i = 0;
                int tokenSize = 0;

                while(i < text.Length - settings.MaximalTokenSize)
                {
                    tokenSize = random.Next(settings.MinimalTokenSize, settings.MaximalTokenSize);
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
                        tokens[tokenValue] = token;
                        result.AddVertex(token);
                    }


                    //that means we generated at least 1 token
                    if(i < settings.MinimalTokenSize) goto FrameEnd; // GOTO \>_</

                    #region makeEdge
                    var startPrevTokenIndex = i - prevTokenSize;

                    string prevTokenValue = text.Substring(startPrevTokenIndex, prevTokenSize);
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
                    #endregion

                FrameEnd:
                    i += tokenSize;
                    prevTokenSize = tokenSize;
                }

                if(settings.LogDebugInfo)
                {
                    Logger.Log($"Total tokens created per iteration: {tokens.Count}", LogType.Debug);
                }
            }

            return result;
        }
    }
}
