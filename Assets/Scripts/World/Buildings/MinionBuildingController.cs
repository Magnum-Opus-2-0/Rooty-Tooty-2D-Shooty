using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionBuildingController : BuildingController
{
    // I'm thinking we'll have a List of GameObjects each with a trigger on them
    // used as spawnpoints. Then we can either randomly assign a spawnpoint to
    // each minion or use the same one and have backups in case something is in
    // the way.

    // This should be good to use on both the big and regular minion spawner. We
    // just have to have a reference to the type of minion we want to spawn.

    /// <summary>
    /// The minion to be spawned.
    /// </summary>
    public GameObject minionToSpawn;
    
    /// <summary>
    /// An array of <see cref="SpawnpointBehavior"/> scripts in the order of
    /// spawn priority with element 0 being highest priority.
    /// </summary>
    public SpawnpointBehavior[] spawns;

    public float timeBetweenBatches = 7.5f;
    public float timeBetweenMinions = 1f;

    /// <summary>
    /// The maximum number of minions that can be spawned at one time.
    /// </summary>
    public int maxMinions = 10;
    private int curMinions;

    protected override void Awake()
    {
        base.Awake();
        curMinions = 0;

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Gets the <see cref="Transform"/> of the first open spawnpoint starting 
    /// with the highest priority and working its way to the lowest.
    /// <para>The Transform of the spawnpoint is returned and not its position
    /// because a Vector3 is a struct and therefore cannot be <c>null</c>.</para>
    /// </summary>
    /// <returns>The Transform of the highest priority open spawn, or <c>null</c>
    /// if no spawnpoints are open.</returns>
    public Transform GetOpenSpawn()
    {
        for (int i = 0; i < spawns.Length; i++)
        {
            if (spawns[i].IsOpen)
            {
                return spawns[i].transform;
            }
        }

        return null;
    }

    private IEnumerator SpawnMinionBatch()
    {
        SpawnMinion();
        yield return new WaitForSeconds(timeBetweenMinions);
    }

    public void SpawnMinion()
    {
        //GameObject nextMinion = object pooling code to determine which minion should be spawned
        //Instantiate(nextMinion, GetOpenSpawn().position, Quaternion.identity, transform);
        // or
        //curMinion.SetActive = true ?
    }
}
