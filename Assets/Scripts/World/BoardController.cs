using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour {
    /// <summary>
    /// default game tile used for copying and instantiating new tiles
    /// </summary>
    public GameObject emptyTileTemplate;

    public GameObject fluffTemplate;
    public GameObject fiberTemplate;
    public GameObject objectTemplate;

    /// <summary>
    /// Empty GameObject to store all generated tiles in hierarchy
    /// </summary>
    public GameObject tileFondler;

    private static readonly int TILE_WIDTH = 10;


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
    void Start() {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// Called at beginning of game.
    /// Generates a X_MAX x Z_MAX tile grid out of the tile gameobject
    /// </summary>
    private void GenerateGrid() {

        emptyTileTemplate.SetActive(true);

        for (int x = 0; x < X_MAX; x++) {
            for (int z = 0; z < Z_MAX; z++) {

                GameObject newTile = Instantiate(emptyTileTemplate, new Vector3(x * TILE_WIDTH, 0, z * TILE_WIDTH), Quaternion.identity);

                // Store a reference to this tile into a "holder" object, tileFondler
                newTile.transform.parent = tileFondler.transform;

                // Also store our own reference to this tile in our own 2D array
                tileArray[x, z] = newTile;
            }
        }

        // @TODO: Randomly generate resources and obstacles,
        // and populate the tileArray here

        emptyTileTemplate.SetActive(false);
    }
}
