using System.Collections;
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

    private HealthBehavior health;

    protected QueueObjectPooler<RecyclableBullet> bulletPool;

    /// <summary>
    /// The possible targets in range of the shooter.
    /// This is mainly manipulated by the Acquire Targets methods and will
    /// rarely need to be accessed directly.
    /// </summary>
    protected List<GameObject> targets;


    protected virtual void Start()
    {
        bulletPool = new QueueObjectPooler<RecyclableBullet>(bullet, bulletFondler, maxBullets);
        health = GetComponent<HealthBehavior>();
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
    /// Acquires the targets. That is, gets all GameObjects with the specified 
    /// tags in range. This method guarantees that no duplicate GameObjects will
    /// be returned.
    /// </summary>
    /// <returns>A List of GameObjects representing the targets in range.</returns>
    /// <param name="layer">The layer of the targets to be acquired.</param>
    protected List<GameObject> AcquireTargets(int layer)
    {
        // Make room for new targets
        targets.Clear();

        // Get all of the colliders in range
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, layer);

        // Add the GameObjects of the colliders to the targets list
        foreach (Collider c in colliders)
        {
            GameObject target = c.transform.root.gameObject;
            // Make sure we don't add duplicate references to the same GameObject
            if (!targets.Contains(target))
            {
                targets.Add(target);
            }
        }

        return targets;
    }

    /// <summary>
    /// Acquires the closest, highest priority target from the targets list of
    /// GameObjects. Priority is determined by the implementing class.
    /// </summary>
    /// <returns>The highest priority target.</returns>
    public abstract GameObject AcquireTarget();


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
            return LayerMask.NameToLayer("P2 Target");
        } 

        if (gameObject.layer == LayerMask.NameToLayer("P2 Target"))
        {
            return LayerMask.NameToLayer("P1 Target");
        }

        return -1;
    }
}
