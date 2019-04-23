using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
     /// <summary>
     /// default game tile used for copying and instantiating new tiles
     /// </summary>
     public GameObject tile;
     /// <summary>
     /// Not currently used, but a 2D array that holds all the tile info.
     /// Eventually, we'd probably like to associate certain attributes to
     /// specific tiles, so this is a way to do so.
     /// </summary>
     //public GameObject[,] tileArray;

     /// <summary>
     /// How many tiles there are in the z direction.
     /// </summary>
     private readonly int Z_MAX = 50;
     /// <summary>
     /// How many tiles there are in the x direction.
     /// </summary>
     private readonly int X_MAX = 50;

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
     private void GenerateGrid()
     {
          tile.SetActive(true);

          for (int x = 0; x < X_MAX; x++)
          {
               for (int z = 0; z < Z_MAX; z++)
               {
                    var newTile = Instantiate(tile, new Vector3(x * 10, 0, z * 10), Quaternion.identity);
                    newTile.transform.parent = gameObject.transform;
               }
          }

          tile.SetActive(false);
     }
}
