using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pigeon.data;
using pigeon.gfx;
using pigeon.time;
using pigeon.utilities.extensions;

namespace pigeon.gfx.drawable.sprite {
    public class Sprite : Graphic {
        private const string NO_TEXTURE = "NO TEXTURE SPECIFIED";

        #region database
        private static readonly Regex smartParse = new Regex(@"(?<animName>\S*)\[(?<seed>\S.*)](\((?<dupeParams>\S.*)\))?(\{(?<delayParams>\S.*)\})?");
        private static readonly Regex seedParse = new Regex(@"(?<anchorX>\d.*),(?<anchorY>\d.*),(?<sheetX>\d.*),(?<sheetY>\d.*),(?<width>\d.*),(?<height>\d.*)");

        public static readonly Dictionary<string, Sprite> allSprites = new Dictionary<string, Sprite>();

        public static void LoadCompact(string path) {
            var lines = GameData.Read(path);
            int i = 0;

            while (lines != null && i < lines.Length) {
                string spriteName = lines[i++];

                if (string.IsNullOrWhiteSpace(spriteName) || spriteName.StartsWith("#")) {
                    continue;
                }

                var splitName = spriteName.Split(new[] { '=' });
                spriteName = splitName[0];
                string textureName = null;
                if (splitName.Length == 2) {
                    textureName = splitName[1];
                }

                Sprite sprite = new Sprite { Animations = new SerializableDictionary<string, List<SpriteFrame>>(), texturePath = textureName };

                bool isSameSprite = true;
                while (isSameSprite) {
                    string animationDef = lines[i];

                    var parseResult = smartParse.Match(animationDef);

                    if (parseResult.Success) {
                        string animationName = parseResult.Groups["animName"].Value;

                        SpriteFrame seedFrame = parseSeed(parseResult.Groups["seed"].Value);
                        List<SpriteFrame> frames = new List<SpriteFrame> { seedFrame };

                        string dupeParamsStr = parseResult.Groups["dupeParams"].Value;
                        string delayParamsStr = parseResult.Groups["delayParams"].Value;
                        if (dupeParamsStr != string.Empty) {
                            string[] dupeParams = dupeParamsStr.Split(new[] { ',' });
                            int duration = dupeParams[0].ToInt();
                            int frameCount = dupeParams[1].ToInt();
                            seedFrame.Duration = duration;

                            for (int frameNum = 1; frameNum < frameCount; frameNum++) {
                                SpriteFrame frame = createFrame(seedFrame, frameNum, duration);
                                frames.Add(frame);
                            }
                        } else if (delayParamsStr != string.Empty) {
                            string[] delayParams = delayParamsStr.Split(new[] { ',' });
                            int frameCount = delayParams.Length;
                            seedFrame.Duration = delayParams[0].ToInt();

                            for (int frameNum = 1; frameNum < frameCount; frameNum++) {
                                SpriteFrame frame = createFrame(seedFrame, frameNum, delayParams[frameNum].ToInt());
                                frames.Add(frame);
                            }
                        }

                        sprite.Animations.Add(animationName, frames);
                    }

                    i++;

                    if (i >= lines.Length || !lines[i].StartsWith("\t")) {
                        isSameSprite = false;
                        allSprites.Add(spriteName, sprite);
                    }
                }
            }
        }

        private static SpriteFrame parseSeed(string seed) {
            var parseResult = seedParse.Match(seed);
            return parseResult.Success
                ? new SpriteFrame {
                    AnchorOffsetX = parseResult.Groups["anchorX"].Value.ToInt(),
                    AnchorOffsetY = parseResult.Groups["anchorY"].Value.ToInt(),
                    SheetX = parseResult.Groups["sheetX"].Value.ToInt(),
                    SheetY = parseResult.Groups["sheetY"].Value.ToInt(),
                    Width = parseResult.Groups["width"].Value.ToInt(),
                    Height = parseResult.Groups["height"].Value.ToInt()
                }
                : null;
        }

        private static SpriteFrame createFrame(SpriteFrame seedFrame, int frameNum, int duration) {
            return new SpriteFrame {
                Duration = duration,
                AnchorOffsetX = seedFrame.AnchorOffsetX,
                AnchorOffsetY = seedFrame.AnchorOffsetY,
                SheetX = seedFrame.SheetX + (seedFrame.Width * frameNum),
                SheetY = seedFrame.SheetY,
                Width = seedFrame.Width,
                Height = seedFrame.Height
            };
        }

        public static Sprite Clone(string name, string customTexturePath = null) {
            var sourceSprite = allSprites[name];
            var texturePath = customTexturePath ?? sourceSprite.texturePath;

            return new Sprite {
                Animations = sourceSprite.Animations,
                texturePath = texturePath,
                Texture = ResourceCache.Texture(texturePath)
            };
        }

        #endregion

        public bool Looping { get; set; }
        private bool isPingPonging;
        private int pingPongCoefficient;

        public SerializableDictionary<string, List<SpriteFrame>> Animations;
        public Texture2D Texture;
        private string texturePath = NO_TEXTURE;
        private List<SpriteFrame> animation;
        public Rectangle SourceArea = new Rectangle();
        private int frameIndex;
        private float frameTimer;
        private bool playing;
        public Action Callback;
        private int _callbackFrame;

        public string CurrentAnimation { get; private set; }

        private Vector2 adjOrigin;

        public bool ContainsAnimation(string name) {
            return Animations.ContainsKey(name);
        }

        public void Stop() {
            playing = false;
        }

        public void Play(string name, Action onFinish = null, int callbackFrame = -1) {
            CurrentAnimation = name;

            Callback = onFinish;
            _callbackFrame = callbackFrame;

            animation = Animations[name];
            playFrame(0);
            playing = animation[0].Duration != -1;

            Looping = false;
            isPingPonging = false;
        }

        public void Loop(string name, Action onFinish = null) {
            CurrentAnimation = name;
            Callback = onFinish;

            animation = Animations[name];
            playFrame(0);
            playing = animation[0].Duration != -1;

            Looping = true;
            isPingPonging = false;
        }

        public void PingPong(string name) {
            CurrentAnimation = name;
            Callback = null;

            animation = Animations[name];
            playFrame(0);
            playing = animation[0].Duration != -1;

            isPingPonging = true;
            Looping = true;
            pingPongCoefficient = 0;
        }

        public void ShowFrame(int frame, string name) { // uses current animation if "name" == null
            if (name != null) {
                CurrentAnimation = name;
                animation = Animations[name];
            }

            Callback = null;
            playFrame(frame);

            playing = false;
            isPingPonging = false;
            Looping = false;
        }

        private void playFrame(int index) {
            frameIndex = index;

            var frame = animation[index];
            frameTimer = frame.Duration;
            SourceArea.X = frame.SheetX;
            SourceArea.Y = frame.SheetY;
            SourceArea.Width = frame.Width;
            SourceArea.Height = frame.Height;
            Offset = new Point(frame.AnchorOffsetX, frame.AnchorOffsetY);
            Size = new Point(SourceArea.Width, SourceArea.Height);
        }

        public override void Update() {
            if (!playing) { return; }

            frameTimer -= Time.MsScaled;
            if (frameTimer > 0) { return; } // still displaying current frame

            if (isPingPonging) {
                if (frameIndex == animation.Count - 1) {
                    pingPongCoefficient = -1;
                } else if (frameIndex == 0) {
                    pingPongCoefficient = 1;
                }
                playFrame(frameIndex + (pingPongCoefficient * 1));
                return;
            }

            if (frameIndex == animation.Count - 1) {
                if (Looping) {
                    if (isPingPonging) {
                        playFrame(animation.Count - 2);
                        pingPongCoefficient = -1;
                    } else {
                        playFrame(0);
                    }
                } else {
                    playing = false;
                }

                if (Callback != null && (_callbackFrame == -1 || _callbackFrame == animation.Count - 1)) {
                    Callback.Invoke();
                }
            } else {
                if (Callback != null && _callbackFrame == animation.Count - 1) {
                    Callback.Invoke();
                }

                playFrame(frameIndex + 1);
            }
        }

        public override void Draw(Vector2 position, float layer) {
            adjOrigin = Offset.ToVector2();

            if ((Flip & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally) {
                adjOrigin.X = Size.X - adjOrigin.X - 1;
            }

            if ((Flip & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically) {
                adjOrigin.Y = Size.Y - adjOrigin.Y - 1;
            }

            Renderer.SpriteBatch.Draw(Texture, position, SourceArea, Color, Rotation, adjOrigin, Scale, Flip, layer);
        }
    }
}