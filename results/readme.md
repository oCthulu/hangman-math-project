# results
This is the directory with the results, stored in custom `.hgr` (which stands for "hangman game record") files.
## File Format
The file begins with a 4 byte signed integer which specify the amount of games in the file. This is immedately followed by the data for the games. The data for each game is stored as follows:
 - A 4 byte signed integer storing the word for the game. This is the index into the words defined in the `/words.json` file
 - A single byte (8 bit unsigned integer) specifying the game length (how many times the guesser guessed)
 - 1 byte (8 bit unsigned integer) for each time the guesser guessed which specifies what letter the guesser guessed. 0 means 'a', 1 means 'b', 2 means 'c', ect...
The next game is stored immeadtely after with no delimeter between them. This continues untill the end of the file