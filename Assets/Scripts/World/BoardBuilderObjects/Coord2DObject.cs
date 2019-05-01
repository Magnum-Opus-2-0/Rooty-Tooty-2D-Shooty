using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coord2DObject /*: ScriptableObject*/ {

    public Coord2DObject(int x, int y) {

        this.x = x;
        this.y = y;
    }

    public Coord2DObject(Coord2DObject other) {

        this.x = other.x;
        this.y = other.y;
    }

    public override string ToString() {
        return "{" + x + ", " + y + "}";
    }

    public override bool Equals(object other) {

        if (!this.GetType().IsAssignableFrom(other.GetType()))
            return false;

        Coord2DObject otherCoord = (Coord2DObject)other;

        return x == otherCoord.x && y == otherCoord.y;
    }

    public override int GetHashCode() {

        return 19 * x + 31 * y;
    }

    public int x { get; set; }
    public int y { get; set; }

}
