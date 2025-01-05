
using System.Collections.Generic;
using System;

namespace WG2.Tokenization
{
    /// <summary>
    /// Space Slice Tokenizer, slices input string by spaces and creates tokens. Does not use token size.
    /// </summary>
    public class SSTokenizer : ITokenizer
    {
        public Token[] Tokenize(TokenizerSettings settings, string text)
        {
            List<Token> result = new List<Token>(settings.ResultCapacity);
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
                    token.SubsequentTokens = new List<Token>[settings.SubsequentTokensCount];
                    for(int j = 0; j < settings.SubsequentTokensCount; j++)
                    {
                        token.SubsequentTokens[j] = new List<Token>();
                    }
                    tokens[tokenValue] = token;
                }
                result.Add(token);
                if(i >= 1)
                {
                    for(int j = 0; j < token.SubsequentTokens.Length; j++)
                    {
                        var startPrevTokenIndex = i - j - 1;
                        if(startPrevTokenIndex < 0) continue; //we just not far from start, there is no
                        //previous tokens at this position and subsequent depth
                        string prevTokenValue = rawTokens[startPrevTokenIndex];
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
            }
                        
            if(settings.LogDebugInfo)
            {
                Logger.LogDebug($"Total tokens created: {result.Count}");
            }
            return result.ToArray();
        }
    }
}