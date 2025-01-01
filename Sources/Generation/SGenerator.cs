
using System;
using System.Collections.Generic;
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

        private Token GetNextToken(Token token, int nextTokensCount, Token[] grabIfNull)
        {
            int nextTokenIndex = 0;
            nextTokenIndex = Random.Next(0, nextTokensCount);
            if(nextTokenIndex >= token.SubsequentTokens[0].Count)
            {
                nextTokenIndex = token.SubsequentTokens[0].Count - 1;
            }
            if(nextTokenIndex < 0)
            {
                PickRandom(ref token, grabIfNull);
                return token;
            }
            return token.SubsequentTokens[0][nextTokenIndex];
        }

        public string Generate(GeneratorSettings settings, Token[] tokens)
        {
            StringBuilder sb = new StringBuilder();
            Token thisToken = null;
            Queue<Token> previousTokens = new Queue<Token>();
            for(int i = 0; i < settings.SubsequentTokensCount; i++)
            {
                previousTokens.Enqueue(null);
            }
            for(int i = 0; i < settings.TokensGenerateCount; i++)
            {
                if(IsBadToken(thisToken) || Random.Next(0, 100) < settings.RandomNextTokenChance)
                {
                    PickRandom(ref thisToken, tokens);
                }

                sb.Append(thisToken.Value);

                Token nextToken = GetNextToken(thisToken, settings.NextTokensCount, tokens);
                if(i != 0)
                {
                    int nextTokenAppearances = 0;
                    do
                    {
                        nextToken = GetNextToken(thisToken, settings.NextTokensCount, tokens);
                        nextTokenAppearances = 0;
                        for(int j = previousTokens.Count - 1; j >= 0; j--)
                        {
                            Token previousToken = previousTokens.ToArray()[j]; //oh god, that is such bullshit
                            if(previousToken == null)
                            {
                                //if previous token is null, we can neglect it
                                nextTokenAppearances++;
                                continue;
                            }
                            if(previousToken.SubsequentTokens[j].Contains(nextToken))
                            {
                                nextTokenAppearances++;
                            }
                        }
                    } while(nextTokenAppearances < settings.MinimalNextTokenAppearances);
                }
                previousTokens.Dequeue();
                previousTokens.Enqueue(thisToken);
                thisToken = nextToken;
            }
            return sb.ToString();
        }
    }
}
