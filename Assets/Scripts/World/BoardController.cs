﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    /// <summary>
    /// default game tile used for copying and instantiating new tiles
    /// </summary>
    public GameObject emptyTileTemplate;
    public GameObject wallPieceTemplate;
    public GameObject obstaclePieceTemplate;

    public GameObject fluffTemplate;
    public GameObject fiberTemplate;
    public GameObject objectTemplate;

    /// <summary>
    /// Empty GameObject to store all generated tiles in hierarchy
    /// </summary>
    public GameObject tileFondler;
    public GameObject wallFondler;
    public GameObject obstacleFondler;



    /// I like 20 for pathLandmarks and 2 for pathWidth for a broader battlefield,
    /// or 30 pathLandmarks and 1 pathWidth for more obstacles.
    /// If we get minimaps working, 50 and 0 will be pretty fun.

    /// <summary>
    /// Broadly, the complexity of the map to be generated.
    /// Maps are generated by placing random points on the map,
    /// and then connecting them all in one traversable route.
    /// pathLandmarks is the number of random points the map gets.
    /// A value of 0 yields the most basic route possible.
    /// 
    /// More pathLandmarks are fun, but be careful!
    /// The grid generator runs in O( |pathLandmarks|^2 ) time,
    /// and the lag really gets noticable around pathLandmarks = 50,
    /// so try not to go too far above that.
    /// </summary>
    public int pathLandmarks;

    /// <summary>
    /// As stated above, maps are generated by connecting points on a map.
    /// The path can have a specified width. This width denotes the layers each path has.
    /// A pathWidth of 0 yields the thinnest possible path,
    /// i.e. a path which is only 1 tile in diameter.
    /// Increasing pathWidth to 1 yields a path which is 3 tiles in diameter,
    /// and so on.
    /// 
    /// You'll typically use a pathWidth no more than 3. I prefer 0 or 1 myself.
    /// When this goes over 5, the map become really open,
    /// so you won't want to do that unless you purposefully want a purely blank grid.
    /// In that case, use pathLandmarks = 0 and pathWidth = 20 or something lol
    /// </summary>
    public int pathWidth;

    private static readonly int TILE_WIDTH = 1;

    /// <summary>
    /// How many tiles there are in the z direction.
    /// </summary>
    private static readonly int Z_MAX = 50;
    /// <summary>
    /// How many tiles there are in the x direction.
    /// </summary>
    private static readonly int X_MAX = 50;

    /// <summary>
    /// A 2D array that holds references to all tiles on this board.
    /// This array is populated within GenerateGrid().
    /// </summary>
    public GameObject[,] tileArray = new GameObject[X_MAX, Z_MAX];

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Called at beginning of game.
    /// Generates a X_MAX x Z_MAX tile grid out of the tile gameobject
    /// </summary>
    public void GenerateGrid()
    {
        // Builds the walls that encompass the grid.
        wallPieceTemplate.SetActive(true);
        BuildAWall(-1, Z_MAX + 1, -1, 'z', wallPieceTemplate);
        BuildAWall(-1, Z_MAX + 1, X_MAX, 'z', wallPieceTemplate);
        BuildAWall(-1, X_MAX + 1, -1, 'x', wallPieceTemplate);
        BuildAWall(-1, X_MAX + 1, Z_MAX, 'x', wallPieceTemplate);
        wallPieceTemplate.SetActive(false);

        // Builds the actual grid.
        emptyTileTemplate.SetActive(true);
        for (int x = 0; x < X_MAX; x++)
        {
            for (int z = 0; z < Z_MAX; z++)
            {
                GameObject newTile = Instantiate(emptyTileTemplate, new Vector3(x + TILE_WIDTH - 1, 0, z + TILE_WIDTH - 1), Quaternion.Euler(90f, 0f, 0f));

                // Store a reference to this tile into a "holder" object, tileFondler
                newTile.transform.parent = tileFondler.transform;

                // Also store our own reference to this tile in our own 2D array
                tileArray[x, z] = newTile;
            }
        }
        emptyTileTemplate.SetActive(false);



        procedurallyPlaceObstacles(wallPieceTemplate);
    }

    /// <summary>
    /// Builds a wall! Keeps out all the illegal fluff.
    /// </summary>
    /// <param name="start">starting coordinate</param>
    /// <param name="end">endpoint of wall</param>
    /// <param name="constant">the coordinate of the axis that doesn't change</param>
    /// <param name="axis">what axis to build the wall along (either x, y, or z)</param>
    /// <param name="wallObject">the object that shall be instantiated</param>
    private void BuildAWall(int start, int end, int constant, char axis, GameObject wallObject)
    {

        for(int i = start; i < end; i++)
        {
            GameObject tempWall = null;

            switch (axis)
            {
                case 'x':
                    tempWall = Instantiate(wallObject, new Vector3(i, 0.5f, constant), Quaternion.identity);
                    break;
                case 'y':
                    tempWall = Instantiate(wallObject, new Vector3(0, i, 0), Quaternion.identity);
                    break;
                case 'z':
                    tempWall = Instantiate(wallObject, new Vector3(constant, 0.5f, i), Quaternion.identity);
                    break;
                default:
                    Debug.LogError("Invalid axis '" + axis + "' given.");
                    break;
            }

            // Store a reference to this wall piece into a "holder" object, wallFondler
            tempWall.transform.parent = wallFondler.transform;
        }
    }

    /// <summary>
    /// Places obstacle objects on the board,
    /// such that one can always traverse between the two bases on opposite corners of the board.
    /// Find some balance of the two paramters such that the board designs are generally favorable.
    /// These can be changed to your preference - you do you!
    /// Please note that this runs in O( |pathLandmarks|^2 ),
    /// and the lag really gets noticable around pathLandmarks = 50.
    /// </summary>
    /// <param name="obstacleTemplate">GameObject which should be instantiated as objects</param>
    private void procedurallyPlaceObstacles(GameObject obstacleTemplate) {

        // Before anything else, make sure our template is active
        obstacleTemplate.SetActive(true);

        // Also, tear down any leftover tiles hanging out in obstacleFondler from previous runs
        obstacleFondler.transform.DetachChildren();

        // First, generate new grid
        GameGrid2DObject tempGrid = new GameGrid2DObject(new Coord2DObject(X_MAX, Z_MAX), pathWidth, pathLandmarks);

        // Then, iterate through that grid.
        // Every time you see a Tile whose type is NON_TRAVERSABLE,
        // instantiate a Wall object at that location,
        // and add it to ObstacleFondler
        foreach (TileObject t in tempGrid) {

            if (t.type != TileObject.TileType.NON_TRAVERSABLE) continue;

            GameObject tempObstacle = Instantiate(obstacleTemplate,
                new Vector3(t.location.x, 0.5f, t.location.y),
                Quaternion.identity);

            tempObstacle.transform.parent = obstacleFondler.transform;
        }

        // Housekeeping - set our template to false again
        obstacleTemplate.SetActive(false);
    }
}
