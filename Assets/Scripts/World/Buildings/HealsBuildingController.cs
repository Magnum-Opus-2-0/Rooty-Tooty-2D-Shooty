using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealsBuildingController : BuildingController
{
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        // If collider is attached to player or minion or just about anything
        // with a HealthBehavior script then heal it. 
        // We just don't want this building healing other buildings.
    }
}
