using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverPointsComparer : IComparer<RiverPoint> {
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
