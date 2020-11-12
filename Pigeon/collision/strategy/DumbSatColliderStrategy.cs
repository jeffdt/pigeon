using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using pigeon.utilities;

namespace pigeon.collision {
    public class DumbSatColliderStrategy : IColliderStrategy {
        private const int ITERATIONS = 3;

        public bool Enabled { get; set; } = true;

        public void Collide(List<ColliderComponent> allBoxes) {
            if (!Enabled) {
                return;
            }

            for (int i = 0; i < allBoxes.Count; i++) {
                var boxA = allBoxes[i];
                if (!boxA.Enabled || boxA.IsPassive()) {
                    continue;
                }

                var rectA = boxA.GetRectangle();

                for (int j = 0; j < allBoxes.Count; j++) {
                    var boxB = allBoxes[j];
                    if (i == j || !boxB.Enabled || boxA.FrameCollisions.Contains(boxB) || (boxA.IgnoredColliders?.Contains(boxB) == true)) {
                        continue;
                    }

                    bool collided;

                    if (boxA.GetShape() == HitboxShapes.Box && boxB.GetShape() == HitboxShapes.Box) {
                        var rectB = boxB.GetRectangle();

                        int iterationCount = 1;
                        if (boxA.Object.Velocity.LengthSquared() + boxB.Object.Velocity.LengthSquared() > 3000) {
                            iterationCount = 2;
                        }

                        var boxAWorldPosition = boxA.Object.WorldPosition;
                        var boxBWorldPosition = boxB.Object.WorldPosition;

                        for (int iteration = 0; iteration < iterationCount; iteration++) {
                            var deltaTime = (iteration + 1) * time.Time.SecScaled / iterationCount;
                            var boxASpeculativePosition = boxA.Object.SpeculativeWorldPositionAt(deltaTime);
                            var boxBSpeculativePosition = boxB.Object.SpeculativeWorldPositionAt(deltaTime);

                            var specRectXA = getSpeculativeRect(rectA, boxASpeculativePosition.X, boxAWorldPosition.Y);
                            var specRectXB = getSpeculativeRect(rectB, boxBSpeculativePosition.X, boxBWorldPosition.Y);
                            var penX = checkBox(specRectXA, specRectXB, true);
                            if (penX != 0) {
                                penX = (penX < 0 && (boxA.IgnoredSides[1] || boxB.IgnoredSides[3]))
                                    || (penX > 0 && (boxA.IgnoredSides[3] || boxB.IgnoredSides[1]))
                                    ? 0 : penX;
                            }

                            var specRectYA = getSpeculativeRect(rectA, boxAWorldPosition.X, boxASpeculativePosition.Y);
                            var specRectYB = getSpeculativeRect(rectB, boxBWorldPosition.X, boxBSpeculativePosition.Y);
                            var penY = checkBox(specRectYA, specRectYB, false);
                            if (penY != 0) {
                                penY = (penY < 0 && (boxA.IgnoredSides[2] || boxB.IgnoredSides[0]))
                                    || (penY > 0 && (boxA.IgnoredSides[0] || boxB.IgnoredSides[2]))
                                    ? 0 : penY;
                            }

                            if (penX != 0 || penY != 0) {
                                var penetration = new Point(penX, penY);
                                boxA.Collided(boxB, penetration);
                                boxB.Collided(boxA, penetration.Mult(-1));
                                break;
                            }
                        }
                    } else if (boxA.GetShape() == HitboxShapes.Box && boxB.GetShape() == HitboxShapes.Point) {
                        collided = SatCollider.CollideBoxPoint(boxA.GetRectangle(), boxB.Object.WorldPosition);
                        if (collided) {
                            boxA.Collided(boxB, Point.Zero);
                            boxB.Collided(boxA, Point.Zero);
                        }
                    } else if (boxA.GetShape() == HitboxShapes.Point && boxB.GetShape() == HitboxShapes.Box) {
                        collided = SatCollider.CollideBoxPoint(boxB.GetWorldRectangle(), boxA.Object.WorldPosition);

                        if (collided) {
                            boxA.Collided(boxB, Point.Zero);
                            boxB.Collided(boxA, Point.Zero);
                        }
                    } else {
                        throw new NotImplementedException(string.Format("no collision detection algorithm for shapes {0} and {1}", boxA.GetShape(), boxB.GetShape()));
                    }
                }
            }
        }

        private static Rectangle getSpeculativeRect(Rectangle hbRect, int xPosition, int yPosition) {
            return new Rectangle(hbRect.X + xPosition, hbRect.Y + yPosition, hbRect.Width, hbRect.Height);
        }

        private static int checkBox(Rectangle specRectA, Rectangle specRectB, bool isX) {
            bool collided = SatCollider.CollideBoxes(specRectA, specRectB);
            return collided
                ? (isX ? SatCollider.GetMinTranslationX(specRectA, specRectB) : SatCollider.GetMinTranslationY(specRectA, specRectB))
                : 0;
        }

        public void Draw() { }
    }
}