
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

        public string Generate(GeneratorSettings settings, DirectedGraph<Token> tokens)
        {
            StringBuilder sb = new StringBuilder();
            Token thisToken = null;
            for(int i = 0; i < settings.TokensGenerateCount; i++)
            {
                if(IsBadToken(thisToken, tokens) || Random.NextDouble() < settings.RandomNextTokenChance)
                {
                    PickRandom(ref thisToken, tokens);
                }

                if(settings.LogDebugInfo)
                {
                    sb.Append('|');
                }

                sb.Append(thisToken.Value);

                var adjacentVertices = tokens.GetAdjacentVertices(thisToken);
                thisToken = adjacentVertices[Random.Next(0, adjacentVertices.Count)];
            }
            return sb.ToString();
        }

        private bool IsBadToken(Token token, DirectedGraph<Token> source)
        {
            if(token == null || source.GetAdjacentVertices(token).Count == 0)
            {
                return true;
            }

            return false;
        }

        private void PickRandom(ref Token token, DirectedGraph<Token> source)
        {
            //do while because we want to pick random token even if input token is valid
            do
            {
                token = source.GetRandom(0, source.Count());
            } while(IsBadToken(token, source));
        }
    }
}
