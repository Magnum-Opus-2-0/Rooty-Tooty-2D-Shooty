using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecyclableBullet : BulletController, IRecyclable
{
    public void Recycle()
    {
        trail.Clear();
    }

    protected override void ResetBullet()
    {
        if (transform.parent)
        {
            base.ResetBullet();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
