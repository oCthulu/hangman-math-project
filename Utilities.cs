namespace Hangman{
    public static class Utilities{
        public static byte ToByte(char c){
            return (byte)(c - 97);
        }

        public static char FromByte(byte c){
            return (char)(c + 97);
        }

        public static byte[] ToByteString(string str){
            byte[] bytes = new byte[str.Length];

            for (int i = 0; i < str.Length; i++)
            {
                bytes[i] = ToByte(str[i]);
            }

            return bytes;
        }

        public static T[][] ToDoubleArray<T>(T[,] arr){
            T[][] val = new T[arr.GetLength(0)][];

            for (int x = 0; x < val.Length; x++)
            {
                val[x] = new T[arr.GetLength(1)];

                for (int y = 0; y < val[x].Length; y++)
                {
                    val[x][y] = arr[x,y];
                }
            }

            return val;
        }

        public static string FormatNumberUnit(double number, string unit){
            if(number * 1E-30 >= 1) return (number * 1E-30).ToString("0.###") + "Q" + unit;
            if(number * 1E-27 >= 1) return (number * 1E-27).ToString("0.###") + "R" + unit;
            if(number * 1E-24 >= 1) return (number * 1E-24).ToString("0.###") + "Y" + unit;
            if(number * 1E-21 >= 1) return (number * 1E-21).ToString("0.###") + "Z" + unit;
            if(number * 1E-18 >= 1) return (number * 1E-18).ToString("0.###") + "E" + unit;
            if(number * 1E-15 >= 1) return (number * 1E-15).ToString("0.###") + "P" + unit;
            if(number * 1E-12 >= 1) return (number * 1E-12).ToString("0.###") + "T" + unit;
            if(number * 1E-9 >= 1) return (number * 1E-9).ToString("0.###") + "G" + unit;
            if(number * 1E-6 >= 1) return (number * 1E-6).ToString("0.###") + "M" + unit;
            if(number * 1E-3 >= 1) return (number * 1E-3).ToString("0.###") + "k" + unit;
            if(number * 1E0 >= 1 || number == 0) return (number * 1E0).ToString("0.###") + unit;
            if(number * 1E3 >= 1) return (number * 1E3).ToString("0.###") + "m" + unit;
            if(number * 1E6 >= 1) return (number * 1E6).ToString("0.###") + "μ" + unit;
            if(number * 1E9 >= 1) return (number * 1E9).ToString("0.###") + "n" + unit;
            if(number * 1E12 >= 1) return (number * 1E12).ToString("0.###") + "p" + unit;
            if(number * 1E15 >= 1) return (number * 1E15).ToString("0.###") + "f" + unit;
            if(number * 1E18 >= 1) return (number * 1E18).ToString("0.###") + "a" + unit;
            if(number * 1E21 >= 1) return (number * 1E21).ToString("0.###") + "z" + unit;
            if(number * 1E27 >= 1) return (number * 1E24).ToString("0.###") + "y" + unit;
            if(number * 1E30 >= 1) return (number * 1E27).ToString("0.###") + "r" + unit;
            return (number * 1E30).ToString("0.###") + "q" + unit;
        }
    }
}