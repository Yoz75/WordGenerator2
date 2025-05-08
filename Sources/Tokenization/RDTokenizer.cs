
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
        public Token[] Tokenize(TokenizerSettings settings, string text)
        {
            List<Token> result = new List<Token>(settings.ResultCapacity);
            Dictionary<string, Token> tokens = new Dictionary<string, Token>();
            //we need this to know sizes of previous tokens (cuz it random every time)
            Stack<int> prevTokenSizes = new Stack<int>(settings.SubsequentTokensCount);
            for(int j = 0; j < prevTokenSizes.Capacity; j++) prevTokenSizes.Push(0);


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

                //that means we generated at least 1 token
                if(i >= settings.MinimalTokenSize)
                {
                    var prevTokensArray = prevTokenSizes.ToArray();
                    for(int j = 0; j < token.SubsequentTokens.Length; j++)
                    {
                        //every token is random-sized, so we add all sizes to 1 variable
                        int prevSymbolsCount = 0;
                        for(int k = 0; k < prevTokensArray.Length; k++)
                        {
                            if(j == 0) break;
                            prevSymbolsCount += prevTokensArray[k];
                        }

                        var startPrevTokenIndex = (i - prevTokensArray[0]) - prevSymbolsCount;
                        //we just not far from start, there is no
                        //previous tokens at this position and subsequent depth
                        if(startPrevTokenIndex < 0 || prevTokensArray[j] <= 0) continue;
                        string prevTokenValue = text.Substring(startPrevTokenIndex, prevTokensArray[j]);
                        Token prevToken = tokens[prevTokenValue];
                        prevToken.SubsequentTokens[j].Add(token);

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

                i += tokenSize;
                prevTokenSizes.Pop();
                prevTokenSizes.Push(tokenSize);
            }
            if(settings.LogDebugInfo)
            {
                Logger.LogDebug($"Total tokens created: {result.Count}\n");
            }

            return result.ToArray();
        }
    }
}
