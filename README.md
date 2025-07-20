Word generator 2 (wg2 shortly) is a token based text generator. A tokenizer slices input text to small pieces (tokens) and remembers wich tokens go after wich. After a generator selects random token and creates next tokens in the chain. <br>
**THE MORE TEXT PROVIDED AS INPUT, THE BETTER RESULT WILL BE**. I think an acceptable amount would be 500-600 kilobytes

![изображение](https://github.com/user-attachments/assets/32de48c1-0cc2-4201-8c4f-fde4e881979c)


> [!NOTE]  
> New version uses [Console Application Interface](https://github.com/Yoz75/CommandAppInterface), to use console arguments, download v1.0 or select console-arguments branch.

Write help command to view commands and their usage.

# Short commands description
* run [path/text] -- if file with that name exists, process file, otherwise process input
* ts [number] -- set token size to [number], as smaller the [number], as smaller pieces of text will create tokenizer and vice versa
* tmin [number] -- set minimal token size to [number], created to support tokenizers with random token size
* tmax [number] -- set maximal token size to [number], created to support tokenizers with random token size
* tg [number] -- generate [number] tokens. Do not confuse characters and tokens. If ts=2 and tg=200, wg2 will generate 400 symbols
* tr [number in range 0..1] -- chance random next token (default 0)
* separate [size/space] -- "space": separate tokens with spaces, potentially works better with analytic languages (like english)<br> "size": default separating tokens to several characters
* fr [number] -- process input to tokens, generate response and tokenize it again ([number] times)
* hp -- set high process priority (requires admin rights!)
* np -- set normal process priority
* debinf [true/false] -- log debug info?

# Standard source files
* abracadabra.txt -- just abracadabra lol
* words.txt-- just some russian words, separated with comma
* englishWords.txt -- google translated words.txt
* source.txt -- the biggest standard source file, contains very much russian text (817KB)
