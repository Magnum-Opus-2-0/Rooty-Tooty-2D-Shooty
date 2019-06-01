using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class MinionBuildingController : BuildingController
{
    private static Transform master_minion_fondler;
    private static bool findFondlerOnce = true;

    /// <summary>
    /// The bullet fondler that will be instantiated for each minion.
    /// </summary>
    public GameObject bulletFondler;

    /// <summary>
    /// The minion to be spawned.
    /// </summary>
    public GameObject minionToSpawn;

    public Transform minionFondler;
    public Transform MinionFondler
    {
        get
        {
            return minionFondler;
        }

        set
        {
            minionFondler = value;
            minionPool.Fondler = minionFondler;
        }
    }

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
    public int minionsPerBatch = 3;
    private int minionsThisBatch;

    private MinionObjectPooler<MinionController> minionPool;

    protected override void Awake()
    {
        base.Awake();

        if (findFondlerOnce)
        {
            master_minion_fondler = CreateBuilding.FindMasterMinionFondler();
            findFondlerOnce = false;
        }

        minionsThisBatch = 0;

        minionPool = new MinionObjectPooler<MinionController>(minionToSpawn, minionFondler, maxMinions, bulletFondler);
        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!health.isNotDead() && !diedOnce)
        {
            minionPool.Destroy();
            Destroy(minionFondler.gameObject);
        }

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

    /// <summary>
    /// A coroutine that spawns a batch of minions. <see cref="minionsPerBatch"/>
    /// Minions are spawned one at a time every <see cref="timeBetweenMinions"/>
    /// seconds. Minions will only be spawned as long as the health of the base
    /// is not zero.
    /// </summary>
    /// <returns>An IEnumerator so that this method can function as a Coroutine.</returns>
    private IEnumerator SpawnMinionBatch()
    {
        while (minionsThisBatch < minionsPerBatch && IsNotDead())
        {
            SpawnMinion();
            minionsThisBatch++;
            yield return new WaitForSeconds(timeBetweenMinions);
        }
        minionsThisBatch = 0;
    }

    /// <summary>
    /// A coroutine that calls the Minion batch coroutine every <see cref="timeBetweenBatches"/>
    /// seconds.
    /// </summary>
    /// <returns>The routine.</returns>
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenBatches);
            StartCoroutine(SpawnMinionBatch());
        }
    }

    /// <summary>
    /// Spawns a Minion at the first available spawnpoint if one is available from
    /// the object pool. Minions will not spawn if there are no open spawnpoints, or
    /// there are no available minions. I.e. the object pool request fails.
    /// </summary>
    public void SpawnMinion()
    {
        Transform spawnpoint = GetOpenSpawn();
        if (spawnpoint)
        {
            MinionController minion = minionPool.Request(spawnpoint);
            if (minion)
            {

            }
            else
            {
                //Debug.Log("Maximum allowed minions reached. Skipping Minion spawn.");
            }
        } 
        else
        {
            //Debug.Log("No open spawnpoints. Skipping Minion spawn.");
        }
    }

}
