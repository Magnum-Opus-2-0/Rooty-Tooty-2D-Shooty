using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HealsBuildingController : BuildingController
{
    /// <summary>
    /// Rate of healing, per second.
    /// E.g. if healRate = 2,
    /// this building will heal affected units for 2 damage per second.
    /// </summary>
    public int healRate;

    private TagManager taggyBoi;
    private HashSet<GameObject> healables;

    protected override void Awake()
    {
        base.Awake();
        taggyBoi = new TagManager();
        healables = new HashSet<GameObject>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();



        foreach (GameObject g in healables) {

            HealthBehavior hb = g.GetComponent<HealthBehavior>();
            Assert.IsTrue(hb != null, g.name + " does not have a HealthBehavior script attached");

            hb.adjustHealth(healRate);
        }
    }


    private void OnTriggerStay(Collider other)
    {
        // If collider is attached to player or minion or just about anything
        // with a HealthBehavior script then heal it. 
        // We just don't want this building healing other buildings.
        HealthBehavior hb = other.gameObject.GetComponent<HealthBehavior>();
        if (hb == null) return; 

    }

    
}
