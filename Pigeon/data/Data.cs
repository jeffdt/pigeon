using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;
using pigeon.utilities.extensions;

namespace pigeon.data {
    public static class Data {
        private static readonly char[] commaSeparator = { ',' };

        public static string FormattedTimestamp() {
            var now = DateTime.Now;
            return string.Format(@"{0:000}.{1:00}.{2:00}_{3:00}.{4:00}.{5:00}.{6:000}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
        }

        #region file operations
        public static void WriteToFile(string fullPathFilename, string[] data) {
            File.WriteAllLines(fullPathFilename, data);
        }

        public static void AppendToFile(string filename, string message) {
            using (StreamWriter w = File.AppendText(filename)) {
                w.WriteLine(message);
            }
        }

        public static string[] ReadFile(string fullPathFilename) {
            return File.ReadAllLines(fullPathFilename);
        }

        public static List<List<string>> ReadCsvFile(string fullPathFilename) {
            var output = new List<List<string>>();

            var rows = File.ReadAllLines(fullPathFilename);

            for (int i = 0; i < rows.Length; i++) {
                var row = new List<string>();

                var cells = rows[i].Split(commaSeparator);
                for (int j = 0; j < cells.Length; j++) {
                    row.Add(cells[j]);
                }

                output.Add(row);
            }

            return output;
        }

        public static void SerializeObject<T>(T data, string fullPathFilename) {
            var writer = new StreamWriter(fullPathFilename);
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, data);
            writer.Close();
        }

        public static T DeserializeObject<T>(string directory, string filename) where T : class {
            return DeserializeObject<T>(Path.Combine(directory, filename));
        }

        public static T DeserializeObject<T>(string filePath) where T : class {
            if (!File.Exists(filePath)) {
                var dir = Directory.GetCurrentDirectory();
                Pigeon.Console.Error(string.Format("file {0} not found in directory {1}", filePath, dir));
                return null;
            }

            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StreamReader(filePath)) {
                return (T) serializer.Deserialize(reader);
            }
        }

        public static List<string> GetFileList(string searchLocation, string searchPattern, bool includeExtension = true) {
            DirectoryInfo dirInfo = new DirectoryInfo(searchLocation);
            var fileInfos = dirInfo.GetFiles(searchPattern);

            List<string> fileNames = new List<string>();

            if (includeExtension) {
                foreach (var fileInfo in fileInfos) {
                    fileNames.Add(fileInfo.Name);
                }
            } else {
                foreach (var fileInfo in fileInfos) {
                    fileNames.Add(Path.GetFileNameWithoutExtension(fileInfo.Name));
                }
            }

            return fileNames;
        }

        public static bool FileExists(string fullPathFilename) {
            return File.Exists(fullPathFilename);
        }
        #endregion

        #region directory operations
        public static bool DirectoryExists(string fullPathDir) {
            return Directory.Exists(fullPathDir);
        }

        public static void CreateDirectory(string fullPathDirectoryName) {
            if (!Directory.Exists(fullPathDirectoryName)) {
                Directory.CreateDirectory(fullPathDirectoryName);
            }
        }
        #endregion

        #region screenshots
        public static void SaveAsPng_Pigeon(this Texture2D texture, string fullPathFilename) {
            saveAsPng(texture.ToBitmap(), fullPathFilename);
        }

        // helper method to save bitmaps as pngs
        private static void saveAsPng(this Bitmap bitmap, string fullPathFilename) {
            using (Stream stream = File.Create(fullPathFilename)) {
                bitmap.Save(stream, ImageFormat.Png);
            }

            Pigeon.Console.Log(string.Format("Saving {0}...", fullPathFilename));
        }
        #endregion
    }
}
