
using System;
using System.Text;

namespace WG2.Generation
{
    /// <summary>
    /// Standard Generator.
    /// </summary>
    public class SGenerator : IGenerator
    {
        private Random Random = new Random();
        private bool IsBadToken(Token token)
        {
            if(token == null)
            {
                return true;
            }
            for(int i = 0; i < token.SubsequentTokens.Length; i++)
            {
                if(token.SubsequentTokens[i] == null)
                {
                    return true;
                }
            }
            return false;
        }

        private void PickRandom(ref Token token, Token[] sourceArray)
        {
            do
            {
                token = sourceArray[Random.Next(0, sourceArray.Length)];
            } while(IsBadToken(token));
        }

        private Token GetNextToken(Token token,int subTokenIndex, int nextTokensCount, Token[] grabIfNull)
        {
            int nextTokenIndex = 0;
            nextTokenIndex = Random.Next(0, nextTokensCount);
            if(nextTokenIndex >= token.SubsequentTokens[subTokenIndex].Count)
            {
                nextTokenIndex = token.SubsequentTokens[subTokenIndex].Count - 1;
            }
            if(nextTokenIndex < 0)
            {
                PickRandom(ref token, grabIfNull);
                return token;
            }
            return token.SubsequentTokens[subTokenIndex][nextTokenIndex];
        }

        public string Generate(GeneratorSettings settings, Token[] tokens)
        {
            StringBuilder sb = new StringBuilder();
            Token thisToken = null;
            for(int i = 0; i < settings.TokensGenerateCount; i++)
            {
                if(IsBadToken(thisToken) || Random.NextDouble() < settings.RandomNextTokenChance)
                {
                    PickRandom(ref thisToken, tokens);
                    sb.Append(thisToken.Value);
                }

                Token nextToken = thisToken;
                for(int j = 0; j < settings.SubsequentTokensCount; j++)
                {
                    nextToken = GetNextToken(thisToken, j, settings.NextTokensCount, tokens);
                    if(settings.LogDebugInfo)
                    {
                        Logger.LogDebug($"this token: {thisToken.Value} next token: {nextToken.Value} token dimension: {j}");
                    }
                    sb.Append(nextToken.Value);
                    if(j == settings.SubsequentTokensCount - 1)
                    { 
                        i += j; //we generated some tokens, so add generated tokens count to i
                        thisToken = nextToken;
                    }
                }

            }
            return sb.ToString();
        }
    }
}
