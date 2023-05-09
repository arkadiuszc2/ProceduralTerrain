using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue {
    private SortedSet<RiverPoint> priorityQueue = new SortedSet<RiverPoint>(new RiverPointsComparer());

    public void Enqueue(RiverPoint item) {
        priorityQueue.Add(item);
    }

    public RiverPoint Dequeue() {
        RiverPoint item = priorityQueue.Min;
        priorityQueue.Remove(item);
        return item;
    }

    public bool Contains(RiverPoint riverPoint) {
        if (priorityQueue.Contains(riverPoint)) {
            return true;
        }

        return false;
    }

    public int Count {
        get { return priorityQueue.Count; }
    }

    private class RiverPointsComparer : IComparer<RiverPoint> {
        public int Compare(RiverPoint p1, RiverPoint p2) {
            if (p1.fCost < p2.fCost) {
                return -1;
            } else if (p1.fCost > p2.fCost) {
                return 1;
            } else {
                return 0;
            }
        }
    }
}