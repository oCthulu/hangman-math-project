using System.Text.Json.Nodes;
using static Hangman.Utilities;

namespace Hangman{
    public class WordDictionary{
        public WeightedRandomPool<byte>? characterFrequeances;
        public Word[] words;

        public WordDictionary(string json){
            characterFrequeances = new WeightedRandomPool<byte>(new WeightedRandomPoolElement<byte>[26]);
            for (byte i = 0; i < 26; i++) characterFrequeances.elements[i] = new WeightedRandomPoolElement<byte>(i, 0);

            JsonNode root = JsonNode.Parse(json) ?? throw new Exception("Invalid json provided");
            JsonArray dict = (JsonArray)(root["words"] ?? throw new Exception("Could not find key \"words\" in root object."));
            
            int totalFreq = 0;
            foreach(JsonNode? word in dict){
                if(word == null) throw new Exception("Invalid json object in \"word\" array");

                totalFreq += (int)(word["freq"] ?? throw new Exception("Key \"freq\" not found in word")).GetValue<float>();
            }

            words = new Word[dict.Count];

            for (int i = 0; i < dict.Count; i++)
            {
                JsonNode? word = dict[i];
                if (word == null) throw new Exception("Invalid json object in \"word\" array");

                float freq = (word["freq"] ?? throw new Exception("Key \"freq\" not found in word")).GetValue<float>() / totalFreq;

                words[i] = new Word(ToByteString((word["word"] ?? throw new Exception("Key \"word\" not found in word")).GetValue<string>()), freq, this, i);

                foreach(byte ch in words[i].letters){
                    characterFrequeances.elements[ch].weight++;
                }
            }

            characterFrequeances.RecalculateTotalWeight();
        }

        public WordDictionary(WordDictionary copyFrom){
            words = new Word[copyFrom.words.Length];
            copyFrom.words.CopyTo(words, 0);
        }
    }
}