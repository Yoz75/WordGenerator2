
using System;
using System.Collections.Generic;

namespace WG2.Tokenization
{
    /// <summary>
    /// Random [token size] Dictionary Tokenizer. 
    /// Uses Dictionary of tokens to add next tokens to them. Supports different token sizes.
    /// </summary>
    public class RDTokenizer : ITokenizer
    {
        public DirectedGraph<Token> Tokenize(TokenizerSettings settings, string text)
        {
            DirectedGraph<Token> result = new(settings.ResultCapacity);

            Dictionary<string, Token> tokens = new Dictionary<string, Token>();

            //we need this to know sizes of previous tokens (cuz it random every time)
            int prevTokenSize = 0;


            Random random = new Random();
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
                    token = new Token(tokenValue);
                    tokens[tokenValue] = token;
                    result.AddVertex(token);
                }


                //that means we generated at least 1 token
                if(i < settings.MinimalTokenSize) goto FrameEnd; // GOTO \>_</

                var startPrevTokenIndex = i - prevTokenSize;

                string prevTokenValue = text.Substring(startPrevTokenIndex, prevTokenSize);
                Token prevToken = tokens[prevTokenValue];

                result.AddEdge(prevToken, token);

                const int baseFrequency = 20;
                if(settings.LogDebugInfo)
                {
                    if(i % baseFrequency == 0)
                    {
                        Logger.LogDebug($"token: {token.Value} prevtoken: {prevToken.Value}");
                    }
                }

            FrameEnd:
                i += tokenSize;
                prevTokenSize = tokenSize;
            }
            if(settings.LogDebugInfo)
            {
                Logger.LogDebug($"Total tokens created: {result.Count}\n");
            }

            return result;
        }
    }
}
