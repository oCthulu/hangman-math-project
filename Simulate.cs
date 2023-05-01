using System.Diagnostics;
using static Hangman.Utilities;
using System.Threading;

namespace Hangman.Simulation{
    public static class Simulation{
        public static void DoSimulations(int gameCount, string? file, Guesser.GuesserCreater guesserCreater, Executioner.ExecutionerCreater executionerCreater){
            GameRecord[] games = new GameRecord[gameCount];

            Stopwatch gamesTimer = new Stopwatch();

            Console.WriteLine($"Simulating {gameCount} games...");

            int i = 0;

            Thread t = new Thread(() => {   
                gamesTimer.Start();

                for (i = 0; i < gameCount; i++)
                {
                    Executioner executioner = executionerCreater();
                    Guesser guesser = guesserCreater(executioner);

                    Game game = new Game(executioner as WordBasedExecutioner ?? throw new Exception("Executioner must be a word based executioner"), guesser);

                    game.DoGame();

                    games[i] = game.Record;
                }

                gamesTimer.Stop();
            });

            t.Start();

            while(t.ThreadState == System.Threading.ThreadState.Running){
                string str = $"Simulated game {i+1}/{gameCount} ({Math.Floor((((float)i+1)/gameCount) * 100)}%)";
                if(str.Length < Console.BufferWidth) str += new String(' ', Console.BufferWidth - str.Length);
                Console.Write("\r");
                Console.Write(str);
            }

            Console.WriteLine();
            Console.WriteLine($"Simulated {gameCount} games in {FormatNumberUnit(gamesTimer.Elapsed.TotalSeconds, "s")} (avg. {FormatNumberUnit(gamesTimer.Elapsed.TotalSeconds / gameCount, "s")} per game)");

            if(file != null){
                Console.WriteLine("Writing file.");

                // string path = "games";

                // int suffix = 0;

                // while(File.Exists($"{path}_{suffix}.hgr")) suffix++;

                using(FileStream stream = File.Open($"results/{file}.hgr", FileMode.CreateNew)){
                    stream.Write(BitConverter.GetBytes(gameCount));

                    foreach(GameRecord game in games){
                        stream.Write(BitConverter.GetBytes(game.word.index ?? throw new Exception("Can only save game with words that are a member of a dictionary")));
                        stream.WriteByte((byte)game.guesses.Count);
                        stream.Write(game.guesses.ToArray());
                    }
                }

                Console.WriteLine("Successfully wrote file");
            }
        }
    }
}