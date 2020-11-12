using Microsoft.Xna.Framework;
using pigeon.debugger;
using pigeon.gameobject;
using pigeon.utilities;
using pigeon.gfx.drawable;

namespace pigeon.gfx {
    public class YSorter : Component, IRenderable {
        #region per-game configuration
        private static float layerMin;
        private static float layerMax;
        private static int yTop;
        private static int yBottom;

        public static void Configure(float layerMinValue, float layerMaxValue, int yLimitTop, int yLimitBottom) {
            layerMin = layerMinValue;
            layerMax = layerMaxValue;
            yTop = yLimitTop;
            yBottom = yLimitBottom;
        }
        #endregion

        public static bool DrawEnabled = false;

        public int YOffset;
        private int adjustedY;

        protected override void Initialize() {
            adjustLayer();
        }

        protected override void Update() {
            adjustLayer();
        }

        private void adjustLayer() {
            adjustedY = Object.WorldPosition.Y + YOffset;
            Object.Layer = adjustedY.CrossLerp(yTop, yBottom, layerMin, layerMax);
        }

        public void Draw() {
            if (DrawEnabled) {
                var worldPosition = Object.WorldPosition;
                DebugHelper.DrawDot(new Point(worldPosition.X, adjustedY), DebugHelper.Blue);
            }
        }
    }
}
