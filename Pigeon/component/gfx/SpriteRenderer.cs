using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pigeon.gameobject;

namespace pigeon.gfx.drawable.sprite {
    public class SpriteRenderer : Component, IRenderable, IFlippable {
        public Sprite Sprite;

        public SpriteRenderer() { }

        public SpriteRenderer(string spriteName, string initialAnim = null, string texturePath = null) {
            Sprite = Sprite.Clone(spriteName, texturePath);

            if (initialAnim != null) {
                Play(initialAnim);
            }
        }

        public SpriteRenderer Play(string anim) {
            Sprite.Play(anim);
            return this;
        }

        public SpriteRenderer Play(string anim, Action callback) {
            Sprite.Play(anim, callback);
            return this;
        }

        public SpriteRenderer Play(string anim, Action callback, int callbackFrame) {
            Sprite.Play(anim, callback, callbackFrame);
            return this;
        }

        public SpriteRenderer Loop(string anim, Action onFinish = null) {
            Sprite.Loop(anim, onFinish);
            return this;
        }

        public SpriteRenderer PingPong(string anim) {
            Sprite.PingPong(anim);
            return this;
        }

        public SpriteRenderer ShowFrame(int frameIndex, string animation = null) {
            Sprite.ShowFrame(frameIndex, animation);
            return this;
        }

        public void Stop() {
            Sprite.Stop();
        }

        public bool IsFlippedHorizontal() {
            return (Sprite.Flip & SpriteEffects.FlipHorizontally) != 0;
        }

        public bool IsFlippedVertical() {
            return (Sprite.Flip & SpriteEffects.FlipVertically) != 0;
        }

        public void SetFlipHorizontal(bool hFlip) {
            if (hFlip) {
                Sprite.Flip |= SpriteEffects.FlipHorizontally;
            } else {
                Sprite.Flip &= SpriteEffects.FlipVertically;
            }
        }

        public void SetFlipVertical(bool vFlip) {
            if (vFlip) {
                Sprite.Flip |= SpriteEffects.FlipVertically;
            } else {
                Sprite.Flip &= SpriteEffects.FlipHorizontally;
            }
        }

        public void Rotate90DegreesCW(int numOf90DegreeRotations) {
            // TODO: no idea if/how this works
            Sprite.Rotation = MathHelper.PiOver2 * numOf90DegreeRotations;
        }

        protected override void Initialize() {
            SetFlipHorizontal(Object.IsFlippedX());
            SetFlipVertical(Object.IsFlippedY());
        }

        protected override void Update() {
            Sprite.Update();
        }

        public void Draw() {
            if (Enabled) {
                Sprite.Draw(Object.WorldPosition.ToVector2(), Object.DrawLayer);
            }
        }

        public void SetColor(Color color) {
            Sprite.Color = color;
        }

        public void OnFlipped() {
            SetFlipHorizontal(Object.IsFlippedX());
            SetFlipVertical(Object.IsFlippedY());
        }
    }
}

