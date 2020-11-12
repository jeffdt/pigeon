using System;
using System.Collections.Generic;

namespace pigeon.core.tasks {
    public class TaskRegistry {
        private readonly List<TimedTask> toAdd = new List<TimedTask>();
        private readonly List<TimedTask> toRemove = new List<TimedTask>();
        private readonly List<TimedTask> tasks = new List<TimedTask>();

        public void Add(Action callback, float time) {
            toAdd.Add(new TimedTask(callback, time));
        }

        public void Update() {
            addNew();
            removeOld();

            updateCurrent();
        }

        private void updateCurrent() {
            foreach (var task in tasks) {
                var isFinished = task.Update();
                if (isFinished) {
                    toRemove.Add(task);
                }
            }
        }

        private void addNew() {
            foreach (var task in toAdd) {
                tasks.Add(task);
            }
            toAdd.Clear();
        }

        private void removeOld() {
            foreach (var task in toRemove) {
                tasks.Remove(task);
            }
            toRemove.Clear();
        }
    }
}
