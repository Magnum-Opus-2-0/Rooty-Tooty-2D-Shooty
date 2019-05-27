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
        List<GameObject> inRange;
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, layer);

        inRange = new List<GameObject>();

        foreach (Collider c in colliders)
        {
            GameObject target = c.transform.root.gameObject;
            // Make sure we don't add duplicate references to the same GameObject
            if (!inRange.Contains(target))
            {
                inRange.Add(target);
            }
        }

        return inRange;
    }

    /// <summary>
    /// Acquires the highest priority target from a list of GameObjects.
    /// Priority is determined by the implementing class.
    /// </summary>
    /// <returns>The highest priority target.</returns>
    /// <param name="targets">A list of GameObjects from which to acquire the
    /// target.</param>
    public abstract GameObject AcquireTarget(List<GameObject> targets);

    /// <summary>
    /// Determines the target layer. By comparing its own layer and returning
    /// the other.
    /// <para>E.g. if this GameObject has the "Player_2_Target" layer then it returns 
    /// the "Player_1_Target" layer and vice versa.</para>
    /// </summary>
    /// <returns>The targets' layer, or -1 if this GameObject's layer is invalid.</returns>
    public int DetermineTargetLayer()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Player_1_Target"))
        {
            return LayerMask.NameToLayer("Player_2_Target");
        } 

        if (gameObject.layer == LayerMask.NameToLayer("Player_2_Target"))
        {
            return LayerMask.NameToLayer("Player_1_Target");
        }

        return -1;
    }
}
