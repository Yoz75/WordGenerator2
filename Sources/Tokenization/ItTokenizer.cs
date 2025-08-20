
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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

    public DirectedGraph<Token> Tokenize(TokenizerSettings settings, string input)
    {
        DirectedGraph<Token> result = new DirectedGraph<Token>(settings.ResultCapacity);

        // Init raw tokens as list of strings, but each string is a single character from input
        LinkedList<string> rawTokens = new LinkedList<string>(input.Select((ch, _) => { return ch.ToString(); }));

        for(int i = 0; i < settings.ItTokenizerSamples; i++)
        {
            if(settings.LogDebugInfo)
            {
                Logger.LogDebug($"\n----------------------------------------");
                Logger.LogDebug($"Iteration: {i}");
            }
            else if(i % 100 == 0)
            {
                Logger.LogMessage($"\n----------------------------------------");
                Logger.LogMessage($"Iteration: {i}");
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
                        Logger.LogDebug($"pair: {pair.Left}|{pair.Right} count: {pairs[pair]}");
                }

                // Move to next node
                current = next;
            }

            if(pairs.Count <= 2)
            {
                Logger.LogError("there is no more tokens left! (probably tmax is too big)");
                break;
            }

            if(settings.LogDebugInfo)
                Logger.LogDebug($"Pairs count: {pairs.Count}\n");

            List<KeyValuePair<Pair, int>> pairsList = pairs.ToList();
            pairsList.Sort(
                (a, b) =>
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
                    Logger.LogDebug($"sorted pair: {pair.Key.Left}|{pair.Key.Right} count: {pair.Value}");
                }
            }

            settings.ItTokenizerTopK = settings.ItTokenizerTopK > pairsList.Count ?
                pairsList.Count : settings.ItTokenizerTopK;

            int index = Random.Next(0, settings.ItTokenizerTopK);

            int j = 0;
            KeyValuePair<Pair, int> selectedPair = default;
            const int maxTokenFindingIterations = 1000;

            do
            {
                if(j >= maxTokenFindingIterations || index + j >= pairsList.Count)
                {
                    Logger.LogError($"Failed to find suitable token less than {settings.MaximalTokenSize + 1} " +
                        $"and more frequent than {settings.ItTokenizerMinMergeCount} " +
                        $"times after {maxTokenFindingIterations} iterations. " +
                        $"(that's normal for small input or big tis values)");
                    break;
                }
                selectedPair = pairsList[index + j];

                j++;
            } while(selectedPair.Key.FullString.Length > settings.MaximalTokenSize
            || selectedPair.Value < settings.ItTokenizerMinMergeCount);

            if(settings.LogDebugInfo)
            {
                Logger.LogDebug($"Selected built pair: {selectedPair.Key.FullString}\n");
            }

            UpdateRawTokens(settings.MaximalTokenSize, rawTokens, selectedPair);
        }

        ToTokens(result, rawTokens);

        if(settings.LogDebugInfo)
        {
            Logger.LogDebug("Tokenization completed. " +
                $"Total tokens created: {result.Count()}");
        }

        return result;
    }

    private static void UpdateRawTokens(int maxTokenSize, LinkedList<string> rawTokens, KeyValuePair<Pair, int> selectedPair)
    {
        var current = rawTokens.First;

        while(current != null)
        {
            LinkedListNode<string>? next = current?.Next;

            if(next != null)
            {
                int newLength = current!.Value.Length + next.Value.Length;
                string newString = current!.Value + next!.Value;
                if(selectedPair.Key.FullString == current.Value + next.Value && newLength <= maxTokenSize)
                {
                    current.Value = selectedPair.Key.FullString;
                    //current = next.Next;
                    rawTokens.Remove(next);
                }
            }

            current = current?.Next;
        }
    }

    private static void ToTokens(DirectedGraph<Token> result, LinkedList<string> rawTokens)
    {
        LinkedListNode<string>? current = rawTokens.First;

        Dictionary<string, Token> resultTokens = new Dictionary<string, Token>();

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
                result.AddEdge(resultTokens[previous.Value], token);
            }

            // Move to next node
            current = current.Next;
        }
    }
}