using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pigeon.data;
using pigeon.gfx;
using pigeon.time;

namespace pigeon.particle {
    public class Particle : IRegistryObject {
        #region static
        private const int RESERVE_COUNT = 1000;
        private static Queue<Particle> particles;

        public static void Initialize() {
            if (particles == null) {
                particles = new Queue<Particle>();
            }

            refillPool();
        }

        private static void refillPool() {
            var count = particles.Count;
            if (count < RESERVE_COUNT / 2) {
                for (int i = count; i < RESERVE_COUNT; i++) {
                    particles.Enqueue(new Particle());
                }
            }
        }

        public static Particle Get() {
            refillPool();
            return particles.Dequeue();
        }

        public static void Recycle(Particle particle) {
            particle.age = 0;
            particles.Enqueue(particle);
        }
        #endregion

        // parameters
        public Point Position {
            get { return new Point((int) truePosition.X, (int) truePosition.Y); }
            set { truePosition = value.ToVector2(); }
        }

        public Vector2 Velocity { get { return _velocity; } set { _velocity = value; } }
        public Color Color { get; set; }
        public float Lifespan { get; set; }
        public float DrawLayer { get; set; }

        // state
        private Vector2 _velocity;
        private Vector2 truePosition;
        private float age { get; set; }

        public bool Update() {
            age += Time.SecScaled;
            if (age >= Lifespan) {
                return true;
            }

            truePosition += Velocity * Time.SecScaled;
            return false;
        }

        public void OnUnregister() {
            Recycle(this);
        }

        public void Draw() {
            Renderer.SpriteBatch.Draw(ResourceCache.Pixel, Position.ToVector2(), null, Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, DrawLayer);
            // Renderer.spriteBatch.Draw(ResourceCache.Pixel, Position, Color);
        }

        public void SetPositionX(int x) {
            truePosition.X = x;
        }

        public void SetPositionY(int y) {
            truePosition.Y = y;
        }

        public void SetPositionXY(int x, int y) {
            truePosition.X = x;
            truePosition.Y = y;
        }

        public void SetVelocityX(int x) {
            _velocity.X = x;
        }

        public void SetVelocityY(int y) {
            _velocity.Y = y;
        }

        public void SetVelocityXY(int x, int y) {
            _velocity.X = x;
            _velocity.Y = y;
        }
    }
}