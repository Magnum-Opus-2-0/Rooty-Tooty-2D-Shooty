using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

public class Grid2DObject : /*ScriptableObject,*/ IEnumerable<TileObject> {



    public Grid2DObject(Coord2DObject dimensions) {

        this.dimensions = new Coord2DObject(dimensions);

        grid = new TileObject[dimensions.x, dimensions.y];

        for (int i = 0; i < dimensions.x; i++) {
            for (int j = 0; j < dimensions.y; j++) {

                setTile(TileObject.TileType.EMPTY, new Coord2DObject(i, j));
            }
        }
    }

    public Grid2DObject(Grid2DObject other) {

        this.dimensions = new Coord2DObject(other.dimensions);

        grid = new TileObject[dimensions.x, dimensions.y];

        for (int i = 0; i < dimensions.x; i++) {
            for (int j = 0; j < dimensions.y; j++) {

                Coord2DObject thisCoord2D = new Coord2DObject(i, j);
                TileObject thisOtherTile = other.getTile(thisCoord2D);
                setTile(thisOtherTile.type, thisCoord2D);
            }
        }
    }

    public override string ToString() {

        StringBuilder sb = new StringBuilder((dimensions.x + 2) * (dimensions.y + 3) + 1);

        // Top row border
        sb.Append('*');
        for (int i = 0; i < dimensions.x; i++)
            sb.Append('-');
        sb.Append('*');
        sb.Append('\n');

        // Actual grid
        for (int i = dimensions.x - 1; i >= 0; i--) {
            for (int j = 0; j < dimensions.y; j++) {

                // Left border
                if (j == 0)
                    sb.Append('|');

                TileObject thisTile = getTile(new Coord2DObject(i, j));
                sb.Append(thisTile.getChar());

                // Right border
                if (j == dimensions.y - 1)
                    sb.Append('|');
            }
            sb.Append('\n');
        }

        // Bottom row border
        sb.Append('*');
        for (int i = 0; i < dimensions.x; i++)
            sb.Append('-');
        sb.Append('\n');

        return sb.ToString();
    }

    public void setTile(TileObject.TileType type, Coord2DObject location) {

        assertBounds(location);

        grid[location.x, location.y] = new TileObject(type, new Coord2DObject(location));
    }

    public TileObject getTile(Coord2DObject location) {

        return grid[location.x, location.y];
    }

    public bool checkBounds(Coord2DObject location) {

        if (location.x < 0 || location.y < 0)
            return false;

        return location.x < dimensions.x && location.y < dimensions.y;
    }

    public void assertBounds(Coord2DObject location) {

        Assert.raiseExceptions = true;
        Assert.IsTrue(checkBounds(location), "Invalid coordinate " + location.ToString());
    }

    public int size() {

        return dimensions.x * dimensions.y;
    }

    public char getChar(Coord2DObject location) {

        return getTile(location).getChar();
    }

    public bool canGoUp(Coord2DObject location) {

        return checkBounds(new Coord2DObject(location.x, location.y + 1));
    }

    public bool canGoDown(Coord2DObject location) {

        return checkBounds(new Coord2DObject(location.x, location.y - 1));
    }

    public bool canGoLeft(Coord2DObject location) {

        return checkBounds(new Coord2DObject(location.x - 1, location.y));
    }

    public bool canGoRight(Coord2DObject location) {

        return checkBounds(new Coord2DObject(location.x + 1, location.y));
    }

    public TileObject getUp(Coord2DObject fromHere) {

        if (!canGoUp(fromHere)) return null;

        return getTile(new Coord2DObject(fromHere.x, fromHere.y + 1));
    }

    public TileObject getDown(Coord2DObject fromHere) {

        if (!canGoDown(fromHere)) return null;

        return getTile(new Coord2DObject(fromHere.x, fromHere.y - 1));
    }

    public TileObject getLeft(Coord2DObject fromHere) {

        if (!canGoLeft(fromHere)) return null;

        return getTile(new Coord2DObject(fromHere.x - 1, fromHere.y));
    }

    public TileObject getRight(Coord2DObject fromHere) {

        if (!canGoRight(fromHere)) return null;

        return getTile(new Coord2DObject(fromHere.x + 1, fromHere.y));
    }

    /// <summary>
    /// Sets all the tiles in a specified line on this grid to the specified type.
    /// Note that point1 and point2 MUST lie on the same line
    /// (i.e. either their x coordinates are equal or their y coordinates are equal),
    /// or I'm going to crash your program.
    /// If prioritize is false,
    /// this function will only mark tiles which are currently empty.
    /// </summary>
    /// <param name="point1">An endpoint</param>
    /// <param name="point2">Another endpoint</param>
    /// <param name="type">Tile type to set this line to</param>
    /// <param name="prioritize">Whether this method should override existing non-empty tiles</param>
    public void setTypeLine(Coord2DObject point1, Coord2DObject point2,
        TileObject.TileType type, bool prioritize) {

        assertBounds(point1); assertBounds(point2);

        Assert.raiseExceptions = true;
        Assert.IsTrue(point1.x == point2.x || point1.y == point2.y,
            "point1 " + point1.ToString() + " and point2 " + point2.ToString() + " must lie on a straight line");

        if (point1.Equals(point2)
            && (prioritize || getTile(point1).type == TileObject.TileType.EMPTY)) {

            getTile(point1).type = type;
            return;
        }

        // If on the same row
        if (point1.y == point2.y) {

            // Iterate from least x to greater x,
            // whichever is which
            int biggerX  = (point1.x > point2.x ? point1.x : point2.x);
            int smallerX = (point1.x < point2.x ? point1.x : point2.x);

            for (int i = smallerX; i <= biggerX; i++) {

                TileObject thisTile = getTile(new Coord2DObject(i, point1.y));

                if (thisTile == null) Debug.Log("i is " + i + ", point1.y is " + point1.y);

                if (prioritize || thisTile.type == TileObject.TileType.EMPTY)
                    thisTile.type = type;
            }
        }

        // Else, they're on the same column
        else {

            // Iterate from least y to greatest y,
            // whichever is which
            int biggerY  = (point1.y > point2.y ? point1.y : point2.y);
            int smallerY = (point1.y < point2.y ? point1.y : point2.y);

            for (int i = smallerY; i <= biggerY; i++) {

                TileObject thisTile = getTile(new Coord2DObject(point1.x, i));

                if (prioritize || thisTile.type == TileObject.TileType.EMPTY)
                    thisTile.type = type;
            }
        }
    }

    /// <summary>
    /// Sets all the tiles in a specified rectangle on this grid to the specified type.
    /// Note that lowerLeft and upperRight MUST have this relationship
    /// (for example, upperRight.y cannot be lower than lowerLeft.y),
    /// or I'm going to crash your program.
    /// If prioritize is false,
    /// this function will only mark tiles which are currently empty.
    /// Also, this function technically can mark single lines,
    /// but it's recommended that you use setTypeLine() instead for that.
    /// </summary>
    /// <param name="lowerLeft">Lower leftmost point of the desired rectangle</param>
    /// <param name="upperRight">Upper rightmost point of the desired rectangle</param>
    /// <param name="type">Tile type to set this rectangle to</param>
    /// <param name="prioritize">Whether this action should override existing non-empty tiles</param>
    public void setTypeRect(Coord2DObject lowerLeft, Coord2DObject upperRight,
        TileObject.TileType type, bool prioritize) {

        assertBounds(lowerLeft); assertBounds(upperRight);

        Assert.IsTrue(lowerLeft.x <= upperRight.x && lowerLeft.y <= upperRight.y,
            "Invalid argument order. "
            + "lowerLeft should be " + upperRight.ToString()
            + "and upperRight should be " + lowerLeft.ToString());

        // Just setTypeLine if they gave us a line instead of a rectangle
        if (lowerLeft.x == upperRight.x || lowerLeft.y == upperRight.y) {

            setTypeLine(lowerLeft, upperRight, type, prioritize);
            return;
        }

        // If we're here, then we're marking a non-line rectangle,
        // and the arguments were provided in correct order
        for (int thisY = lowerLeft.y; thisY <= upperRight.y; thisY++) {

            // Go row by row
            Coord2DObject thisRowLeft  = new Coord2DObject(lowerLeft.x,  thisY);
            Coord2DObject thisRowRight = new Coord2DObject(upperRight.x, thisY);

            setTypeLine(thisRowLeft, thisRowRight, type, prioritize);
        }
    }

    /// <summary>
    /// Sets all the tiles in a specified line on this grid to the specified type.
    /// This overload adds the "thickness" parameter.
    /// A single straight line of Tiles is considered a row with layers = 0 (total thickness 1).
    /// layers = 1 surrounds the line on each side with additional rows, for a total thickness of 3.
    /// All other parameters remain the same as the other overload for this method,
    /// including that part about me crashing your program
    /// if point1 and point2 aren't on the same line.
    /// </summary>
    /// <param name="point1">An endpoint</param>
    /// <param name="point2">Another endpoint</param>
    /// <param name="type">Tile type to set this line to</param>
    /// <param name="layers">Number of layers on each side of this line</param>
    /// <param name="prioritize">Whether this method should override existing non-empty tiles</param>
    public void setTypeLine(Coord2DObject point1, Coord2DObject point2,
        TileObject.TileType type, int layers, bool prioritize) {

        for (int thisLayer = 0; thisLayer <= layers; thisLayer++) {

            // Rows (horizontal)
            if (point1.y == point2.y) {

                // Do row on top, offset by thisLevel
                Coord2DObject point1Layered = new Coord2DObject(point1.x, point1.y + thisLayer);
                Coord2DObject point2Layered = new Coord2DObject(point2.x, point2.y + thisLayer);

                if (checkBounds(point1Layered) && checkBounds(point2Layered)) {

                    setTypeLine(point1Layered, point2Layered, type, prioritize);
                }



                // Do row on bot, offset by thisLevel
                point1Layered = new Coord2DObject(point1.x, point1.y - thisLayer);
                point2Layered = new Coord2DObject(point2.x, point2.y - thisLayer);

                if (checkBounds(point1Layered) && checkBounds(point2Layered)) {

                    setTypeLine(point1Layered, point2Layered, type, prioritize);
                }
            }

            // Columns (vertical)
            else if (point1.x == point2.x) {

                // Do column on left, offset by thisLevel
                Coord2DObject point1Layered = new Coord2DObject(point1.x - thisLayer, point1.y);
                Coord2DObject point2Layered = new Coord2DObject(point2.x - thisLayer, point2.y);

                if (checkBounds(point1Layered) && checkBounds(point2Layered)) {

                    setTypeLine(point1Layered, point2Layered, type, prioritize);
                }

                // Do column on right, offset by thisLevel
                point1Layered = new Coord2DObject(point1.x + thisLayer, point1.y);
                point2Layered = new Coord2DObject(point2.x + thisLayer, point2.y);

                if (checkBounds(point1Layered) && checkBounds(point2Layered)) {

                    setTypeLine(point1Layered, point2Layered, type, prioritize);
                }
            }

            else {

                Assert.IsTrue(false, "point1" + point1.ToString() + " and point2 " + point2.ToString() + " not on a line");
            }
        }
    }

    /// <summary>
    /// Get the set of tiles immediately adjacent (up, down, left, right) to the specified coordinate.
    /// Note that in cases where the given location is on an edge or corner,
    /// the returned set will a size less than the usual four.
    /// Also note that this only returns neighbors which aren't non-traversable,
    /// so the returned tiles may be of type EMPTY or TRAVERSABLE.
    /// </summary>
    /// <param name="location">Location of tile whose neighbors should be returned</param>
    /// <returns>HashSet of TileObject references to this location's eligible neighbors</returns>
    public HashSet<TileObject> getTraversableNeighbors(Coord2DObject location) {

        assertBounds(location);

        HashSet<TileObject> neighbors = new HashSet<TileObject>();

        if (canGoUp(location)) {

            TileObject upNeighbor = getUp(location);

            if (upNeighbor.type != TileObject.TileType.NON_TRAVERSABLE)
                neighbors.Add(upNeighbor);
        }

        if (canGoDown(location)) {

            TileObject downNeighbor = getDown(location);

            if (downNeighbor.type != TileObject.TileType.NON_TRAVERSABLE)
                neighbors.Add(downNeighbor);
        }

        if (canGoLeft(location)) {

            TileObject leftNeighbor = getLeft(location);

            if (leftNeighbor.type != TileObject.TileType.NON_TRAVERSABLE)
                neighbors.Add(leftNeighbor);
        }

        if (canGoRight(location)) {

            TileObject rightNeighbor = getRight(location);

            if (rightNeighbor.type != TileObject.TileType.NON_TRAVERSABLE)
                neighbors.Add(rightNeighbor);
        }

        return neighbors;
    }



    public IEnumerator<TileObject> GetEnumerator() {
        
        for (int i = 0; i < dimensions.x; i++) {
            for (int j = 0; j < dimensions.y; j++) {

                yield return getTile(new Coord2DObject(i, j));
            }
        }

        yield break;
    }

    IEnumerator IEnumerable.GetEnumerator() {

        for (int i = 0; i < dimensions.x; i++) {
            for (int j = 0; j < dimensions.y; j++) {

                yield return getTile(new Coord2DObject(i, j));
            }
        }

        yield break;
    }



    public Coord2DObject dimensions { get; }
    protected TileObject[,] grid;
}
