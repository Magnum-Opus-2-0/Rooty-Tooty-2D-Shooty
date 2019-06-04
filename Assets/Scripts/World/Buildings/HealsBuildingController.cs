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
    public float healRadius;

    private static TagManager taggyBoi = new TagManager();
    private HashSet<GameObject> healables;

    private float t;

    protected override void Awake()
    {
        base.Awake();
        healables = new HashSet<GameObject>();

        t = 0.0f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // Update time
        t += Time.deltaTime;

        // Every frame, keep track of healables
        populateHealables();

        // This checks t; if t > 1.0f, it applies healing
        healHealables();

        // clean up for next frame
        t %= 1.0f;
        healables.Clear();
    }

    // If collider is attached to player or minion or just about anything
    // with a HealthBehavior script then heal it. 
    // We just don't want this building healing other buildings.
    //private void OnTriggerStay(Collider other)
    //{

    //    HealthBehavior hb = other.gameObject.GetComponent<HealthBehavior>();
    //    if (hb == null) return; 

    //}

    /// <summary>
    /// Adds a reference to other's GameObject
    /// if it is eligible for healing.
    /// GameObjects eligible are any objects which have HealthBehaviors,
    /// but are not buildings.
    /// </summary>
    /// <param name="other"></param>
    //protected override void OnTriggerEnter(Collider other) {

    //    GameObject go = other.gameObject;

    //    if (   ( taggyBoi.isP1Tag(this.tag) && taggyBoi.isP1Tag(go.tag))
    //        || ( taggyBoi.isP2Tag(this.tag) && taggyBoi.isP2Tag(go.tag))) {

    //        // TODO: exclude buildings

    //        healables.Add(go);
    //    }
    //}

    /// <summary>
    /// Since other's GameObject is exiting the healing zone,
    /// it is no longer eligible for healing.
    /// If it was previously eligible,
    /// it will be removed
    /// </summary>
    /// <param name="other"></param>
    //private void OnTriggerExit(Collider other) {

    //    GameObject go = other.gameObject;

    //    if (healables.Contains(go)) {

    //        healables.Remove(go);
    //    }
    //}

    /// <summary>
    /// Populates this script's internal healables collection.
    /// Checks colliders in a radius around this object.
    /// If they are eligible to be healed,
    /// adds their GameObject to the healables collection.
    /// Note that this healables collection should be flushed at the end of every frame,
    /// because this function will be called next frame.
    /// </summary>
    private void populateHealables() {

        Collider[] overlaps = Physics.OverlapSphere(gameObject.transform.position, healRadius);

        foreach (Collider c in overlaps) {

            GameObject go = NPCShootController.FindParentObject(c.gameObject.transform);
            HealthBehavior hb = (go == null ? null : go.GetComponent<HealthBehavior>());

            // Exclude things which don't have HealthBehaviors
            if (hb == null) continue;

            // Exclude buildings (else, healing buildings would be pretty broken)
            if (taggyBoi.isStructure(go.tag)) continue;

            // Don't heal if they're dead
            if (!hb.isNotDead()) continue;

            // Only heal if us and go are of the same team
            if ((taggyBoi.isP1Tag(this.gameObject.tag) && taggyBoi.isP1Tag(go.tag))
                || (taggyBoi.isP2Tag(this.gameObject.tag) && taggyBoi.isP2Tag(go.tag))) {

                healables.Add(go);
            } 
        }
    }

    /// <summary>
    /// Iterates through healables and heals them all based on healRate,
    /// only when t > 1.0f.
    /// If t is less than 1.0f,
    /// this function has no effect.
    /// </summary>
    private void healHealables() {

        if (t < 1.0f) return;

        foreach (GameObject g in healables) {

            HealthBehavior hb = g.GetComponent<HealthBehavior>();

            // hb better be valid before we heal it!
            Assert.IsTrue(hb != null, g.name + " does not have a HealthBehavior script attached");
            Assert.IsTrue(hb.isActiveAndEnabled, g.name + "'s HealthBehavior either is inactive or disabled");

            hb.adjustHealth(healRate);
        }
    }
}
