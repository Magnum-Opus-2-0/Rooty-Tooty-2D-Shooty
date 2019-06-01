using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionShootController : NPCShootController
{

    public int burstSize;

    public Collider rifle;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
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
    /// Defines the target priorities of minions
    /// <para>
    /// <list type="number">
    ///     <listheader>
    ///         <term>Target Priorities</term>
    ///         <description>*Highest to lowest*</description>
    ///     </listheader>
    ///     <item>
    ///         <term>Teddy</term>
    ///     </item>
    ///     <item>
    ///         <term>Soldiers</term>
    ///     </item>
    ///     <item>
    ///         <term>Building</term>
    ///     </item>
    ///     <item>
    ///         <term>Player</term>
    ///     </item>
    /// </list>
    /// </para>
    /// </summary>
    public override void DefinePriorities()
    {
        priorities.Add("P1_Teddy", 1);
        priorities.Add("P2_Teddy", 1);

        priorities.Add("P1_Soldier", 2);
        priorities.Add("P2_Soldier", 2);

        priorities.Add("P1_Base", 3);
        priorities.Add("P2_Base", 3);
        priorities.Add("P1_Spawner", 3);
        priorities.Add("P2_Spawner", 3);
        priorities.Add("P1_Turret", 3);
        priorities.Add("P2_Turret", 3);
        priorities.Add("P1_Healer", 3);
        priorities.Add("P2_Healer", 3);

        priorities.Add("Player1_obj", 4);
        priorities.Add("Player2_obj", 4);
        // Not sure if these last two are necessary, but it can't hurt to add them
        priorities.Add("Player1", 4);
        priorities.Add("Player2", 4);
    }

}
