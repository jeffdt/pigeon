using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace pigeon.data {
    public static class PlayerData {
        internal static string SaveFolderName { private get; set; }

        public static string UserDataPath { get; private set; }

        public static void Initialize() {
            UserDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), SaveFolderName);
            DirectoryInfo directory = new DirectoryInfo(UserDataPath);
            if (!directory.Exists) {
                directory.Create();
            }
        }

        private static string buildPath(string filename) {
            return Path.Combine(UserDataPath, filename);
        }

        #region file operations
        public static string[] Read(string filename) {
            return Data.ReadFile(buildPath(filename));
        }

        public static void AppendToFile(string filename, string message) {
            Data.AppendToFile(buildPath(filename), message);
        }

        public static void WriteToFile(string filename, string[] data) {
            Data.WriteToFile(buildPath(filename), data);
        }

        public static void Serialize<T>(T data, string filename) {
            Data.SerializeObject(data, buildPath(filename));
        }

        public static T Deserialize<T>(string filename) where T : class {
            return Data.DeserializeObject<T>(UserDataPath, filename);
        }

        public static bool FileExists(string filename) {
            return Data.FileExists(buildPath(filename));
        }

        public static List<string> GetFileList(string searchPattern, bool includeExtension = true) {
            return Data.GetFileList(UserDataPath, searchPattern, includeExtension);
        }

        public static void SaveAsTimestampedPng(this Texture2D texture) {
            var screenshotFolder = Path.Combine(UserDataPath, "screenshots");
            if (!DirectoryExists(screenshotFolder)) {
                var directoryInfo = Directory.CreateDirectory(screenshotFolder);
                Pigeon.Console.Log("Created directory: " + directoryInfo.FullName);
            }

            texture.SaveAsPng_Pigeon(Path.Combine(screenshotFolder, Data.FormattedTimestamp() + ".png"));
        }
        #endregion

        #region directory operations
        public static bool DirectoryExists(string dir) {
            return Data.DirectoryExists(buildPath(dir));
        }

        public static void CreateDirectory(string directoryName) {
            Data.CreateDirectory(buildPath(directoryName));
        }
        #endregion
    }
}
