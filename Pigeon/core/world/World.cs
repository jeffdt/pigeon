using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using pigeon.collision;
using pigeon.data;
using pigeon.core.tasks;
using pigeon.gameobject;
using pigeon.particle;

namespace pigeon.core {
    public abstract class World {
        protected bool AddDebugger = true;

        public GameObject RootObj = new GameObject { Name = "Root", LayerSortingVarianceEnabled = false, LayerInheritanceEnabled = false };
        internal List<ColliderComponent> Hitboxes = new List<ColliderComponent>();

        public readonly ObjectRegistry<Particle> ParticleRegistry = new ObjectRegistry<Particle>();
        private readonly TaskRegistry taskRegistry = new TaskRegistry();
        public Color BackgroundColor = Pigeon.EngineBkgdColor;

        public IColliderStrategy ColliderStrategy;
        public static bool DrawColliderDebugInfo;

        public void AddObj(GameObject obj) {
            RootObj.AddChild(obj);
        }

        public void AddNestedObj(string path, GameObject obj) {
            FindObj(path).AddChild(obj);
        }

        public GameObject FindObj(string name) {
            return RootObj.FindChildRecursive(name);
        }

        public void AddEmptyObj(params string[] names) {
            foreach (var name in names) {
                RootObj.AddChild(new GameObject(name));
            }
        }

        internal bool DeleteObjSafe(string name) {
            GameObject obj = RootObj.FindChildRecursive(name);
            if (obj != null) {
                obj.Deleted = true;
                return true;
            }

            return false;
        }

        public void LoadContent() {
            Particle.Initialize();
            Load();
        }

        protected abstract void Load();
        protected abstract void Unload();

        public void UnloadContent() {
            ParticleRegistry.Clear();

            Unload();
        }

        // called every time a world is swapped in. compare to Initialize() which is called only the first time.
        public virtual void Enter() { }

        public List<ObjectSeed> LoadSeeds(string seedLocation) {
            string path = Path.Combine("data", "seeds", seedLocation);
            return GameData.Deserialize<List<ObjectSeed>>(path);
        }

        public void LoadAndAddSeeds(string seedLocation) {
            var objectSeeds = LoadSeeds(seedLocation);

            foreach (var seed in objectSeeds) {
                GameObject obj = seed.Build();

                if (seed.ParentName == null) {
                    Pigeon.World.AddObj(obj);
                } else {
                    Pigeon.World.FindObj(seed.ParentName).AddChild(obj);
                }
            }
        }

        public virtual void Update() {
            taskRegistry.Update();
            RootObj.Update();
            ParticleRegistry.Update();

            ColliderStrategy?.Collide(Hitboxes);

            RootObj.FinalUpdate();
        }

        public virtual void Draw() {
            Pigeon.Renderer.RenderGame(renderAll, BackgroundColor);
        }

        private void renderAll() {
            RootObj.Draw();

            foreach (Particle particle in ParticleRegistry.Objects) {
                particle.Draw();
            }

            if (ColliderStrategy != null && DrawColliderDebugInfo) {
                ColliderStrategy.Draw();
            }
        }

        public void AddTask(float time, Action action) {
            taskRegistry.Add(action, time);
        }
    }
}