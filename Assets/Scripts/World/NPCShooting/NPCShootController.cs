﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCShootController : MonoBehaviour
{
    [SerializeField]
    protected Transform barrel;

    [SerializeField]
    protected GameObject bullet;
    [SerializeField]
    protected Transform bulletFondler;
    public Transform BulletFondler
    {
        get
        {
            return bulletFondler;
        }

        set
        {
            bulletFondler = value;
            bulletPool.Fondler = value;
        }
    }

    [SerializeField]
    protected float rateOfFire;
    public float RateOfFire
    {
        get
        {
            return rateOfFire;
        }
        set
        {
            rateOfFire = value;
        }
    }

    [SerializeField]
    protected float range;
    public float Range
    {
        get
        {
            return range;
        }
        set
        {
            range = value;
        }
    }

    [SerializeField]
    protected float bulletSpeed;
    public float BulletSpeed
    {
        get
        {
            return bulletSpeed;
        }
        set
        {
            bulletSpeed = value;
        }
    }

    [SerializeField]
    protected int maxBullets;
    public int MaxBullets
    {
        get
        {
            return maxBullets;
        }

        set
        {
            maxBullets = value;
        }
    }

    protected HealthBehavior health;
    protected HealthBehavior targetHealth;

    protected QueueObjectPooler<RecyclableBullet> bulletPool;

    #region TARGET_MEMBERS
    /// <summary>
    /// The possible targets in range of the shooter.
    /// This is mainly manipulated by the Acquire Targets methods and will
    /// rarely need to be accessed directly.
    /// </summary>
    protected List<GameObject> targets;
    public List<GameObject> Targets
    {
        get
        {
            return targets;
        }
    }

    /// <summary>
    /// A temporary list to hold the highest priority targets we find.
    /// </summary>
    protected List<GameObject> tempTargets;

    /// <summary>
    /// A dictionary representing the priority of each <see cref="TagManager">shootable</see>
    /// tag. <see cref="DefinePriorities"/> will fill the dictionary with
    /// key, value pairs.
    /// </summary>
    protected Dictionary<string, int> priorities;

    /// <summary>
    /// The layer of this shooter's targets.
    /// </summary>
    protected int targetLayerMask;
    protected int targetLayerIndex;
    #endregion


    protected virtual void Awake()
    {
        bulletPool = new QueueObjectPooler<RecyclableBullet>(bullet, bulletFondler, maxBullets);
        health = GetComponent<HealthBehavior>();
        targets = new List<GameObject>();
        tempTargets = new List<GameObject>();
        priorities = new Dictionary<string, int>();
        DefinePriorities();
        targetLayerMask = DetermineTargetLayer();
    }

    protected virtual void Update()
    {
        if (!health.isNotDead())
        {
            bulletPool.Destroy();
        }
    }

    public abstract void Shoot();

    /// <summary>
    /// Detects the targets in range. That is, gets all GameObjects with the
    /// specified layer in range. This method guarantees that no duplicate 
    /// GameObjects will be returned.
    /// </summary>
    /// <returns>A List of GameObjects representing the targets in range.</returns>
    public List<GameObject> DetectTargets()
    {
        // Make room for new targets
        targets.Clear();
        //Debug.Log(name + ": clearing targets");

        // Get all of the colliders in range
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, targetLayerMask);
        //Debug.Log(name + ": Found " + colliders.Length + " colliders");
        // Add the GameObjects of the colliders to the targets list
        foreach (Collider c in colliders)
        {
            GameObject target = FindParentObject(c.transform);
            //Debug.Log(name + ": Target found: " + target);
            // Make sure we don't add duplicate references to the same GameObject
            // Make sure we don't add dead GameObjects to the targets.
            if (!targets.Contains(target) && target.GetComponent<HealthBehavior>().isNotDead())
            {
                //Debug.Log(name + ": Target added: " + target);
                targets.Add(target);
            }
        }

        //Debug.Log(name + ": Total targets: " + targets.Count);
        return targets;
    }

    /// <summary>
    /// Acquires the closest, highest priority target from the <see cref="targets"/>
    /// list of GameObjects. Priority is determined by the implementing class. 
    /// This method assumes a full list of targets (filled by DetectTargets).
    /// </summary>
    /// <returns>The highest priority target or <c>null</c> if no GameObjects
    /// are in range.</returns>
    public GameObject AcquireTarget()
    {
        tempTargets.Clear();

        // First lets find out what our highest priority target is and create a
        // list of only those targets
        // We can't just remove GameObjects from the targets list because we're
        // enumerating over it
        string highestPriority = DetermineHighestPriority();
        foreach (GameObject g in targets)
        {
            if (IsValidTarget(g) && g.tag == highestPriority)
            {
                tempTargets.Add(g);
            }
        }

        // Next lets copy that temp list back in to our targets list so that
        // other methods can still make use of the targets list
        targets.Clear();
        foreach (GameObject g in tempTargets)
        {
            targets.Add(g);
        }

        // Finally return the closest GameObject in target
        return DetermineClosestTarget();
    }

    /// <summary>
    /// This method shall determine the priorities of all shootable GameObjects,
    /// where 1 is the highest priority.
    /// </summary>
    /// <returns>The tag of the highest priority.</returns>
    public abstract void DefinePriorities();

    /// <summary>
    /// Determines the highest priority GameObject in the <see cref="targets"/>
    /// list as defined by <see cref="priorities"/>.
    /// </summary>
    /// <returns>The tag of the highest priority GameObject(s) or null if there
    /// were no GameObjects in <see cref="targets"/>.</returns>
    public string DetermineHighestPriority()
    {
        string ret = null;
        int highest = int.MaxValue;
        int tempPriority = highest;

        foreach (GameObject g in targets)
        {
            // First make sure the GameObject we're looking at hasn't beem destroyed by something else
            // TryGetValue returns false if the string does not exist in the dictionary
            // so we only change highest if we actually found a matching key
            if (IsValidTarget(g))
            {
                priorities.TryGetValue(g.tag, out tempPriority);
            }
            // After we find a matching key we check to see if the priority was
            // higher than the previous
            if (tempPriority < highest)
            {
                highest = tempPriority;
                ret = g.tag;
            }
        }

        //Debug.Log(name + ": Highest priority is " + ret);
        return ret;
    }

    /// <summary>
    /// Determines the closest target from the <see cref="targets"/>
    /// list of GameObjects. Priority is determined by the implementing class. 
    /// This method assumes a full list of targets (filled by DetectTargets).
    /// </summary>
    /// <returns>The GameObject of the closest target, or <c>null</c> if there
    /// are no targets in the list.</returns>
    public GameObject DetermineClosestTarget()
    {
        GameObject closest = null;
        float minDist = float.MaxValue;
        float tempDist;

        //Debug.Log(name + ": DetermineClosestTarget count: " + targets.Count);

        foreach (GameObject g in targets)
        {
            tempDist = (g.transform.position - transform.position).magnitude;
            if (tempDist < minDist)
            {
                minDist = tempDist;
                closest = g;
            }
        }

        if (closest)
        {
            //Debug.Log(name + ": Closest Target: " + closest.name);
        }
        if(closest != null){
            targetHealth = closest.GetComponent<HealthBehavior>();
        }
        
        return closest;
    }

    /// <summary>
    /// Determines the target layer. By comparing its own layer and returning
    /// the other.
    /// <para>E.g. if this GameObject has the "Player_2_Target" layer then it returns 
    /// the "Player_1_Target" layer and vice versa.</para>
    /// </summary>
    /// <returns>The targets' layer, or -1 if this GameObject's layer is invalid.</returns>
    public int DetermineTargetLayer()
    {
        if (gameObject.layer == LayerMask.NameToLayer("P1 Target"))
        {
            return LayerMask.GetMask("P2 Target");
        } 

        if (gameObject.layer == LayerMask.NameToLayer("P2 Target"))
        {
            return LayerMask.GetMask("P1 Target");
        }

        return -1;
    }

    /// <summary>
    /// Finds the parent object of the specified collider. That is, this method
    /// finds the GameObject that has the health behavior script.
    /// 
    /// </summary>
    /// <returns>The parent object of the GameObject which has a HealthBehavior script,
    /// or null if no such parent exists.</returns>
    /// <param name="child">The transform of a child whose parents we would like
    /// to search.</param>
    public static GameObject FindParentObject(Transform child)
    {
        // If base case - that is, if we're the highest possible parent
        if (child.parent == null) {

            return (child.gameObject.GetComponent<HealthBehavior>() == null ? null : child.gameObject);
        }

        return FindParentObject(child.parent);
    }

    /// <summary>
    /// Determines whether the specified GameObject is valid. That is, whether
    /// it is not null and it is active.
    /// <para>This is important because it is possible for the shooter to be in
    /// the process of targeting/attacking and its target is killed.</para>
    /// </summary>
    /// <returns><c>true</c>, if valid target was ised, <c>false</c> otherwise.</returns>
    /// <param name="g">The green component.</param>
    public static bool IsValidTarget(GameObject g)
    {
        return g != null && g.activeInHierarchy;
    }
}
