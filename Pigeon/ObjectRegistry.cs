using System.Collections.Generic;

namespace pigeon {
    public class ObjectRegistry<T> where T : IRegistryObject {
        public readonly List<T> Objects = new List<T>();
        private readonly List<T> toAdd = new List<T>();
        private readonly List<T> toRemove = new List<T>();

        public void Clear() {
            Objects.Clear();
            toAdd.Clear();
            toRemove.Clear();
        }

        public void Register(T obj) {
            toAdd.Add(obj);
        }

        public void Unregister(T obj) {
            toRemove.Add(obj);
        }

        public void Update() {
            addNew();
            removeOld();
            updateCurrent();
        }

        private void updateCurrent() {
            foreach (var obj in Objects) {
                var isDeleted = obj.Update();

                if (isDeleted) {
                    obj.OnUnregister();
                    toRemove.Add(obj);
                }
            }
        }

        private void addNew() {
            foreach (var obj in toAdd) {
                Objects.Add(obj);
            }
            toAdd.Clear();
        }

        private void removeOld() {
            foreach (var obj in toRemove) {
                Objects.Remove(obj);
            }
            toRemove.Clear();
        }
    }

    public interface IRegistryObject {
        bool Update();
        void OnUnregister();
    }
}
