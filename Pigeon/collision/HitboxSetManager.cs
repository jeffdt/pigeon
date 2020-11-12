using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using pigeon.data;
using pigeon.utilities.extensions;

namespace pigeon.collision {
    internal class HitboxSetManager {
        #region database
        private static readonly Regex smartParse = new Regex(@"\[(?<offsets>\S.*)]\[(?<dimensions>\S.*)](\{(?<options>\S.*)\})?");

        private static readonly Regex infoParse = new Regex(@"(?<x>-?\d*(.\d+)?),(?<y>-?\d*(.\d+)?)");

        private static readonly Dictionary<string, HitboxSetManager> allHitboxCollections = new Dictionary<string, HitboxSetManager>();

        public static void Load(string path) {
            if (!GameData.FileExists(path)) {
                return;
            }

            var lines = GameData.Read(path);

            int i = 0;

            while (lines != null && i < lines.Length) {
                string hitboxName = lines[i++];

                if (string.IsNullOrWhiteSpace(hitboxName) || hitboxName.StartsWith("#")) {
                    continue;
                }

                HitboxSetManager hitboxSetManager = new HitboxSetManager {
                    allHitboxSets = new List<HitboxSet>()
                };

                bool isSameHitbox = true;
                while (isSameHitbox) {
                    string vertexSetDef = lines[i];

                    var parseResult = smartParse.Match(vertexSetDef);

                    if (parseResult.Success) {
                        // string vertexSetName = parseResult.Groups["name"].Value;
                        Point offset = parsePoint(parseResult.Groups["offsets"].Value);
                        Point dimensions = parsePoint(parseResult.Groups["dimensions"].Value);
                        string options = parseResult.Groups["options"].Value;
                        bool hFlip = parseOption(options, 'H');
                        bool vFlip = parseOption(options, 'V');
                        bool passive = parseOption(options, 'P');

                        HitboxSet hitboxSet = new HitboxSet(offset, dimensions, hFlip, vFlip, passive);
                        hitboxSetManager.allHitboxSets.Add(hitboxSet);
                    }

                    i++;

                    if (i >= lines.Length || !lines[i].StartsWith("\t")) {
                        isSameHitbox = false;
                        allHitboxCollections.Add(hitboxName, hitboxSetManager);
                    }
                }
            }
        }

        private static Point parsePoint(string info) {
            var parseResult = infoParse.Match(info);
            if (parseResult.Success) {
                return new Point {
                    X = parseResult.Groups["x"].Value.ToInt(),
                    Y = parseResult.Groups["y"].Value.ToInt()
                };
            }

            throw new ArgumentException("bad point string formatting: " + info);
        }

        private static bool parseOption(string optionString, char character) {
            return optionString.IndexOf(character) >= 0;
        }

        #endregion

        public Rectangle GetHitbox() {
            return currentHitbox;
        }

        public bool IsPassive() {
            return currentHitboxSet.IsPassive;
        }

        private List<HitboxSet> allHitboxSets;
        private HitboxSet currentHitboxSet;
        private Rectangle currentHitbox;

        public static HitboxSetManager Clone(string name) {
            var sourceSet = allHitboxCollections[name];

            return new HitboxSetManager {
                allHitboxSets = sourceSet.allHitboxSets
            };
        }

        public void UseHitbox(int hbNumber, bool isFlippedX, bool isFlippedY) {
            currentHitboxSet = allHitboxSets[hbNumber];
            SetFlipping(isFlippedX, isFlippedY);
        }

        public void SetFlipping(bool isFlippedX, bool isFlippedY) {
            currentHitbox = currentHitboxSet.GetHitbox(isFlippedX, isFlippedY);
        }
    }
}
