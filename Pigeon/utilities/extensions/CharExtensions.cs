using System.Collections.Generic;

namespace pigeon.utilities.extensions {
    public static class CharExtensions {
        private static readonly Dictionary<char, char> shiftChars = new Dictionary<char, char>{
            {'1', '!'},
            {'2', '@'},
            {'3', '#'},
            {'4', '$'},
            {'5', '%'},
            {'6', '^'},
            {'7', '&'},
            {'8', '*'},
            {'9', '('},
            {'0', ')'},
            {'-', '_'},
            {'=', '+'},
            {';', ':'},
            {'\'', '\"'},
            {',', '<'},
            {'.', '>'},
            {'/', '?'}
        };

        public static char ToLower(this char character) {
            return char.ToLower(character);
        }

        public static bool IsLower(this char character) {
            return char.IsLower(character);
        }

        public static int ParseIntDigit(this char character) {
            return (int) char.GetNumericValue(character);
        }

        public static int Ascii(this char character) {
            return character;
        }

        public static bool TryGetShiftChar(this char character, out char shiftCharacter) {
            return shiftChars.TryGetValue(character, out shiftCharacter);
        }
    }
}
