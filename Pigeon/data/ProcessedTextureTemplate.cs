using Microsoft.Xna.Framework.Graphics;

namespace pigeon.data {
    public delegate ProcessedTextureTemplate[] TextureTemplateProcessor(string texturePath);

    public class ProcessedTextureTemplate {
        public string Alias;
        public Texture2D Texture;
    }
}