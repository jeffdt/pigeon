using System;
using System.Collections.Generic;
using System.IO;

namespace pigeon.data {
    public static class GameData {
        private static string contentDirectory;

        public static void Initialize() {
            string dir = "current dir: " + Directory.GetCurrentDirectory();
            Console.WriteLine(dir);
            Directory.SetCurrentDirectory("Content");
            contentDirectory = Directory.GetCurrentDirectory();
        }

        public static bool FileExists(string filename) {
            return Data.FileExists(filename);
        }

        public static string[] Read(string filename) {
            return Data.ReadFile(filename);
        }

        public static T Deserialize<T>(string filename) where T : class {
            return Data.DeserializeObject<T>(contentDirectory, filename);
        }

        // example: GameData.GetFileList(@"data\rooms\mp\*.lvl")
        public static List<string> GetFileList(string searchPattern, bool includeExtension = true) {
            return Data.GetFileList(contentDirectory, searchPattern, includeExtension);
        }

        public static List<List<string>> ReadCsvFile(string filename) {
            return Data.ReadCsvFile(Path.Combine(contentDirectory, filename));
        }

        public static string[] GetContentFiles(string directory) {
            string path = directory;

            if (!Directory.Exists(path)) {
                return new string[0];
            }

            string[] filePaths = Directory.GetFiles(path, @"*.xnb", SearchOption.AllDirectories);
            for (int index = 0; index < filePaths.Length; index++) {
                filePaths[index] = filePaths[index].Replace(path + @"\", "");
                filePaths[index] = filePaths[index].Replace(@".xnb", "");
            }

            return filePaths;
        }
    }
}
