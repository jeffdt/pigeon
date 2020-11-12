using System.Collections.Generic;

namespace pigeon.collision {
    public interface IColliderStrategy {
        bool Enabled { get; set; }
        void Collide(List<ColliderComponent> allBoxes);
        void Draw();
    }
}