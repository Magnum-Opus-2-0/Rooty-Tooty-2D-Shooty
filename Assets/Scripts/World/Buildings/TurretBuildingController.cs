using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBuildingController : BuildingController
{
    static int id = 0;
    // This probably needs a reference to its target GameObject. Then in the
    // on trigger stay we can shoot at it. This is going to be the building
    // that requires the most work. :(

    protected override void Awake()
    {
        base.Awake(); 
        id++;
        name += " (" + id + ")";
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
