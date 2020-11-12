using System;

namespace pigeon.gameobject {
    public abstract class Component {
        public GameObject Object;

        public Action Destructor;
        private bool initialized;

        public bool Enabled { get; set; } = true;

        public T GetComponent<T>() where T : Component {
            return Object.GetComponent<T>();
        }

        internal void InitializeComponent() {
            if (!initialized) {
                Initialize();
            }

            initialized = true;
        }

        internal void UpdateComponent() {
            if (!Enabled) {
                return;
            }

            Update();
        }

        protected abstract void Initialize();
        protected abstract void Update();
    }
}
