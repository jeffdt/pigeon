using System;
using System.Collections.Generic;

namespace pigeon.core.events {
    public class EventRegistry {
        public delegate void SimpleEventHandler(object sender);

        private readonly Dictionary<Type, EventHandler> eventHandlers = new Dictionary<Type, EventHandler>();
        private readonly Dictionary<string, SimpleEventHandler> simpleEventHandlers = new Dictionary<string, SimpleEventHandler>();
        private readonly Dictionary<string, Action> notificationHandlers = new Dictionary<string, Action>();

        public void Reset() {
            eventHandlers.Clear();
            notificationHandlers.Clear();
        }

        #region register
        public void RegisterEventHandler<T>(EventHandler handler) where T : EventArgs {
            Type eventType = typeof(T);

            if (!eventHandlers.ContainsKey(eventType)) {
                eventHandlers.Add(eventType, handler);
            } else {
                eventHandlers[eventType] += handler;
            }
        }

        public void RegisterSimpleEventHandler(string type, SimpleEventHandler handler) {
            if (!simpleEventHandlers.ContainsKey(type)) {
                simpleEventHandlers.Add(type, handler);
            } else {
                simpleEventHandlers[type] += handler;
            }
        }

        public void RegisterNotificationHandler(string type, Action handler) {
            if (!notificationHandlers.ContainsKey(type)) {
                notificationHandlers.Add(type, handler);
            } else {
                notificationHandlers[type] += handler;
            }
        }
        #endregion

        #region unregister
        public void UnregisterEventHandler<T>(EventHandler handler) where T : EventArgs {
            Type eventType = typeof(T);

            if (!eventHandlers.ContainsKey(eventType)) {
                throw new InvalidOperationException(@"handler " + handler + @" cannot be unregistered for event type " + eventType + @" because it was not registered.");
            }

            eventHandlers[eventType] -= handler;
        }

        public void UnregisterSimpleEventHandler(string type, SimpleEventHandler handler) {
            if (!simpleEventHandlers.ContainsKey(type)) {
                throw new InvalidOperationException(@"handler " + handler + @" cannot be unregistered for simple event type " + type + @" because it was not registered.");
            }

            simpleEventHandlers[type] -= handler;
        }

        public void UnregisterNotificationHandler(string type, Action handler) {
            if (!notificationHandlers.ContainsKey(type)) {
                throw new InvalidOperationException(@"handler " + handler + @" cannot be unregistered for notification type " + type + @" because it was not registered.");
            }

            notificationHandlers[type] -= handler;
        }
        #endregion

        #region raise
        public void RaiseEvent(object sender, EventArgs e) {
            Type eventType = e.GetType();

            if (eventHandlers.TryGetValue(eventType, out EventHandler handler) && handler != null) {
                handler(sender, e);
            }
        }

        public void RaiseEmptyEvent<T>() {
            if (eventHandlers.ContainsKey(typeof(T)) && eventHandlers[typeof(T)] != null) {
                eventHandlers[typeof(T)](null, null);
            }
        }

        public void RaiseNotification(string type) {
            if (notificationHandlers.TryGetValue(type, out Action handler) && handler != null) {
                handler();
            }
        }

        public void RaiseSimpleEvent(string type, object sender) {
            if (simpleEventHandlers.TryGetValue(type, out SimpleEventHandler handler) && handler != null) {
                handler(sender);
            }
        }
        #endregion
    }
}
