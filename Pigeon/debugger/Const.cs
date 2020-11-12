using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using pigeon.data;
using pigeon.utilities.extensions;

namespace pigeon.debugger {
    public static class Const2 {
        private const string regexPattern = @"(?<name>\w+)=(?<val>[-+]?[0-9]*\.?[0-9]+)";
        private static readonly Regex parser = new Regex(regexPattern);

        private static string lastDefaultFilename;
        private static bool isModified = false;
        private static Dictionary<string, Dictionary<string, float>> defaultValues = new Dictionary<string, Dictionary<string, float>>();
        private static readonly Dictionary<string, Dictionary<string, float>> currentValues = new Dictionary<string, Dictionary<string, float>>();

        internal static List<string> Categories {
            get {
                List<string> categories = new List<string>(currentValues.Keys);
                categories.Sort();
                return categories;
            }
        }

        internal static List<string> GetVarsForCategory(string category) {
            List<string> categories = new List<string>(currentValues[category].Keys);
            categories.Sort();
            return categories;
        }

        public static int GetInt(string category, string varName) {
            return (int) currentValues[category][varName];
        }

        public static float GetFloat(string category, string varName) {
            return currentValues[category][varName];
        }

        public static void SetVar(string category, string varName, float val) {
            isModified = true;
            currentValues[category][varName] = val;
        }

        public static void LoadDefaults(string path) {
            if (lastDefaultFilename != path || !isModified) {
                var lines = GameData.Read(path);
                defaultValues = parseFile(lines);
                copyToCurrent(defaultValues);
                lastDefaultFilename = path;

                isModified = false;
            }
        }

        internal static void LoadPreset(string presetName) {
            var lines = PlayerData.Read(presetName);
            var preset = parseFile(lines);
            copyToCurrent(preset);
        }

        internal static void SavePreset(string presetName) {
            List<string> lines = new List<string>();

            foreach (var category in currentValues) {
                lines.Add(category.Key);
                lines.AddRange(category.Value.Select(variable => string.Format("{0}={1}", variable.Key, variable.Value)));
                lines.Add(string.Empty);
            }

            PlayerData.WriteToFile(presetName, lines.ToArray());
        }

        internal static void ResetToDefaults() {
            isModified = false;
            LoadDefaults(lastDefaultFilename);
        }

        private static void copyToCurrent(Dictionary<string, Dictionary<string, float>> desiredValues) {
            currentValues.Clear();

            foreach (var category in desiredValues.Keys) {
                currentValues.Add(category, new Dictionary<string, float>());
                foreach (var varName in desiredValues[category].Keys) {
                    currentValues[category].Add(varName, desiredValues[category][varName]);
                }
            }
        }

        private static Dictionary<string, Dictionary<string, float>> parseFile(string[] fileContents) {
            var loadedVals = new Dictionary<string, Dictionary<string, float>>();
            int i = 0;

            while (fileContents != null && i < fileContents.Length) {
                string cat = fileContents[i++];

                if (string.IsNullOrWhiteSpace(cat) || cat.StartsWith("#")) {
                    continue;
                }

                if (!loadedVals.ContainsKey(cat)) {
                    loadedVals.Add(cat, new Dictionary<string, float>());
                }

                bool isSameCat = true;
                while (isSameCat) {
                    string nextVar = fileContents[i];
                    var parseResult = parser.Match(nextVar);
                    if (parseResult.Success) {
                        string varName = parseResult.Groups["name"].Value;
                        float varVal = parseResult.Groups["val"].Value.ToFloat();

                        loadedVals[cat].Add(varName, varVal);
                    }

                    i++;

                    if (i >= fileContents.Length || string.IsNullOrWhiteSpace(fileContents[i])) {
                        isSameCat = false;
                    }
                }
            }

            return loadedVals;
        }
    }
}
