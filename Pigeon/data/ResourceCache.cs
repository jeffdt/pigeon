using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using pigeon.gfx;

namespace pigeon.data {
    public static class ResourceCache {
        private static readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        private static readonly Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();
        private static readonly Dictionary<string, SpriteFont> fonts = new Dictionary<string, SpriteFont>();

        private static TextureTemplateProcessor templateProcessor;

        public static Texture2D Pixel;

        public static Texture2D Texture(string name) {
            return textures[name];
        }

        public static SoundEffect Sound(string name) {
            return sounds[name];
        }

        public static SpriteFont Font(string name) {
            return fonts[name];
        }

        public static void Initialize(TextureTemplateProcessor textureTemplateProcessor) {
            templateProcessor = textureTemplateProcessor;

            var whitePixel = new Texture2D(Renderer.GraphicsDeviceMgr.GraphicsDevice, 1, 1);
            whitePixel.SetData(new[] { Color.White });
            textures.Add("pixel", whitePixel);
            Pixel = whitePixel;
        }

        public static void LoadTextures(string[] texturePaths, string[] templateTextures) {
            foreach (var texturePath in texturePaths) {
                if (templateTextures != null && Array.IndexOf(templateTextures, texturePath) > -1) {
                    ProcessedTextureTemplate[] processedTextureTemplates = templateProcessor.Invoke(texturePath);
                    foreach (var newTexture in processedTextureTemplates) {
                        textures.Add(newTexture.Alias, newTexture.Texture);
                    }
                } else {
                    textures.Add(texturePath, ContentLoader.LoadTexture(texturePath));
                }
            }
        }

        public static void LoadSounds(string[] sfxPaths) {
            foreach (var sfxPath in sfxPaths) {
                sounds.Add(sfxPath, ContentLoader.LoadSoundEffect(sfxPath));
            }
        }

        public static void LoadFonts(Font[] fontDefs) {
            foreach (var fontDef in fontDefs) {
                SpriteFont font = ContentLoader.LoadFont(fontDef.Path);
                font.Spacing = fontDef.Spacing ?? font.Spacing;
                font.LineSpacing = fontDef.LineSpacing ?? font.LineSpacing;
                fonts.Add(fontDef.Name, font);
            }
        }
    }
}
