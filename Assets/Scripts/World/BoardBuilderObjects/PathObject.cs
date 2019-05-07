using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PathObject /*: ScriptableObject*/ {

    public PathObject(Grid2DObject grid) {

        this.grid = grid;
        joints = new List<Coord2DObject>();
        this.thickness = 0;
    }

    public PathObject(Grid2DObject grid, Coord2DObject point1, Coord2DObject point2, int thickness) {

        this.grid = grid;
        joints = new List<Coord2DObject>();
        this.thickness = thickness;

        populateBestPath(point1, point2);
    }

    public PathObject(Grid2DObject grid, List<Coord2DObject> joints, int thickness) {

        this.grid = grid;
        this.joints = new List<Coord2DObject>(joints);
        this.thickness = thickness;
    }

    /// <summary>
    /// Checks if two coordinates are eligible to be a straight path.
    /// If they are diagonal relative to each other,
    /// this will return false.
    /// Otherwise, if they're on the same horizontal row or vertical column,
    /// this will return true.
    /// </summary>
    /// <param name="joint1">an endpoint</param>
    /// <param name="joint2">another endpoint</param>
    /// <returns>True if the points are compatible, false otherwise</returns>
    public bool areCompatibleJoints(Coord2DObject joint1, Coord2DObject joint2) {

        return joint1.x == joint2.x || joint1.y == joint2.y;
    }

    /// <summary>
    /// Add a joint to this path at the specified index.
    /// Adjacent points are required to be compatible,
    /// i.e. they lie on the same line as their adjacent joint.
    /// If you try to use this function and add a joint which isn't compatible with its neighbors,
    /// this function returns false and does NOT add the joint.
    /// Else, it adds the joint to this path and returns true.
    /// Don't worry if this Path isn't yet complete (i.e. doesn't have 2 or more joints);
    /// you can add joints with this method and still return true safely.
    /// </summary>
    /// <param name="newJoint">Joint to be added</param>
    /// <param name="index">Index in this list of joints</param>
    /// <returns>True if this joint was compatible and added successfully, false otherwise</returns>
    public bool addJoint(Coord2DObject newJoint, int index) {

        joints.Insert(index, newJoint);

        

        // Check compatibility with joint before, if it exists
        if (index > 0) {

            Coord2DObject leftNeighbor = joints[index - 1];

            if (!areCompatibleJoints(leftNeighbor, newJoint)) {

                joints.RemoveAt(index);
                return false;
            }
        }

        // Check compatibility with joint after, if it exists
        if (index < joints.Count - 1) {

            Coord2DObject rightNeighbor = joints[index + 1];

            if (!areCompatibleJoints(newJoint, rightNeighbor)) {

                joints.RemoveAt(index);
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Sets the type of tiles on this path.
    /// Should be used to either set a path as traversable,
    /// or to "erase" by setting them to empty (or something else!).
    /// Note that you must first set this object's path by giving it 2 or more joints,
    /// otherwise you're trying to draw an ill-defined path
    /// and I'm going to abort your program.
    /// </summary>
    /// <param name="type">Type that all tiles on this path should be set to</param>
    /// <param name="prioritize">True if should override non-empty tiles, false otherwise</param>
    public void setPathType(TileObject.TileType type, bool prioritize) {

        Assert.raiseExceptions = true;
        Assert.IsTrue(joints.Count >= 2,
            "Not enough joints in this path, here are all joints: " + joints.ToString());

        Coord2DObject firstJoint = joints[0];

        IEnumerator<Coord2DObject> e = joints.GetEnumerator();

        while (e.MoveNext()) {

            Coord2DObject secondJoint = e.Current;

            grid.setTypeLine(firstJoint, secondJoint, type, thickness, prioritize);

            // Slide first joint
            // (second joint is slid using the while-loop condition
            firstJoint = secondJoint;
        }
    }

    /// <summary>
    /// Populates this Path's "joints" list with the best path between point1 and point2.
    /// This is accomplished by implementing Dijkstra's algorithm.
    /// This path's grid and joints MUST be initialized before this method is called.
    /// Algorithm adopted from the pseudocode found here:
    /// https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm#Pseudocode
    /// </summary>
    /// <param name="src">The coordinate that the path should start from</param>
    /// <param name="dest">The coordinate that the path should start from</param>
    private void populateBestPath(Coord2DObject src, Coord2DObject dest) {

        Grid2DObject tempGrid = new Grid2DObject(grid); // generate copy, maintaining tile types

        TileObject srcTile  = tempGrid.getTile(src);
        TileObject destTile = tempGrid.getTile(dest);
        srcTile.distance = 0;

        // Do some error checking
        Assert.raiseExceptions = true;
        Assert.IsTrue(!src.Equals(dest), "Attempted autopath to the same tile " + src.ToString());
        Assert.IsTrue(srcTile.type != TileObject.TileType.NON_TRAVERSABLE,
            "Path attempted on srcTile  non-traversable tile " + srcTile.ToString() + " to destTile " + destTile.ToString());
        Assert.IsTrue(destTile.type != TileObject.TileType.NON_TRAVERSABLE,
            "Path attempted on destTile non-traversable tile " + destTile.ToString() + " from srcTile " + srcTile.ToString());

        // Populate set Q
        HashSet<TileObject> setQ = new HashSet<TileObject>();
        foreach (TileObject t in tempGrid) {

            if (t.type != TileObject.TileType.NON_TRAVERSABLE)
                setQ.Add(t);
        }
        List<TileObject> listQ = new List<TileObject>(setQ);
        shuffleList<TileObject>(listQ);

        TileObject uTile = null;

        while (listQ.Count > 0) {

            // Get tile with minimum distance from setQ
            int runningMin = System.Int32.MaxValue;
            foreach (TileObject t in listQ) {

                if (t.distance < runningMin) {

                    runningMin = t.distance;
                    uTile = t;
                }
            }

            // Make sure uTile is properly set,
            // then remove it from setQ
            Assert.IsTrue(uTile != null, "Minimum distance tile uTile not properly set");
            Assert.IsTrue(listQ.Contains(uTile), "setQ doesn't contain uTile " + uTile.ToString());
            listQ.Remove(uTile);

            // Break out if we've reached the destination,
            // we now need to construct the path via reverse iteration
            if (uTile == destTile)  // check for identity, not just equivalence
                break;

            // Update distances of all uTile's current neighbors
            HashSet<TileObject> uNeighbors = tempGrid.getTraversableNeighbors(uTile.location);

            foreach (TileObject thisNeighbor in uNeighbors) {

                int currentDist = uTile.distance + 1;

                if (currentDist < thisNeighbor.distance) {

                    thisNeighbor.distance = currentDist;
                    thisNeighbor.prev = uTile;
                }
            }
        }



        // Ensure that uTile is actually usable
        Assert.IsTrue(uTile.prev != null || uTile == srcTile,
            "Condition specified by Dijkstra's not met");

        // Populate joints by backtracing
        while (uTile != null) {

            joints.Add(uTile.location);
            uTile = uTile.prev;

            // Make sure if we're about to break out,
            // that we have enough joints to do so
            // (i.e. that we have at least 2 joints)
            Assert.IsTrue(!(uTile == null && joints.Count < 2),
                "Not enough prev's? For sure not enough joints\n"
                + "Perhaps src and dest are the same?\n"
                + "src:  " + srcTile.ToString() + '\n'
                + "dest: " + destTile.ToString() + '\n'
                + "src.equals(dest)? " + src.Equals(dest));
        }
    }

    private void shuffleList<T>(List<T> list)
    {

        for (int i = 0; i < list.Count; i++)
        {

            int swapHere = Random.Range(0, list.Count);

            T temp = list[swapHere];
            list[swapHere] = list[i];
            list[i] = temp;
        }
    }


    private Grid2DObject grid;
    private List<Coord2DObject> joints;
    private int thickness;
}
