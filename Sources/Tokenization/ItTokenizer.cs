
using System;
using System.Collections.Generic;
using System.Linq;
using WG2.Logging;

namespace WG2.Tokenization;

/// <summary>
/// Iterative tokenizer. Something like BPE, but not
/// </summary>
public class ItTokenizer : ITokenizer
{
    private readonly struct Pair
    {
        // We need this shit with memory and string because we often use full string in our code,
        // multiple concatenation of Left and Right would be 
        // slower, than concatenate them constructor and use baked value

        public readonly ReadOnlyMemory<char> Left, Right;
        public readonly string FullString;
        private readonly int HashCode;

        public Pair(string left, string right)
        {
            FullString = left + right;

            Left = FullString.AsMemory(0, left.Length);
            Right = FullString.AsMemory(left.Length, right.Length);

            HashCode = FullString.GetHashCode();
        }

        public static bool operator ==(Pair left, Pair right)
        {
            return left.FullString == right.FullString;
        }

        public static bool operator !=(Pair left, Pair right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj) =>
        obj is Pair other && FullString == other.FullString;

        public override int GetHashCode()
        {
            return HashCode;
        }
    }

    private readonly Random Random = new Random();

    public DirectedGraph<Token, int> Tokenize(TokenizerSettings settings, string input)
    {
        DirectedGraph<Token, int> result = new(settings.ResultCapacity);

        // Init raw tokens as list of strings, but each string is a single character from input
        LinkedList<string> rawTokens = new LinkedList<string>(input.Select((ch, _) => { return ch.ToString(); }));

        for(int i = 0; i < settings.ItTokenizerSamples; i++)
        {
            if(settings.LogDebugInfo)
            {
                Logger.Log($"\n----------------------------------------", LogType.Debug);
                Logger.Log($"Iteration: {i}", LogType.Debug);
            }
            else if(i % 100 == 0)
            {
                WGConsole.WriteLine($"\n----------------------------------------");
                WGConsole.WriteLine($"Iteration: {i}");
            }

            Dictionary<Pair, int> pairs = new Dictionary<Pair, int>();

            LinkedListNode<string>? current = rawTokens.First;

            while(current != null)
            {
                // Get next node
                LinkedListNode<string>? next = current.Next;

                // If next node is not null, count pair
                if(next != null)
                {
                    Pair pair = new(current.Value, next.Value);

                    if(pairs.ContainsKey(pair))
                    {
                        pairs[pair]++;
                    }
                    else
                    {
                        pairs[pair] = 1;
                    }

                    if(settings.LogDebugInfo)
                        Logger.Log($"pair: {pair.Left}|{pair.Right} count: {pairs[pair]}", LogType.Debug);
                }

                // Move to next node
                current = next;
            }

            if(pairs.Count <= 2)
            {
                // Red color is meaning something like error, white usually used for some slop info. This isn't slop and isn't an error
                // More like an "important message", so I use blue color here
                WGConsole.WriteLine("there is no more tokens left! (probably tmax is too big)", Color.Blue);
                break;
            }

            if(settings.LogDebugInfo)
                Logger.Log($"Pairs count: {pairs.Count}\n", LogType.Debug);

            Pair selectedPair = SelectPair(settings, pairs);

            if(selectedPair == default) break;
            
            UpdateRawTokens(settings.MaximalTokenSize, rawTokens, selectedPair);            
        }

        ToTokens(result, rawTokens);

        if(settings.LogDebugInfo)
        {
            Logger.Log("Tokenization completed. Total tokens created: {result.Count()}", LogType.Debug);
        }

        return result;
    }


    private Pair SelectPair(TokenizerSettings settings, Dictionary<Pair, int> pairs)
    {
        List<KeyValuePair<Pair, int>> pairsList = pairs.ToList();

        pairsList = pairsList.Where((pair) =>
        {
            return
                pair.Key.FullString.Length <= settings.MaximalTokenSize &&
                pair.Value >= settings.ItTokenizerMinMergeCount;

        }).ToList();

        if(pairsList.Count <= 0)
        {
            WGConsole.WriteLine($"Failed to find suitable token less than {settings.MaximalTokenSize + 1} " +
                $"and more frequent than {settings.ItTokenizerMinMergeCount}. Tokenization completed ahead of schedule " +
                $"(that's normal for small input or big tis values)", Color.Blue);

            return default;
        }

        pairsList.Sort((a, b) =>
        {
            int lengthComparison = b.Key.FullString.Length.CompareTo(a.Key.FullString.Length);
            int countComparison = b.Value.CompareTo(a.Value);

            // Count is more important than length, so we multiply it by 2
            return lengthComparison + countComparison * 2;
        });

        if(settings.LogDebugInfo)
        {
            foreach(var pair in pairsList)
            {
                Logger.Log($"sorted pair: {pair.Key.Left}|{pair.Key.Right} count: {pair.Value}", LogType.Debug);
            }
        }

        var topK = settings.ItTokenizerTopK > pairsList.Count ?
            pairsList.Count : settings.ItTokenizerTopK;

        KeyValuePair<Pair, int> selectedPair = default;
        int index = Random.Next(0, topK);

        selectedPair = pairsList[index];

        if(settings.LogDebugInfo)
        {
            Logger.Log($"Selected built pair: {selectedPair.Key.FullString}\n", LogType.Debug);
        }

        return selectedPair.Key;
    }

    private static void UpdateRawTokens(int maxTokenSize, LinkedList<string> rawTokens, Pair selectedPair)
    {
        var current = rawTokens.First;

        while(current != null)
        {
            LinkedListNode<string>? next = current?.Next;

            if(next != null)
            {
                string newString = current!.Value + next!.Value;
                if(selectedPair.FullString == current.Value + next.Value)
                {
                    current.Value = selectedPair.FullString;

                    rawTokens.Remove(next);
                }
            }

            current = current?.Next;
        }
    }

    private static void ToTokens(DirectedGraph<Token, int> result, LinkedList<string> rawTokens)
    {
        LinkedListNode<string>? current = rawTokens.First;

        Dictionary<string, Token> resultTokens = new Dictionary<string, Token>();
        Dictionary<(Token, Token), int> pairCounts = [];


        while(current != null)
        {
            Token token;

            if(resultTokens.ContainsKey(current.Value))
            {
                token = resultTokens[current.Value];
            }
            else
            {
                token = new Token { Value = current.Value };
                resultTokens[current.Value] = token;
                result.AddVertex(token);
            }

            // If next node is not null, add edge
            LinkedListNode<string>? previous = current.Previous;

            if(previous != null)
            {
                var prevToken = resultTokens[previous.Value];
                var edge = (prevToken, token);
                if(!pairCounts.ContainsKey(edge)) pairCounts[edge] = 0;
                pairCounts[edge]++;

                result.AddEdge(prevToken, token, pairCounts[edge]);
            }

            // Move to next node
            current = current.Next;
        }
    }
}