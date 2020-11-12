using System.Collections.Generic;
using Microsoft.Xna.Framework;
using pigeon.gameobject;

namespace pigeon.collision {
    public abstract class ColliderComponent : Component, IFlippable {
        // parameters
        public CollisionHandler CollisionHandler;

        public abstract HitboxShapes GetShape();
        public abstract bool IsPassive();
        public abstract Rectangle GetRectangle();

        public Rectangle GetWorldRectangle() {
            var localRect = GetRectangle();
            var worldPosition = Object.WorldPosition;
            return new Rectangle(worldPosition.X + localRect.X, worldPosition.Y + localRect.Y, localRect.Width, localRect.Height);
        }

        public int Width { get { return GetRectangle().Width; } }
        public int Height { get { return GetRectangle().Height; } }

        public HashSet<string> Tags = new HashSet<string>();
        public HashSet<ColliderComponent> FrameCollisions = new HashSet<ColliderComponent>();
        public HashSet<ColliderComponent> LastFrameCollisions = new HashSet<ColliderComponent>();

        public HashSet<ColliderComponent> IgnoredColliders;
        public bool[] IgnoredSides = new bool[4];

        protected ColliderComponent() {
            Destructor = destroy;
        }

        public ColliderComponent AddTag(string tag) {
            Tags.Add(tag);
            return this;
        }

        public ColliderComponent SetTags(HashSet<string> tags) {
            Tags = tags;
            return this;
        }

        public bool HasTag(string tag) {
            return Tags.Contains(tag);
        }

        public bool HasAnyTag(params string[] tags) {
            foreach (var tag in tags) {
                if (Tags.Contains(tag)) {
                    return true;
                }
            }

            return false;
        }

        public void Collided(ColliderComponent otherHitbox, Point penetration) {
            CollisionHandler?.Invoke(this, otherHitbox, penetration);
            FrameCollisions.Add(otherHitbox);
        }

        protected override void Update() {
            if (LastFrameCollisions.Count > 0) {
                LastFrameCollisions.Clear();
            }

            if (FrameCollisions.Count > 0) {
                foreach (var collision in FrameCollisions) {
                    LastFrameCollisions.Add(collision);
                }

                FrameCollisions.Clear();
            }
        }

        protected override void Initialize() {
            Pigeon.World.Hitboxes.Add(this);
        }

        private void destroy() {
            Pigeon.World.Hitboxes.Remove(this);
        }

        public abstract void OnFlipped();
        public abstract void Draw();
    }
}
