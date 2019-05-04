using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameGrid2DObject : Grid2DObject {

    public GameGrid2DObject(Coord2DObject dimensions, int thickness, int landmarks, int baseWidth)
        : base(dimensions) {

        init(thickness, landmarks, baseWidth);
    }

    public GameGrid2DObject(Grid2DObject other)
        : base(other) {

        init(4, 8, 8);
    }

    /// <summary>
    /// Procedurally initializes this grid.
    /// This function ensures that at least one path (of specified thickness)
    /// will connect player 1's base with player 2's.
    /// It basically plays connect-the-dots with player 1's base as the fist dot,
    /// several randomly generated dots in the middle of the grid,
    /// and with player 2's base as the final dot.
    /// Dots are connected using the Dijkstra's logic from PathObject.populateBestPath().
    /// To increase "openness", increase thickness.
    /// To increase complexity of dots, increase numLandmarks
    /// (in other words, number of points to generate between player 1 and player 2).
    /// </summary>
    /// <param name="thickness">Thickness of the thinnest path to be drawn between the two</param>
    /// <param name="numLandmarks">Number of random spots to visit between player 1 and player 2</param>
    /// <param name="baseWidth">For the square bases, the width in units of tiles</param>
    private void init(int thickness, int numLandmarks, int baseWidth) {

        BASE_WIDTH = baseWidth;

        // Also initializes p1LowLeft and p2UpRight
        drawBases();

        List<Coord2DObject> landmarks = getDistinctRandomPoints(numLandmarks);
        landmarks.Insert(0, p1UpRight);
        landmarks.Insert(landmarks.Count, p2LowLeft);

        // Draw preliminary paths with no layers
        List<PathObject> fullPath = getFullPath(landmarks, 0);
        foreach (PathObject p in fullPath)
            p.setPathType(TileObject.TileType.TRAVERSABLE, false);

        // Replace all empty tiles with non-traversables
        foreach (TileObject t in this) {

            if (t.type == TileObject.TileType.EMPTY)
                t.type = TileObject.TileType.NON_TRAVERSABLE;
        }

        // Increase thickness of the existing traversables
        fullPath = getFullPath(landmarks, thickness);
        foreach (PathObject p in fullPath) {

            p.setPathType(TileObject.TileType.TRAVERSABLE, true);
        }
    }

    /// <summary>
    /// Sets types for bases, which are BASE_WIDTH x BASE_WIDTH corners in super.grid.
    /// Also initializes p1UpRight and p2LowLeft, which specify these rectangles.
    /// (I know it's bad practice to make those coordinates member variables instead of local variables,
    /// but I don't feel like passing them through every function that needs them
    /// and dude it's like 3AM rn)
    /// </summary>
    private void drawBases() {

        int gridX = base.dimensions.x;
        int gridY = base.dimensions.y;

        Coord2DObject p1LowLeft = new Coord2DObject(0, 0);
                      p1UpRight = new Coord2DObject(p1LowLeft.x + BASE_WIDTH,
                                                    p1LowLeft.y + BASE_WIDTH);
        Coord2DObject p2UpRight = new Coord2DObject(gridX - 1, gridY - 1);
                      p2LowLeft = new Coord2DObject(p2UpRight.x - BASE_WIDTH + 1,
                                                    p2UpRight.y - BASE_WIDTH + 1);

        base.setTypeRect(p1LowLeft, p1UpRight, TileObject.TileType.TRAVERSABLE, true);
        base.setTypeRect(p2LowLeft, p2UpRight, TileObject.TileType.TRAVERSABLE, true);
    }

    /// <summary>
    /// Returns a list of random Coord2DObjects of specified size.
    /// As long as p1UpRight and p2LowLeft are initialized,
    /// you are guaranteed that this list will have no duplicate values,
    /// and will also not contain the values p1UpRight or p2LowLeft.
    /// Additionally, none of these points shall fall within the bases.
    /// </summary>
    /// <param name="amount">Number of random points to generate</param>
    /// <returns>List of distinct Coord2DObject's</returns>
    private List<Coord2DObject> getDistinctRandomPoints(int amount) {

        HashSet<Coord2DObject> pointsSet = new HashSet<Coord2DObject>();

        // Use a while loop instead of a for loop
        // because there's a small chance
        // that we could accidentally generate duplicate Coord2D's
        while (pointsSet.Count < amount) {

            Coord2DObject randCoord = getRandomNonBase();

            // These two will populate pointsSet later,
            // so check for duplicates now
            if (!randCoord.Equals(p1UpRight) && !randCoord.Equals(p2LowLeft))
                pointsSet.Add(randCoord);

        }

        // As far as this function is concerned,
        // order does not matter,
        // so we can clumsily return a list from our set
        return new List<Coord2DObject>(pointsSet);
    }

    /// <summary>
    /// Constructs a new Path (with the desired thickness) between each adjacent landmark given,
    /// and returns that series of Path objects.
    /// </summary>
    /// <param name="landmarks">Landmarks to be connected. Note that these don't necessarily have to be on the same line.</param>
    /// <param name="thickness">Thickness of the Path objects to be created.</param>
    /// <returns></returns>
    private List<PathObject> getFullPath(List<Coord2DObject> landmarks, int thickness) {

        List<PathObject> paths = new List<PathObject>(landmarks.Count);

        for (int i = 0; i < landmarks.Count - 1; i++) {

            Coord2DObject landmark1 = landmarks[i];
            Coord2DObject landmark2 = landmarks[i + 1];

            PathObject p = new PathObject(this, landmark1, landmark2, thickness);
            paths.Add(p);
        }

        return paths;
    }

    /// <summary>
    /// Get a random coordinate on the grid.
    /// The only constraints on this coordinate
    /// is that it cannot fall within either of the corners,
    /// which are reserved for the player bases.
    /// </summary>
    /// <returns>A random coordinate on this grid, not falling in either base</returns>
    private Coord2DObject getRandomNonBase() {

        int xGrid = base.dimensions.x;
        int yGrid = base.dimensions.y;

        int x, y;

        x = Random.Range(0, xGrid);

        // If x is within range of base1, y needs to dodge base1
        if (x <= p1UpRight.x) {

            y = Random.Range(0, yGrid - p1UpRight.y);
            y += p1UpRight.y;
        }

        // else if x is within range of base2, y needs to dodge base2
        else if (p2LowLeft.x <= x) {

            y = Random.Range(0, p2LowLeft.y);
        }

        // else, y doesn't need to dodge anything
        else {

            y = Random.Range(0, yGrid);
        }

        return new Coord2DObject(x, y);
    }



    private int BASE_WIDTH;

    private Coord2DObject p1UpRight;
    private Coord2DObject p2LowLeft;
}
