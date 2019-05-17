using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionShootController : NPCShootController
{

    public int burstSize;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Shoots a <see cref="burstSize"/>ed bullet burst in the direction of the
    /// barrel's forward vector. 
    /// </summary>
    public override void Shoot()
    {
        StartCoroutine(Burst());
    }

    private IEnumerator Burst()
    {
        for (int i = 0; i < burstSize; i++)
        {
            //Rigidbody rb = bulletPool.Request(barrel).gameObject.GetComponent<Rigidbody>();
            RecyclableBullet b = bulletPool.Request(barrel);
            GameObject go = b.gameObject;
            Rigidbody rb = go.GetComponent<Rigidbody>();
            rb.AddForce(bulletSpeed * barrel.parent.forward, ForceMode.VelocityChange);
            yield return new WaitForSeconds(rateOfFire);
        }
    }

}
