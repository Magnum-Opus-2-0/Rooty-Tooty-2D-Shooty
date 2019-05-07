using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealsBuildingController : BuildingController
{

    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    private void OnTriggerStay(Collider other)
    {
        // If collider is attached to player or minion or just about anything
        // with a HealthBehavior script then heal it. 
        // We just don't want this building healing other buildings.
    }
}
