using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using pigeon.utilities.extensions;

namespace pigeon.data {
    public class Properties {
        public readonly string Filename;

        private readonly Dictionary<string, string> dictionary;

        public Properties(string file) {
            Filename = file;
            dictionary = new Dictionary<string, string>();

            if (File.Exists(Filename))
                loadFromFile(Filename);
            else
                _ = File.Create(Filename);
        }

        private string getRaw(string field) {
            return (dictionary.TryGetValue(field, out string value)) ? (value) : (null);
        }

        private string getReq(string field) {
            if (!dictionary.ContainsKey(field)) {
                throw new KeyNotFoundException(string.Format("required field '{0}' missing in property file {1}", field, Filename));
            }
            return dictionary[field];
        }

        public bool GetBool(string field) {
            return getReq(field).ToBool();
        }

        public bool TryBool(string field, bool defaultValue) {
            string value = getRaw(field);
            return (value == null) ? defaultValue : value.ToBool();
        }

        public int GetInt(string field) {
            return getReq(field).ToInt();
        }

        public int TryInt(string field, int defaultValue) {
            string value = getRaw(field);
            return (value == null) ? defaultValue : value.ToInt();
        }

        public float GetFloat(string field) {
            return getReq(field).ToFloat();
        }

        public float TryFloat(string field, float defaultValue) {
            string value = getRaw(field);
            return (value == null) ? defaultValue : value.ToFloat();
        }

        public string GetString(string field) {
            return getReq(field);
        }

        public string TryString(string field, string defaultValue) {
            string value = getRaw(field);
            return value ?? defaultValue;
        }

        public T GetEnum<T>(string field) {
            return getReq(field).ToEnum<T>();
        }

        public Vector2 GetVector2(string field) {
            return getReq(field).ToVector2();
        }

        public Vector2 TryVector2(string field, Vector2 defaultValue) {
            string value = getRaw(field);
            return (value == null) ? defaultValue : value.ToVector2();
        }

        public List<string> GetStringList(string field) {
            return parseList("string", field).ToList();
        }

        public List<string> TryStringList(string field) {
            string value = getRaw(field);
            return value == null ? new List<string>() : GetStringList(field);
        }

        public List<Vector2> GetVector2List(string field) {
            return parseList("vector", field).Select(str => str.ToVector2()).ToList();
        }

        private string[] parseList(string typeDescription, string field) {
            string[] strings = getReq(field).Split('|', ';');

            if (strings.Length == 0) {
                throw new FormatException(string.Format("{0} list could not be parsed for field '{1}'", typeDescription, field));
            }

            return strings;
        }

        private void loadFromFile(string file) {
            foreach (string line in File.ReadAllLines(file)) {
                if ((!string.IsNullOrEmpty(line))
                    && (!line.StartsWith(";"))
                    && (!line.StartsWith("#"))
                    && (!line.StartsWith("'"))
                    && (line.Contains('='))) {
                    int index = line.IndexOf('=');
                    string key = line.Substring(0, index).Trim();
                    string value = line.Substring(index + 1).Trim();

                    if ((value.StartsWith("\"") && value.EndsWith("\""))
                        || (value.StartsWith("'") && value.EndsWith("'"))) {
                        value = value.Substring(1, value.Length - 2);
                    }

                    try {
                        // catch duplicates
                        dictionary.Add(key, value);
                    } catch (ArgumentException e) {
                        throw new ArgumentException(string.Format("property {0} in property file {1} was defined more than once", e.ParamName, Filename), e);
                    }
                }
            }
        }
    }
}
