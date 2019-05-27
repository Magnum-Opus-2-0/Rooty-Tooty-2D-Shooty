using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionShootController : NPCShootController
{

    public int burstSize;

    public Collider rifle;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Shoots a <see cref="burstSize"/>ed bullet burst in the direction of the
    /// barrel's forward vector. 
    /// </summary>
    public override void Shoot()
    {
        StartCoroutine(Burst());
    }

    /// <summary>
    /// The Coroutine that makes Minions fire in bursts. The burst size and speed
    /// is determined by <see cref="burstSize"/> and <see cref="NPCShootController.rateOfFire"/>
    /// respectively.
    /// </summary>
    /// <returns>The IEnumerator necessary for .</returns>
    private IEnumerator Burst()
    {
        for (int i = 0; i < burstSize; i++)
        {
            Rigidbody rb = bulletPool.Request(barrel).gameObject.GetComponent<Rigidbody>();
            rb.AddForce(bulletSpeed * barrel.parent.forward, ForceMode.VelocityChange);
            yield return new WaitForSeconds(rateOfFire);
        }
    }

    /// <summary>
    /// Acquires the highest priority target.
    /// <para>Priority (in descending order): Player, Other Minions, Buildings</para>
    /// </summary>
    /// <returns>The target.</returns>
    /// <param name="targets">Targets.</param>
    public override GameObject AcquireTarget(List<GameObject> targets)
    {

        return null;
    }

}
