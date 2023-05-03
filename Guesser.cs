using static Hangman.Utilities;

namespace Hangman{
    public abstract class Guesser{
        public delegate Guesser GuesserCreater(Executioner executioner);
        public Executioner executioner;
        public GameBoard board => executioner.board;

        public Guesser(Executioner executioner)
        {
            this.executioner = executioner;
        }

        public abstract byte GetGuess();
    }

    public class RandomGuesser : Guesser
    {
        Random random = new Random();
        public RandomGuesser(Executioner executioner) : base(executioner){}

        public override byte GetGuess()
        {
            while(true){
                byte guess = (byte)(random.NextSingle() * 26);
                if(!board.IsLetterGuessed(guess)){
                    return guess;
                }
            }
        }

        public static GuesserCreater CreateNew(){
            return (Executioner executioner) => new RandomGuesser(executioner);
        }
    }

    public class WeightedRandomGuesser : Guesser
    {
        Random random = new Random();
        WeightedRandomPool<byte> guessWeights;
        public WeightedRandomGuesser(Executioner executioner, WeightedRandomPool<byte> guessWeights) : base(executioner){
            this.guessWeights = guessWeights;
        }

        public override byte GetGuess()
        {
            while(true){
                byte guess = guessWeights.GetRandom();
                if(!board.IsLetterGuessed(guess)){
                    return guess;
                }
            }
        }

        public static GuesserCreater CreateNew(WeightedRandomPool<byte> guessWeights){
            return (Executioner executioner) => new WeightedRandomGuesser(executioner, guessWeights);
        }
    }

    // Unfinished intellegant guesser
    // public class IntellegantGuesser : Guesser
    // {
    //     public List<Word> validWords;
    //     public IntellegantGuesser(Executioner executioner, WordDictionary dictionary) : base(executioner)
    //     {
    //         validWords = new List<Word>();

    //         for (int i = 0; i < dictionary.words.Length; i++)
    //         {
    //             if(dictionary.words[i].Length == board.revealedWord.Length) validWords.Add(dictionary.words[i]);
    //         }
    //     }

    //     public override byte GetGuess()
    //     {
    //         throw new NotImplementedException();
    //     }
    // }

    public class PlayerGuesser : Guesser
    {
        public PlayerGuesser(Executioner executioner) : base(executioner){}

        public override byte GetGuess()
        {
            while(true){
                Console.WriteLine("Please enter a valid guess.");
                string? input = Console.ReadLine();
                if(input == null) continue;
                if(input.Length != 1) continue;

                return ToByte(input[0]);
            }
        }

        public static GuesserCreater CreateNew(){
            return (Executioner executioner) => new PlayerGuesser(executioner);
        }
    }
}