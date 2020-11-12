using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pigeon.data;
using pigeon.gfx;

namespace pigeon.utilities {
    public static class GraphicsUtility {
        public static Texture2D ColorSwap(string textureName, Dictionary<Color, Color> colorMap) {
            Texture2D cachedTexture = ContentLoader.LoadTexture(textureName);

            Texture2D colorSwappedTexture = new Texture2D(Renderer.GraphicsDeviceMgr.GraphicsDevice, cachedTexture.Width, cachedTexture.Height);

            Color[] data = new Color[cachedTexture.Width * cachedTexture.Height];
            cachedTexture.GetData(data);

            for (int i = 0; i < data.Length; i++) {
                if (colorMap.ContainsKey(data[i])) {
                    data[i] = colorMap[data[i]];
                }
            }

            colorSwappedTexture.SetData(data);
            return colorSwappedTexture;
        }
    }
}