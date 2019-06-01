using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionObjectPooler<T> : RestrictedObjectPooler<T> where T : MinionController, IRecyclable
{
    private static Transform master_minion_fondler;

    private GameObject bulletFondler;
    public GameObject BulletFondler
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

    public MinionObjectPooler(GameObject prefab, Transform parent, int max, GameObject bulletFondler) 
        : base(prefab, parent, max)
    {
        if (!master_minion_fondler)
        {
            master_minion_fondler = CreateBuilding.FindMasterMinionFondler();
        }

        BulletFondler = bulletFondler;
    }

    public override T Request(Vector3 at, Quaternion dir)
    {
        T obj = base.Request(at, dir);
        // We want to see just instantiated a new object with the base.Request
        if ((GetNumInstantiated() - 1) < maxAllowed)
        {
            InstantiateBulletFondler(obj.gameObject, " [" + PrevID + "]");
        }

        return obj;
    }


    private void InstantiateBulletFondler(GameObject minion, string suffix)
    {
        GameObject temp = Object.Instantiate(bulletFondler, master_minion_fondler) as GameObject;
        minion.GetComponent<MinionShootController>().BulletFondler = temp.transform;
        temp.name += suffix;
    }
}
