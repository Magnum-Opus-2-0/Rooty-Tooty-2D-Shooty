using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject /*: ScriptableObject*/ {

    public enum TileType {

        EMPTY,
        TRAVERSABLE,
        NON_TRAVERSABLE,
        RESOURCE
    }

    public TileObject() {

        init(TileType.EMPTY, System.Int32.MaxValue, null, null);
    }

    public TileObject(TileType type) {

        init(type, System.Int32.MaxValue, null, null);
    }

    public TileObject(TileType type, Coord2DObject location) {

        init(type, System.Int32.MaxValue, null, location);
    }

    public TileObject(TileType type, int distance) {

        init(type, distance, null, null);
    }

    public TileObject(TileObject other) {

        init(other.type, other.distance, other.prev, other.location);
    }

    public char getChar() {

        switch (type) {

            case TileType.EMPTY:
                return '.';
            case TileType.TRAVERSABLE:
                return 't';
            case TileType.NON_TRAVERSABLE:
                return 'N';
            default:
                return '?';
        }
    }

    public override string ToString() {

        return "Type " + this.getChar() + ", "
            + "distance = " + (distance == System.Int32.MaxValue ? "MAX_VALUE" : distance + "") + ", "
            + "location " + (location == null ? "uninitialized" : location.ToString());
    }

    //public override bool Equals(object obj) {

    //    if (!this.GetType().IsAssignableFrom(obj.GetType()))
    //        return false;

    //    TileObject otherTile = (TileObject)obj;

    //    return type == otherTile.type
    //        && distance == otherTile.distance
    //        && location.Equals(otherTile.location);
    //}

    //public override int GetHashCode() {

    //    //if (location != null)
    //    //    return location.GetHashCode() + (distance % 29);

    //    //else
    //    //    return type.GetHashCode() + (distance % 17);

    //    return ToString().GetHashCode();
    //}

    private void init(TileType t, int dist, TileObject prev, Coord2DObject location) {

        this.type = t;
        this.distance = dist;
        this.prev = prev;
        this.location = location;
    }

    public TileType type { get; set; }
    public int distance { get; set; }
    public TileObject prev { get; set; }
    public Coord2DObject location { get; set; }
}
