using System.Text.Json;
using static Hangman.Utilities;

namespace Hangman.DataAnalysis{
    [System.Serializable]
    public class AnalysedData{
        public int gameCount { get; set; }
        public int[] totalGamesOfLength { get; set; }
        public int[] totalGamesOfMistakes { get; set; }
        public int[][] totalGamesOfLengthByWordLength { get; set; }
        public int[][] totalGamesOfMistakesByWordLength { get; set; }

        public AnalysedData(int gameCount, int[] totalGamesOfLength, int[] totalGamesOfMistakes, int[,] totalGamesOfLengthByWordLength, int[,] totalGamesOfMistakesByWordLength)
        {
            this.gameCount = gameCount;
            this.totalGamesOfLength = totalGamesOfLength;
            this.totalGamesOfMistakes = totalGamesOfMistakes;
            this.totalGamesOfLengthByWordLength = ToDoubleArray(totalGamesOfLengthByWordLength);
            this.totalGamesOfMistakesByWordLength = ToDoubleArray(totalGamesOfMistakesByWordLength);
        }
    }

    public static class Analysis{
        public static void AnalyseData(string file, string destinationDirectory, WordDictionary words){
            Console.WriteLine($"Reading file {file}.hgr");
            GameRecord[] games = ReadFile("results/" + file + ".hgr", words);

            Console.WriteLine("Analysing data...");
            //maximun possible game length is 26
            int[] totalGamesOfLength = new int[26];
            int[] totalGamesOfMistakes = new int[26];

            int longestWordLength = words.words.MaxBy((Word word) => word.Length).Length;

            // int[] totalGamesOfWordLength = new int[longestWordLength];
            // int[] totalMistakesOfWordLength = new int[longestWordLength];
            // int[] totalGameLengthsOfWordLength = new int[longestWordLength];

            int[,] totalGamesOfLengthByWordLength = new int[longestWordLength, 26];
            int[,] totalGamesOfMistakesByWordLength = new int[longestWordLength, 26];

            foreach(GameRecord game in games){
                totalGamesOfLength[game.guesses.Count - 1]++;

                int mistakeCount = 0;
                foreach(byte guess in game.guesses){
                    if(!game.word.Contains(guess)) mistakeCount++;
                }

                totalGamesOfMistakes[mistakeCount]++;

                totalGamesOfLengthByWordLength[game.word.Length - 1, game.guesses.Count - 1]++;
                totalGamesOfMistakesByWordLength[game.word.Length - 1, mistakeCount]++;

                // totalGamesOfWordLength[game.word.Length - 1]++;

                // totalGameLengthsOfWordLength[game.word.Length - 1] += game.guesses.Count;
                // totalMistakesOfWordLength[game.word.Length - 1] += mistakeCount;
            }

            string json = JsonSerializer.Serialize(new AnalysedData(
                games.Length,
                totalGamesOfLength,
                totalGamesOfMistakes,
                totalGamesOfLengthByWordLength,
                totalGamesOfMistakesByWordLength
            ));

            string outPath = destinationDirectory + file + ".json";
            Console.WriteLine($"Writing output to {outPath}");
            File.WriteAllText(outPath, json);
        }

        public static GameRecord[] ReadFile(string file, WordDictionary words){
            using(FileStream fs = File.Open(file, FileMode.Open)){
                byte[] buffer = new Byte[4];

                fs.Read(buffer);
                int gameCount = BitConverter.ToInt32(buffer);

                GameRecord[] games = new GameRecord[gameCount];

                for (int i = 0; i < gameCount; i++)
                {
                    //read word index
                    fs.Read(buffer);
                    int word = BitConverter.ToInt32(buffer);

                    byte[] guesses = new Byte[fs.ReadByte()];
                    fs.Read(guesses);

                    games[i] = new GameRecord(){
                        word = words.words[word],
                        guesses = new List<byte>(guesses)
                    };
                }

                return games;
            }
        }
    }
}