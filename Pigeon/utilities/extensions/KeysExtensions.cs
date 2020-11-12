using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace pigeon.utilities.extensions {
    public static class KeysExtensions {
        private static readonly Dictionary<Keys, char> printCharacters = new Dictionary<Keys, char>{
            {Keys.Space, ' '},
            {Keys.A, 'a'},
            {Keys.B, 'b'},
            {Keys.C, 'c'},
            {Keys.D, 'd'},
            {Keys.E, 'e'},
            {Keys.F, 'f'},
            {Keys.G, 'g'},
            {Keys.H, 'h'},
            {Keys.I, 'i'},
            {Keys.J, 'j'},
            {Keys.K, 'k'},
            {Keys.L, 'l'},
            {Keys.M, 'm'},
            {Keys.N, 'n'},
            {Keys.O, 'o'},
            {Keys.P, 'p'},
            {Keys.Q, 'q'},
            {Keys.R, 'r'},
            {Keys.S, 's'},
            {Keys.T, 't'},
            {Keys.U, 'u'},
            {Keys.V, 'v'},
            {Keys.W, 'w'},
            {Keys.X, 'x'},
            {Keys.Y, 'y'},
            {Keys.Z, 'z'},
            {Keys.D0, '0'},
            {Keys.D1, '1'},
            {Keys.D2, '2'},
            {Keys.D3, '3'},
            {Keys.D4, '4'},
            {Keys.D5, '5'},
            {Keys.D6, '6'},
            {Keys.D7, '7'},
            {Keys.D8, '8'},
            {Keys.D9, '9'},
            {Keys.OemMinus, '-'},
            {Keys.OemPlus,'='},
            {Keys.OemOpenBrackets, '['},
            {Keys.OemCloseBrackets, ']'},
            {Keys.OemSemicolon, ';'},
            {Keys.OemQuotes, '\''},
            {Keys.OemPipe, '\\'},
            {Keys.OemComma, ','},
            {Keys.OemPeriod, '.'},
            {Keys.OemQuestion, '/'},
        };

        public static bool IsPrintable(this Keys key) {
            return printCharacters.ContainsKey(key);
        }

        public static char ToChar(this Keys key) {
            return printCharacters[key];
        }
    }
}
