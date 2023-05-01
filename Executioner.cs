using static Hangman.Utilities;

namespace Hangman{
    public abstract class Executioner{
        public delegate Executioner ExecutionerCreater();
        public readonly GameBoard board;

        protected Executioner(GameBoard board)
        {
            this.board = board;
        }

        public abstract bool Guess(byte guess);
        public abstract bool GuesserHasWon();
        public virtual void Init(){}
    }

    public class WordBasedExecutioner : Executioner
    {
        public virtual Word word {get; set;}

        public override bool Guess(byte guess)
        {
            if(board.IsLetterGuessed(guess)) throw new InvalidOperationException($"Letter ${guess} is invalid because it has already been guessed");

            if(word.Contains(guess)){
                board.MarkLetterWhitelist(guess);
                for (int i = 0; i < word.Length; i++)
                {
                    if(word[i] == guess) board.revealedWord[i] = guess;
                }
                return true;
            }
            else{
                board.MarkLetterBlacklist(guess);
                return false;
            }
        }

        public override bool GuesserHasWon()
        {
            return (board.GuessedLetters & word.containedLetters) == word.containedLetters;
        }

        protected WordBasedExecutioner(Word word) : base(new GameBoard(new byte?[word.Length])){
            this.word = word;
        }

        public static ExecutionerCreater CreateNew(Word word){
            return () => new WordBasedExecutioner(word);
        }
    }

    public class RandomWordBasedExecutioner : WordBasedExecutioner{
        public WordDictionary dictionary;

        public RandomWordBasedExecutioner(WordDictionary dictionary) : base(dictionary.words[(int)(Random.Shared.NextSingle() * dictionary.words.Length)]){
            this.dictionary = dictionary;
        }

        public static ExecutionerCreater CreateNew(WordDictionary word){
            return () => new RandomWordBasedExecutioner(word);
        }
    }

    public class PlayerWordBasedExecutioner : WordBasedExecutioner
    {
        public PlayerWordBasedExecutioner() : base(GetWord()){}

        public static Word GetWord(){
            while(true){
                Console.WriteLine("Please enter a word");
                string? input = Console.ReadLine();

                if(input == null) continue;

                return new Word(ToByteString(input), 0);
            }
        }

        public static ExecutionerCreater CreateNew(){
            return () => new PlayerWordBasedExecutioner();
        }
    }
}