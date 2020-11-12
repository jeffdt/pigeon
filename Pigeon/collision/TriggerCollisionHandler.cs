using Microsoft.Xna.Framework;

namespace pigeon.collision {
    public delegate void CollisionHandler(ColliderComponent thisHitbox, ColliderComponent otherHitbox, Point penetration);
}