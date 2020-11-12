using System.Collections.Generic;

namespace pigeon.gameobject {
    public class Egg {
        // this could potentially be completed to give this engine Prefab technology, but it would get sloppy to implement the Clone() function for every single component
        // if we decide to use this we could create an abstract Clonable method on the Component class and have to implement Clone() for each one we create, which would suck

        public string Name;
        public float Layer;

        private int instanceCount;
        private readonly List<Egg> children = new List<Egg>();
        private readonly List<Component> cmpts = new List<Component>();

        public GameObject Clone() {
            var obj = new GameObject { Name = Name + instanceCount++, Layer = Layer };

            foreach (var child in children) {
                obj.AddChild(child.Clone());
            }

            foreach (var cmpt in cmpts) {
                // TODO: clone a component somehow (don't just copy the reference)
                // if you figure this part out then the whole thing should work
            }

            return obj;
        }

        public void AddChild(Egg newChild) {
            children.Add(newChild);
        }

        public void AddChildren(params Egg[] newChildren) {
            foreach (var child in newChildren) {
                children.Add(child);
            }
        }

        public void AddComponent(Component cmpt) {
            cmpts.Add(cmpt);
        }

        public void AddComponents(params Component[] newCmpts) {
            foreach (var cmpt in newCmpts) {
                cmpts.Add(cmpt);
            }
        }
    }
}
