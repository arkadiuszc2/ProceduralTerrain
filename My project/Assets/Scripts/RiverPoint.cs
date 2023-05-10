using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverPoint 
{
    public int x;
    public int y;
    public float height;
    public float distToEnd;
    public float fCost;
    public RiverPoint parent;

    public RiverPoint(int x, int y, float height, float distToEnd, RiverPoint parent) {
        this.x = x;
        this.y = y;
        this.height = height;
        this.distToEnd = distToEnd;
        this.fCost = height*10000 + distToEnd;
        this.parent = parent;
    }

    public override bool Equals(object obj) {

        if (obj == null) {
            return false;
        }

        if(!(obj is RiverPoint)) {
            return false;
        }

        return (this.x == ((RiverPoint)obj).x) && (this.y == ((RiverPoint)obj).y);


    }

    public override int GetHashCode() {
        return x+y;
    }
}
