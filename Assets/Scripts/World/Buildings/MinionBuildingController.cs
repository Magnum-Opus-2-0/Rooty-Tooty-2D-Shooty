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

    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }


}
