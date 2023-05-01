using System.Collections;

namespace Hangman{
    public class GameBoard{
        public byte?[] revealedWord;

        public GameBoard(byte?[] revealedWord)
        {
            this.revealedWord = revealedWord;
        }

        private int whitelistLetters;
        private int blacklistLetters;

        //letters which are known to be in the word
        public int WhitelistLetters { get => whitelistLetters; }
        //letters known not to be in the word
        public int BlacklistLetters { get => blacklistLetters; }
        //guessed letters
        public int GuessedLetters { get => whitelistLetters | blacklistLetters; }
        
        public void MarkLetterWhitelist(byte letter){
            whitelistLetters |= 1 << letter;
        }

        public void MarkLetterBlacklist(byte letter){
            blacklistLetters |= 1 << letter;
        }

        public bool IsLetterWhitelist(byte letter){
            return ((whitelistLetters >> letter) & 1) == 1;
        }

        public bool IsLetterBlacklist(byte letter){
            return ((blacklistLetters >> letter) & 1) == 1;
        }

        public bool IsLetterGuessed(byte letter){
            return ((GuessedLetters >> letter) & 1) == 1;
        }
    }

    public struct Word{
        public WordDictionary? dictionary;
        public int? index;
        public readonly byte[] letters;
        public readonly float randomWeight;
        public readonly int containedLetters;

        public int Length {get => letters.Length;}

        public Word(byte[] letters, float randomWeight)
        {
            this.letters = letters;
            this.randomWeight = randomWeight;
            
            foreach(byte letter in letters){
                containedLetters |= 1 << letter;
            }
        }

        public Word(byte[] letters, float randomWeight, WordDictionary dictionary, int index)
        {
            this.letters = letters;
            this.randomWeight = randomWeight;

            this.dictionary = dictionary;
            this.index = index;
            
            foreach(byte letter in letters){
                containedLetters |= 1 << letter;
            }
        }

        public bool Contains(byte letter){
            return ((containedLetters >> letter) & 1) == 1;
        }

        public byte this[int i] {get{
            return letters[i];
        }}
    }

    public struct GameRecord{
        public Word word;
        public List<Byte> guesses;
    }

    public class Game{
        public WordBasedExecutioner executioner;
        public Guesser guesser;
        public GameBoard Board => executioner.board; 
        public GameRecord Record => record;
        GameRecord record;

        public Game(WordBasedExecutioner executioner, Guesser guesser)
        {
            this.executioner = executioner;
            this.guesser = guesser;

            record = new GameRecord(){
                word = executioner.word,
                guesses = new List<byte>(26)
            };
        }

        public void DoGame(){
            while(true){
                ProcessTurn();
                if(executioner.GuesserHasWon()) return;
            }
        }

        public void ProcessTurn(){
            byte guess = guesser.GetGuess();
            record.guesses.Add(guess);

            executioner.Guess(guess);
        }
    }

    // public interface IWeightedRandomPoolElement{
    //     float weight {get; set;}
    // }

    public class WeightedRandomPoolElement<T>{
        
        public float weight;
        public T value;

        public WeightedRandomPoolElement(T value, float weight)
        {
            this.weight = weight;
            this.value = value;
        }

        public bool Equals(WeightedRandomPoolElement<T>? other)
        {
            return this == other;
        }
    }

    public class WeightedRandomPool<T>{
        public Random random = new Random();
        public WeightedRandomPoolElement<T>[] elements;
        private float totalWeight;

        public float TotalWeight { get => totalWeight; }

        public T GetRandom(){
            return GetValueAt(random.NextSingle() * TotalWeight);
        }

        public T GetValueAt(float value){
            foreach(WeightedRandomPoolElement<T> element in elements){
                if(value < element.weight) return element.value;
                value -= element.weight;
            }

            throw new ArgumentException("Argument \"value\" is above TotalWeight.");
        }

        public WeightedRandomPool(WeightedRandomPoolElement<T>[] elements)
        {
            this.elements = elements;
        }

        public void RecalculateTotalWeight(){
            totalWeight = 0;
            foreach(WeightedRandomPoolElement<T> element in elements){
                totalWeight += element.weight;
            }
        }
    }

    // public class WeightedRandomPoolEnumerator<T> : IEnumerator<T> where T : IWeightedRandomPoolElement, IEquatable<T>
    // {
    //     public T Current => (current as WeightedRandomPool<T>.ValueNode ?? throw new InvalidOperationException("Cannot get value of WeightedRandomPoolEnumerator positioned before the start")).value;

    //     object IEnumerator.Current => throw new NotImplementedException();

    //     WeightedRandomPool<T>.StarterNode start;
    //     WeightedRandomPool<T>.Node current;

    //     public WeightedRandomPoolEnumerator(WeightedRandomPool<T>.StarterNode start)
    //     {
    //         this.start = start;
    //         this.current = start;
    //     }

    //     public void Dispose(){}

    //     public bool MoveNext()
    //     {
    //         current = current.next ?? throw new InvalidOperationException("Tried to move past end of list");
    //         return current.next != null;
    //     }

    //     public void Reset()
    //     {
    //         current = start;
    //     }
    // }

    // public class WeightedRandomPool<T> : ICollection<T> where T : IWeightedRandomPoolElement, IEquatable<T>{
    //     public abstract class Node{
    //         protected Node(ValueNode? next)
    //         {
    //             this.next = next;
    //         }
            
    //         public virtual ValueNode? next { get; set; }
    //     }

    //     public class StarterNode : Node
    //     {
    //         public StarterNode(ValueNode? next) : base(next){}
    //     }

    //     public class ValueNode : Node
    //     {
    //         public T value;
    //         public Node previous;

    //         public ValueNode(Node previous, ValueNode? next, T value) : base(next)
    //         {
    //             this.previous = previous;
    //             this.value = value;
    //         }
    //     }

    //     StarterNode starter;
    //     public Node Last => last;
    //     Node last;
    //     public int Count => count;
    //     int count = 0;
    //     public float TotalFrequeancy => totalFrequeancy;
    //     float totalFrequeancy;

    //     public WeightedRandomPool()
    //     {
    //         starter = new StarterNode(null);
    //         last = starter;
    //     }

    //     public bool IsReadOnly => false;

    //     public T GetValueAt(float input){
    //         foreach(T value in this){
    //             if(value.weight > input)
    //                 input -= value.weight;
    //             else
    //                 return value;
    //         }

    //         throw new ArgumentException("Argument \"input\" is above TotalFrequeancy");
    //     }

    //     public void Add(T item)
    //     {
    //         last.next = new ValueNode(last, null, item);
    //         last = last.next;

    //         count++;
    //     }

    //     public void Clear()
    //     {
    //         starter = new StarterNode(null);
    //         last = starter;
    //     }

    //     public bool Contains(T item)
    //     {
    //         foreach(T value in this){
    //             if(value.Equals(item)) return true;
    //         }

    //         return false;
    //     }

    //     public void CopyTo(T[] array, int arrayIndex)
    //     {
    //         foreach(T value in this){
    //             array[arrayIndex] = value;
    //             arrayIndex++;
    //         }
    //     }

    //     public bool Remove(T item)
    //     {
    //         if(starter.next == null) return false;

    //         ValueNode current = starter.next;

    //         while(true){
    //             if(current.Equals(item)){
    //                 current.previous.next = current.next;
    //                 if(current.next != null) current.next.previous = current.previous;
    //                 count--;
    //                 return true;
    //             }

    //             if(current.next == null) break;
    //             current = current.next;
    //         }

    //         return false;
    //     }

    //     public IEnumerator<T> GetEnumerator()
    //     {
    //         return new WeightedRandomPoolEnumerator<T>(starter);
    //     }

    //     IEnumerator IEnumerable.GetEnumerator()
    //     {
    //         return GetEnumerator();
    //     }
    // }
}