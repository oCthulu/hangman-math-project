using Hangman;
using Hangman.Simulation;
using Hangman.DataAnalysis;
using System.Diagnostics;


internal class Program
{
    private static void Main(string[] args)
    {
        WordDictionary words = new WordDictionary(File.ReadAllText("words.json"));

        string? file = null;
        Guesser.GuesserCreater guesserCreater = RandomGuesser.CreateNew();
        Executioner.ExecutionerCreater executionerCreater = RandomWordBasedExecutioner.CreateNew(words);

        string outPath = "";

        for (int i = 0; i < args.Length - 1; i++)
        {
            string arg = args[i];
            string val = args[i+1];

            if(arg == "--file") file = val;
            else if(arg == "--guesser"){
                switch(val){
                    case "random":
                        guesserCreater = RandomGuesser.CreateNew();
                        break;
                    case "weighted":
                        guesserCreater = WeightedRandomGuesser.CreateNew(words.characterFrequeances);
                        break;
                    default:
                        throw new Exception($"\"{args[i+1]}\" is not a valid guesser type");
                }
            }
            else if(arg == "--outpath") outPath = val;
        }

        if(args.Contains("--analyse")){
            Analysis.AnalyseData(file ?? throw new Exception("Please specify a file with --file"), outPath, words);
        }
        else{
            const int gameCount = 1000000;
            Simulation.DoSimulations(gameCount, file, guesserCreater, executionerCreater);
        }
    }
}



// while(true){
//     foreach(byte? letter in board.revealedWord){
//         Console.Write(letter == null? '_' : FromByte(letter.GetValueOrDefault()));
//     }
//     Console.WriteLine();

//     if(executioner.GuesserHasWon()) break;

//     executioner.Guess(guesser.GetGuess());
// }