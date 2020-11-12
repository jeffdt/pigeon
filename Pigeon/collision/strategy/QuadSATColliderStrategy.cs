using System.Collections.Generic;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace pigeon.collision {
    public class QuadSATColliderStrategy : IColliderStrategy {
        internal QuadTree rootQuad;

        public QuadSATColliderStrategy(Rectangle bounds) {
            rootQuad = new QuadTree(0, bounds);
        }

        public bool Enabled { get; set; }

        public void Collide(List<ColliderComponent> allBoxes) {
            buildAllQuads(allBoxes);
            checkCollisions(allBoxes);
        }

        public void Draw() {
            rootQuad.Draw();
        }

        private void buildAllQuads(List<ColliderComponent> allBoxes) {
            rootQuad.Clear();
            foreach (ColliderComponent box in allBoxes) {
                rootQuad.Insert(box);
            }
        }

        private void checkCollisions(List<ColliderComponent> allBoxes) {
            var nearbyBoxes = new List<ColliderComponent>();
            foreach (ColliderComponent box in allBoxes) {
                if (box.IsPassive()) {
                    continue;
                }
                nearbyBoxes.Clear();
                rootQuad.RetrieveCandidates(box, nearbyBoxes);

                foreach (ColliderComponent otherBox in nearbyBoxes) {
                    if (box == otherBox || box.FrameCollisions.Contains(otherBox)) {
                        continue;
                    }

                    const bool collided = false;
                    //					var penetration = MinkowskiCollider.Collide(out collided, box, otherBox);
                    if (collided) {
                        //						box.CollidedWith(otherBox, penetration);
                        //						otherBox.CollidedWith(box, penetration);
                    }
                }
            }
        }
    }
}
