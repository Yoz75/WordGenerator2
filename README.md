Word generator 2 (wg2 shortly) is a token based text generator. A tokenizer slices input text to small pieces (tokens) and remembers wich tokens go after wich. After a generator selects random token and creates next tokens in the chain. <br>
**THE MORE TEXT PROVIDED AS INPUT, THE BETTER RESULT WILL BE**. I think an acceptable amount would be 500-600 kilobytes

![Снимок экрана 2025-01-08 224531](https://github.com/user-attachments/assets/d55e4d41-ea22-4e61-99ff-219b34c2f37e)

![Снимок экрана 2025-01-08 224720](https://github.com/user-attachments/assets/0fe4c97f-32cf-4a82-a915-364d64230077)

# Arguments 
Wg2 has some arguments:
* source={sourcefile} -- read source text from Sources/{sourcefile], wg2 read input from console without this argument
* ts={size} -- set token size to {size}, as smaller the {size}, as smaller pieces of text will create tokenizer and vice versa
* tmin={size} -- set minimal token size to {size}, created to support tokenizers with random token size
* tmax={size} -- set maximal token size to {size}, created to support tokenizers with random token size
* tsc={count} -- set subsequent tokens count to {count}. This number is means, how many tokens after itself a token will "remember". I. e this value specifies size of List<Token>[] SubsequentTokens array (SubsequentTokens = new List<Token>[count]. For example, when tsc=5, Token will remember tokens at 5 next positions.
* tn={count} -- count of next tokens, that token will remebmer. For example if tsc=5 and tn=10, token will remember 10 tokens at 5 next positions
* tg={count} -- generate {count} tokens. Do not confuse characters and tokens. If ts=2 and tg=200, wg2 will generate 400 symbols
* tr={chance} -- chance (from 0$ to 100%), that next token will be chosen randomly from all tokens
* tsp -- separate tokens with spaces, potentially works better with analytic languages (like english)
* fr={count} -- process input to tokens, generate response and tokenize it again ({count} times)
* hp -- set high process priority (requires admin rights!)
* debinf -- log debug info

# Standard source files
* abracadabra.txt -- just abracadabra lol
* words.txt-- just some russian words, separated with comma
* englishWords.txt -- google translated words.txt
* source.txt -- the biggest standard source file, contains very much russian text (817KB)
