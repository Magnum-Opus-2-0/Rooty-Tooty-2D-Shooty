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

    protected QueueObjectPooler<RecyclableBullet> bulletPool;

    protected virtual void Start()
    {
        bulletPool = new QueueObjectPooler<RecyclableBullet>(bullet, bulletFondler, maxBullets);
    }

    public abstract void Shoot();
}
