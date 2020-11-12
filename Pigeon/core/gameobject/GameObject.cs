using System.Collections.Generic;
using Microsoft.Xna.Framework;
using pigeon.debugger;
using pigeon.rand;
using pigeon.collision;
using pigeon.time;
using pigeon.utilities;
using pigeon.gfx.drawable;

namespace pigeon.gameobject {
    public class GameObject {
        #region debugging
        public static bool DrawPositionsGlobal;
        public static bool DrawHitboxesGlobal;

        public bool DrawPosition;
        public bool DrawHitbox;
        #endregion

        #region position
        private Vector2 trueLocalPosition;
        private Point flatLocalPosition;
        private Point newWorldPosition;

        public Point LocalPosition {
            get { return flatLocalPosition; }
            set {
                trueLocalPosition = new Vector2(value.X, value.Y);
                flatLocalPosition = new Point(value.X, value.Y);

                updateWorldPosition();
            }
        }

        public Point WorldPosition {
            get { return newWorldPosition; }
            set {
                if (Parent == null) {
                    LocalPosition = value;
                } else {
                    LocalPosition = value - Parent.newWorldPosition;
                }
            }
        }

        public void SetPosX(int x) {
            trueLocalPosition.X = x;
            flatLocalPosition.X = x;

            updateWorldPosition();
        }

        public void SetPosY(int y) {
            trueLocalPosition.Y = y;
            flatLocalPosition.Y = y;

            updateWorldPosition();
        }

        private void updateWorldPosition() {
            if (Parent != null) {
                newWorldPosition = Parent.newWorldPosition + flatLocalPosition;
            } else {
                newWorldPosition = flatLocalPosition;
            }

            if (children != null) {
                for (int i = 0; i < children.Count; i++) {
                    children[i].updateWorldPosition();
                }
            }

            if (children_toAdd != null) {
                for (int i = 0; i < children_toAdd.Count; i++) {
                    children_toAdd[i].updateWorldPosition();
                }
            }
        }

        public void FlattenPosition() {
            trueLocalPosition = LocalPosition.ToVector2();
        }
        #endregion

        #region velocity
        public bool IsStatic = true;

        private Vector2 _velocity;

        public Vector2 Velocity {
            get { return _velocity; }
            set {
                _velocity = value;
                IsStatic = false;
            }
        }

        public Vector2 SpeculativePosition;

        public Point SpeculativeWorldPositionAt(float deltaTime) {
            var speculativePosition = trueLocalPosition + (Velocity * deltaTime);
            return new Point(((int) speculativePosition.X) + Parent.WorldPosition.X, ((int) speculativePosition.Y) + Parent.WorldPosition.Y);
        }

        public float Speed {
            get { return Velocity.Length(); }
            set { Velocity.Scale(value); }
        }

        public void Stop(bool setStatic = false) {
            Velocity = Vector2.Zero;
            if (setStatic) {
                IsStatic = true;
            }
        }

        public void SetVelocity(Vector2 direction, float speed) {
            Velocity = direction.Scale(speed);
        }

        public void SetVelocityEightway(Point direction, float speed) {
            SetVelocity(direction.ToVector2(), speed);
        }

        public void SetVelocityEightway(Vector2 direction, float speed) {
            var dir8way = direction.AlignToEightWay();
            SetVelocity(dir8way, speed);
        }
        #endregion

        #region layers
        public bool LayerInheritanceEnabled = true;
        public bool LayerSortingVarianceEnabled = true;

        internal float sortVariance = Rand.Float(0f, .0001f);

        public float Layer;

        public float DrawLayer {
            get {
                float drawLayer = Layer;

                if (LayerInheritanceEnabled && Parent != null) {
                    drawLayer += Parent.DrawLayer;
                }

                if (LayerSortingVarianceEnabled) {
                    drawLayer += sortVariance;
                }

                return MathHelper.Clamp(drawLayer, 0f, 1f);
            }
        }
        #endregion

        #region control
        private bool isUpdateEnabled { get { return UpdateEnabled && (Parent?.isUpdateEnabled != false); } }
        private bool isDrawEnabled { get { return DrawEnabled && (Parent?.isDrawEnabled != false); } }
        public bool UpdateEnabled = true;
        public bool DrawEnabled = true;
        #endregion

        #region flipping

        public bool FlipWithParent { get; set; } = true;

        public bool FlipLocalPositionWithParent { get; set; }

        public void SetFlipX(bool x) {
            SetFlipping(x, _flippedY);
        }

        public void SetFlipY(bool y) {
            SetFlipping(_flippedX, y);
        }

        public void SetFlipping(bool x, bool y) {
            bool oldX = _flippedX;
            bool oldY = _flippedY;

            _flippedX = x;
            _flippedY = y;

            if (flippableCmpts != null) {
                foreach (var flippable in flippableCmpts) {
                    flippable.OnFlipped();
                }
            }

            flipChildren(x, y, children, oldX, oldY);
            flipChildren(x, y, children_toAdd, oldX, oldY);
        }

        private static void flipChildren(bool x, bool y, List<GameObject> squabjects, bool oldX, bool oldY) {
            if (squabjects != null) {
                foreach (var child in squabjects) {
                    if (child.FlipWithParent) {
                        child.SetFlipping(x, y);
                    }

                    if (child.FlipLocalPositionWithParent) {
                        float newX = child.trueLocalPosition.X;
                        float newY = child.trueLocalPosition.Y;

                        if (oldX != x) {
                            newX *= -1;
                        }

                        if (oldY != y) {
                            newY *= -1;
                        }

                        child.trueLocalPosition = new Vector2(newX, newY);
                    }
                }
            }
        }

        public bool IsFlippedX() {
            return _flippedX;
        }

        public bool IsFlippedY() {
            return _flippedY;
        }

        private bool _flippedX;
        private bool _flippedY;
        #endregion

        #region children
        private List<GameObject> children;
        private List<GameObject> children_toAdd;
        private List<Component> toInitialize;

        public GameObject AddChild(GameObject newChild) {
            if (children == null) {
                children = new List<GameObject>();
            }

            if (children_toAdd == null) {
                children_toAdd = new List<GameObject>();
            }

            newChild.Parent = this;
            newChild.updateWorldPosition();

            children_toAdd.Add(newChild);

            return this;
        }

        public void AddChildImmediate(GameObject newChild) {
            newChild.Parent = this;
            newChild.updateWorldPosition();

            children.Add(newChild);
        }

        public GameObject AddContainer(string name) {
            var obj = new GameObject(name);
            AddChild(obj);
            return obj;
        }

        public void AddContainers(params string[] names) {
            foreach (var name in names) {
                AddChild(new GameObject(name));
            }
        }

        public void DeleteChildren() {
            if (children == null) {
                return;
            }

            foreach (var child in children) {
                child.Deleted = true;
            }
        }

        public List<GameObject> GetChildren() {
            return children;
        }

        public List<GameObject> GetChildrenToAdd() {
            return children_toAdd;
        }

        public int GetChildrenCount() {
            return children?.Count ?? 0;
        }

        public GameObject GetChild(int index) {
            return children[index];
        }

        public bool HasChildren() {
            return children?.Count > 0;
        }

        public GameObject FindChild(string name) {
            // grab from active children if possible, otherwise check the children about to be added
            if (children != null) {
                foreach (var child in children) {
                    if (child.Name == name && !child.Deleted) {
                        return child;
                    }
                }
            }

            if (children_toAdd != null) {
                foreach (var child in children_toAdd) {
                    if (child.Name == name && !child.Deleted) {
                        return child;
                    }
                }
            }

            return null;
        }

        public GameObject FindChildRecursive(string search) {
            int ind = search.IndexOf('.');

            if (ind == -1) {
                return FindChild(search);
            }

            var nextChild = FindChild(search.Substring(0, ind));
            int nextStartInd = ind + 1;
            int nextLength = search.Length - ind - 1;

            return nextChild?.FindChildRecursive(search.Substring(nextStartInd, nextLength));
        }

        public void SafeDeleteChild(string name) {
            var child = FindChild(name);
            if (child != null) {
                child.Deleted = true;
            }
        }

        #endregion

        #region components
        internal readonly List<Component> components = new List<Component>();
        private List<IRenderable> drawableCmpts;
        private List<IFlippable> flippableCmpts;

        public GameObject AddComponent(Component cmpt) {
            if (cmpt.Object == null) {
                cmpt.Object = this;
                components.Add(cmpt);

                if (cmpt is IRenderable) {
                    (drawableCmpts ?? (drawableCmpts = new List<IRenderable>())).Add(cmpt as IRenderable);
                }

                if (cmpt is IFlippable) {
                    (flippableCmpts ?? (flippableCmpts = new List<IFlippable>())).Add(cmpt as IFlippable);
                }

                (toInitialize ?? (toInitialize = new List<Component>())).Add(cmpt);
            } else {
                Pigeon.Console.Error("cannot add a component to more than one object.");
            }

            return this;
        }

        public Component GetComponent(int index) {
            return index < components.Count ? components[index] : null;
        }

        public T GetComponent<T>() where T : Component {
            for (int index = 0; index < components.Count; index++) {
                var component = components[index];
                if (component is T) {
                    return component as T;
                }
            }

            return null;
        }

        public int ComponentCount { get { return components?.Count ?? 0; } }

        public void RemoveComponent(Component cmpt) {
            cmpt.Destructor?.Invoke();

            if (cmpt is IRenderable drawable) {
                drawableCmpts.Remove(drawable);
            }

            if (cmpt is IFlippable flippable) {
                flippableCmpts.Remove(flippable);
            }

            components.Remove(cmpt);
        }
        #endregion

        #region tags
        private List<string> tags;

        public void AddTag(string tag) {
            (tags ?? (tags = new List<string>())).Add(tag);
        }

        public void AddTags(params string[] newTags) {
            if (tags == null) {
                tags = new List<string>();
            }

            for (int i = 0; i < newTags.Length; i++) {
                tags.Add(newTags[i]);
            }
        }

        public bool HasTag(string tag) {
            return tags?.Contains(tag) ?? false;
        }
        #endregion

        public string Name;
        public GameObject Parent;
        public bool Deleted;

        private GameObject newParent;

        public GameObject() { }

        public GameObject(string name) {
            Name = name;
        }

        public void Update() {
            if (toInitialize?.Count > 0) {
                for (int index = 0; index < toInitialize.Count; index++) {
                    var cmpt = toInitialize[index];
                    cmpt.InitializeComponent();
                }
                toInitialize.Clear();
            }

            if (isUpdateEnabled) {
                for (int index = 0; index < components.Count; index++) {
                    var component = components[index];
                    if (component.Enabled) {
                        component.UpdateComponent();
                    }
                }
            }
            if (isUpdateEnabled && !IsStatic) {
                UpdateSpeculativePosition();
            } else {
                SpeculativePosition = LocalPosition.ToVector2();
            }

            if (children != null) {
                updateChildren();
            }
        }

        public void UpdateSpeculativePosition() {
            SpeculativePosition = trueLocalPosition + (Velocity * Time.SecScaled);
        }

        private void updateChildren() {
            if (children_toAdd?.Count > 0) {
                foreach (var child in children_toAdd) {
                    children.Add(child);
                    child.Parent = this;
                }
                children_toAdd.Clear();
            }

            if (children != null) {
                foreach (var child in children) {
                    child.Update();
                }

                for (int index = children.Count - 1; index >= 0; index--) {
                    var child = children[index];

                    if (child.Deleted) {
                        children.RemoveAt(index);
                        child.Delete();
                    } else if (child.newParent != null) {
                        children.RemoveAt(index);
                        child.newParent.AddChildImmediate(child);
                        child.newParent = null;
                    }
                }
            }
        }

        internal void Delete() {
            Parent = null;

            foreach (var component in components) {
                component.Destructor?.Invoke();
            }

            if (children != null) {
                for (int index = children.Count - 1; index >= 0; index--) {
                    var child = children[index];
                    child.Delete();
                }
            }
        }

        public void Draw() {
            if (drawableCmpts != null && isDrawEnabled) {
                foreach (var cmpt in drawableCmpts) {
                    cmpt.Draw();
                }
            }

            if (children != null) {
                foreach (var child in children) {
                    child.Draw();
                }
            }

            if (DrawPositionsGlobal || DrawPosition) {
                DebugHelper.DrawDot(WorldPosition, new Color(.1f, 1f, .1f, 1f));
            }

            if (DrawHitboxesGlobal || DrawHitbox) {
                var hitbox = GetComponent<ColliderComponent>();

                if (hitbox?.Enabled == true) {
                    hitbox.Draw();
                }
            }
        }

        public void FinalUpdate() {
            if (!IsStatic && Velocity != Vector2.Zero) {
                trueLocalPosition = SpeculativePosition;

                Point newFlatLocalPosition = new Point((int) SpeculativePosition.X, (int) SpeculativePosition.Y);
                if (flatLocalPosition.X != newFlatLocalPosition.X || flatLocalPosition.Y != newFlatLocalPosition.Y) {
                    flatLocalPosition = newFlatLocalPosition;
                    updateWorldPosition();
                }
            }

            if (children != null) {
                for (int i = 0; i < children.Count; i++) {
                    children[i].FinalUpdate();
                }
            }
        }

        public override string ToString() {
            var inspector = new ObjectInspector2();
            inspector.AppendField("Name", Name);
            inspector.AppendField("WPos", WorldPosition);
            inspector.AppendField("LPos", LocalPosition);
            inspector.AppendField("DrawLayer", DrawLayer);
            if (children?.Count > 0) {
                inspector.AppendField("Children", children.Count);
            }
            if (components?.Count > 0) {
                inspector.AppendField("Components", components.Count);
            }

            return inspector.ToString();
        }

        public void SetNewParent(GameObject parent) {
            newParent = parent;
        }

        public void SetNewParentImmediate(GameObject parent) {
            Parent.children.Remove(this);
            parent.AddChildImmediate(this);
        }
    }
}
