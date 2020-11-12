using Microsoft.Xna.Framework;
using pigeon.core;
using pigeon.gameobject;
using pigeon.gfx.drawable.shape;
using System;

namespace pigeon.tools.anim {
    public class AnimationTool : World {
        protected override void Load() {
            BackgroundColor = Color.Black;
            Pigeon.Renderer.DrawScale = 3;
            Pigeon.Renderer.SetBaseResolution(500, 300);

            AddObj(new GameObject("Panel").AddComponent(new BoxRenderer() {
                DrawStyle = ShapeDrawStyles.Filled,
                FillColor = new Color(187, 187, 187),
                Rect = new Rectangle(0, 0, 1280, 50)
            }));
        }

        protected override void Unload() { }
    }
}
