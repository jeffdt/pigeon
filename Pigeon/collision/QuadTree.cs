using System.Collections.Generic;
using Microsoft.Xna.Framework;
using pigeon.utilities.extensions;

namespace pigeon.collision {
    internal class QuadTree {
        private const int MAX_OBJECTS = 50;
        private const int MAX_LEVELS = 4;

        private readonly int level;
        private readonly List<ColliderComponent> objects;
        private readonly Rectangle bounds;
        private readonly QuadTree[] nodes;

        public QuadTree(int pLevel, Rectangle pBounds) {
            level = pLevel;
            objects = new List<ColliderComponent>();
            bounds = pBounds;
            nodes = new QuadTree[4];
        }

        public void Draw() {
            if (nodes[0] == null) {
                Color fillColor = Color.Red;
                Color bordercolor = new Color(50, 50, 50, 200);

                if (level == 1) {
                    fillColor = Color.Blue;
                } else if (level == 2) {
                    fillColor = Color.Green;
                } else if (level == 3) {
                    fillColor = Color.Purple;
                }

                fillColor.A = 50;

                bounds.DrawFilledBordered(fillColor, bordercolor);
                // Renderer.Canvas.Draw(ResourceCache.Pixel, bounds, null, color, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            } else {
                foreach (var node in nodes) {
                    node.Draw();
                }
            }
        }

        public void Clear() {
            objects.Clear();

            for (int i = 0; i < nodes.Length; i++) {
                if (nodes[i] != null) {
                    nodes[i].Clear();
                    nodes[i] = null;
                }
            }
        }

        private void split() {
            int subWidth = bounds.Width / 2;
            int subHeight = bounds.Height / 2;
            int x = bounds.X;
            int y = bounds.Y;

            nodes[0] = new QuadTree(level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight));
            nodes[1] = new QuadTree(level + 1, new Rectangle(x, y, subWidth, subHeight));
            nodes[2] = new QuadTree(level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight));
            nodes[3] = new QuadTree(level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
        }

        private int findOccupyingQuad(ColliderComponent collider) {
            int index = -1;
            double verticalMidpoint = bounds.X + (bounds.Width / 2);
            double horizontalMidpoint = bounds.Y + (bounds.Height / 2);

            Rectangle worldHitbox = collider.GetWorldRectangle();

            // object can completely fit within the top quadrants
            bool topQuadrant = (worldHitbox.Bottom < horizontalMidpoint && worldHitbox.Top < horizontalMidpoint);   // TODO: can the second half of this be deleted?
                                                                                                                    // object can completely fit within the bottom quadrants
            bool bottomQuadrant = (worldHitbox.Top > horizontalMidpoint);

            if (worldHitbox.Left < verticalMidpoint && worldHitbox.Right < verticalMidpoint) {  // object can completely fit within the left quadrants
                if (topQuadrant) {
                    index = 1;
                } else if (bottomQuadrant) {
                    index = 2;
                }
            } else if (worldHitbox.Left > verticalMidpoint) {   // object can completely fit within the right quadrants
                if (topQuadrant) {
                    index = 0;
                } else if (bottomQuadrant) {
                    index = 3;
                }
            }

            return index;
        }

        public void Insert(ColliderComponent hitbox) {
            if (nodes[0] != null) {
                int index = findOccupyingQuad(hitbox);

                if (index != -1) {
                    nodes[index].Insert(hitbox);
                    return;
                }
            }

            objects.Add(hitbox);

            if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS) {
                if (nodes[0] == null) {
                    split();
                }

                int i = 0;
                while (i < objects.Count) {
                    int index = findOccupyingQuad(objects[i]);
                    if (index != -1) {
                        nodes[index].Insert(objects[i]);
                        objects.RemoveAt(i);
                    } else {
                        i++;
                    }
                }
            }
        }

        public void RetrieveCandidates(ColliderComponent thisBox, List<ColliderComponent> otherBoxes) {
            int index = findOccupyingQuad(thisBox);
            if (index != -1 && nodes[0] != null) {
                nodes[index].RetrieveCandidates(thisBox, otherBoxes);
            }

            otherBoxes.AddRange(objects);
        }
    }
}
