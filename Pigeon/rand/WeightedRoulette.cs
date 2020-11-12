using System;
using System.Collections.Generic;

namespace pigeon.rand {
    public class WeightedRoulette<T> {
        public int Size { get { return slots.Count; } }

        private readonly List<RouletteSlot<T>> slots = new List<RouletteSlot<T>>();
        private int totalWeight = 0;

        public void AddObject(T obj, int weight) {
            validateWeight(weight, obj);
            totalWeight += weight;
            slots.Add(new RouletteSlot<T>(obj, weight));
        }

        public T Spin() {
            int val = Rand.Int(totalWeight);

            foreach (RouletteSlot<T> slot in slots) {
                if (val < slot.Weight) {
                    return slot.Object;
                }

                val -= slot.Weight;
            }

            throw new InvalidOperationException("unable to choose random roulette value. totalWeight: " + totalWeight);
        }

        private static void validateWeight(int weight, T obj) {
            if (weight < 0) {
                throw new ArgumentException("roulette weight for object [" + obj + "] cannot be less than 0");
            }
        }

        private class RouletteSlot<S> {
            public readonly int Weight;
            public readonly S Object;

            public RouletteSlot(S obj, int weight) {
                Weight = weight;
                Object = obj;
            }
        }
    }
}
