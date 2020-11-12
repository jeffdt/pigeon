using System;

namespace pigeon.core.tasks {
    public class TimedTask {
        private readonly Action callback;
        private float time;

        public TimedTask(Action callback, float time) {
            this.callback = callback;
            this.time = time;
        }

        public bool Update() {
            time -= pigeon.time.Time.SecScaled;

            if (time <= 0) {
                callback.Invoke();
                return true;
            }

            return false;
        }
    }
}
