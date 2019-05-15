using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionBuildingController : BuildingController
{
    /// <summary>
    /// The minion to be spawned.
    /// </summary>
    public GameObject minionToSpawn;
    public Transform minionFondler;
    
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
    public int minonsPerBatch = 3;
    private int minionsThisBatch;

    private RestrictedObjectPooler<MinionController> minionPool;

    protected override void Awake()
    {
        base.Awake();
        minionsThisBatch = 0;

        minionPool = new RestrictedObjectPooler<MinionController>(minionToSpawn, minionFondler, maxMinions);
        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!health.isNotDead())
        {
            minionFondler.DetachChildren();
        }
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
        while (minionsThisBatch < minonsPerBatch && IsNotDead())
        {
            SpawnMinion();
            minionsThisBatch++;
            yield return new WaitForSeconds(timeBetweenMinions);
        }
        minionsThisBatch = 0;
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenBatches);
            StartCoroutine(SpawnMinionBatch());
        }
    }

    public void SpawnMinion()
    {
        Transform spawnpoint = GetOpenSpawn();
        if (spawnpoint != null)
        {
            if (!minionPool.Request(spawnpoint))
            {
                Debug.Log("Maximum allowed minions reached. Skipping Minion spawn.");
            }
        } 
        else
        {
            Debug.Log("No open spawnpoints. Skipping Minion spawn.");
        }
    }
}
