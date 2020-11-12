using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using pigeon.gfx;
using pigeon.gfx.drawable.sprite;

namespace pigeon.gameobject {
    [Serializable]
    public class ObjectSeed {
        public string ParentName;

        public string Name;
        public float Layer;
        public Point Position;
        public string Sprite;
        public string Animation;
        public bool IsSpriteLooped;
        public bool IsSpritePingPonged;
        public bool IsAttached; // TODO: delete?
        public bool IsUpdateEnabled = true;
        public bool IsDrawEnabled = true;

        public bool IsYLayerSorted = false;
        public int YLayerSortOffset;

        public List<ObjectSeed> Children;

        public GameObject Build() {
            GameObject obj = new GameObject(Name) { Layer = Layer };
            if (Sprite != null) {
                var spriteRenderer = new SpriteRenderer(Sprite);
                obj.AddComponent(spriteRenderer);
                if (Animation != null) {
                    if (IsSpriteLooped) {
                        spriteRenderer.Loop(Animation);
                    } else if (IsSpritePingPonged) {
                        spriteRenderer.PingPong(Animation);
                    } else {
                        spriteRenderer.Play(Animation);
                    }
                }
            }

            obj.LocalPosition = Position;
            obj.UpdateEnabled = IsUpdateEnabled;
            obj.DrawEnabled = IsDrawEnabled;
            if (IsYLayerSorted) {
                obj.AddComponent(new YSorter { YOffset = YLayerSortOffset });
            }

            if (Children != null) {
                foreach (var child in Children) {
                    obj.AddChild(child.Build());
                }
            }

            return obj;
        }

        public ObjectSeed AddChild(string name, float layer, Point position, string sprite = null, string animation = null, bool isSpriteLooped = false) {
            var objectSeed = new ObjectSeed { Name = name, Layer = layer, Position = position, Sprite = sprite, Animation = animation, IsSpriteLooped = isSpriteLooped };
            return AddChild(objectSeed);
        }

        public ObjectSeed AddChild(ObjectSeed child) {
            (Children ?? (Children = new List<ObjectSeed>())).Add(child);
            return this;
        }
    }
}