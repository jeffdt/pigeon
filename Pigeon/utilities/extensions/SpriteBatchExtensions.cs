using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace pigeon.utilities.extensions {
    public static class SpriteBatchExtensions {
        public static void BeginPixelPerfect(this SpriteBatch spriteBatch, Matrix transformMatrix, SpriteSortMode sortMode = SpriteSortMode.Deferred) {
            spriteBatch.Begin(sortMode, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, transformMatrix);
        }
    }
}
