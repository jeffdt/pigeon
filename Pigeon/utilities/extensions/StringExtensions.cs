using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace pigeon.utilities.extensions {
    public static class StringExtensions {
        private static readonly char[] COMMA_SEPARATOR = { ',' };
        private static readonly char[] SPACE_SEPARATOR = { ' ' };

        public static string Last(this string source, int tailCharsLength) {
            return tailCharsLength >= source.Length ? source : source.Substring(source.Length - tailCharsLength);
        }

        // calculate how many characters will fit in a space of {spaceWidth} pixels
        public static string LastByPixels(this string source, int spaceWidth, SpriteFont font) {
            return _lastByPixels(source, spaceWidth, (str) => (int) font.MeasureString(str).X);
        }

        internal static string _lastByPixels(this string source, int spaceWidth, Func<string, int> strMeasurer) {
            for (int i = source.Length - 1; i >= 0; i--) {
                if (strMeasurer(source.Substring(i)) > spaceWidth) {
                    return source.Substring(i + 1);
                }
            }

            return source;
        }

        public static string After(this string source, string openingString) {
            return source.Substring(openingString.Length, source.Length - openingString.Length);
        }

        public static string Chop(this string source, string tailString) {
            return source.EndsWith(tailString) ? source.Substring(0, source.Length - tailString.Length) : source;
        }

        public static byte ToByte(this string str) {
            return Convert.ToByte(str);
        }

        public static byte HexToByte(this string hexStr) {
            return Convert.ToByte(hexStr, 16);
        }

        public static int ToInt(this string str) {
            return Convert.ToInt32(str);
        }

        public static float ToFloat(this string str) {
            return Convert.ToSingle(str);
        }

        public static double ToDouble(this string str) {
            return Convert.ToDouble(str);
        }

        public static bool ToBool(this string str) {
            if (str == "true" || str == "1" || str == "on" || str == "t") {
                return true;
            } else if (str == "false" || str == "0" || str == "off" || str == "f") {
                return false;
            } else {
                throw new FormatException();
            }
        }

        public static double? ToUnitInterval(this string str) {
            bool parsed = double.TryParse(str, out double result);
            return (parsed && result.InRange(0, 1)) ? result : null as double?;
        }

        public static Vector2 ToVector2(this string str) {
            string[] values = str.Split(COMMA_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);

            if (values.Length != 2) {
                throw new FormatException(string.Format("cannot parse '{0}' into a vector because it does not contain an X and Y component in the format 'x,y'.", str));
            }

            return new Vector2(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]));
        }

        public static Rectangle ToRectangle(this string str) {
            string[] values = str.Split(COMMA_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);

            if (values.Length != 4) {
                throw new FormatException(string.Format("cannot parse '{0}' into a rectangle because it does not contain X, Y, width and height components in the format 'x,y,width,height'.", str));
            }

            return new Rectangle(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), Convert.ToInt32(values[2]), Convert.ToInt32(values[3]));
        }

        public static T ToEnum<T>(this string str) {
            return (T) Enum.Parse(typeof(T), str, true);
        }

        public static string[] Tokenize(this string str) {
            return str.Split(SPACE_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string FormatWrap(this string text, SpriteFont font, int width, string wrapStr = "", int maxLines = 0) {
            StringBuilder lines = new StringBuilder();
            _wrapString(text, font.MeasureWidth, width, (str) => lines.Append(str).Append('\n'), wrapStr, maxLines);
            return lines.ToString();
        }

        public static List<string> SplitWrap(this string text, SpriteFont font, int width, string wrapStr = "", int maxLines = 0) {
            List<string> lines = new List<string>();
            _wrapString(text, font.MeasureWidth, width, (str) => lines.Add(str), wrapStr, maxLines);
            return lines;
        }

        internal static void _wrapString(this string text, Func<string, int> stringMeasurer, int width, Action<string> onSplit, string wrapStr = "", int maxLines = 0) {
            List<string> words = text.Split(SPACE_SEPARATOR, StringSplitOptions.RemoveEmptyEntries).ToList();
            string line = string.Empty;
            int lineCount = 1;

            for (int i = 0; i < words.Count; i++) {
                string word = words[i];
                int wordLength = stringMeasurer(word);
                if (wordLength > width) { // found one word that's too long to fit into a line on its own
                    for (int j = 0; j < word.Length; j++) {
                        string subword = word.Substring(0, j + 1);
                        int subwordLength = stringMeasurer(subword);
                        if (subwordLength > width) { // found exactly where word {i} too long (index {j})
                            string splitWordFirst = word.Substring(0, j - 1);
                            string splitWordSecond = word.Substring(j - 1, word.Length - j + 1);
                            words[i] = splitWordFirst + wrapStr;
                            words.Insert(i + 1, splitWordSecond);
                            break;
                        }
                    }
                }
            }

            foreach (string word in words) {
                if (stringMeasurer(line + word) > width) {
                    if (maxLines != 0 && lineCount >= maxLines) {
                        throw new ArgumentException(string.Format("cannot format string into {0} lines. original string: {1}", maxLines, text));
                    }

                    onSplit(line.Chop(" "));

                    line = string.Empty;
                    lineCount++;
                }

                line = line + word + ' ';
            }

            onSplit(line.Chop(" ")); // add last line
        }
    }
}
