using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeddyController : MinionController
{
    public int attackDamage;
    //public Animation attackAnim;

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

    public override void Attack()
    {
        StartCoroutine(TeddySmash());
    }

    private IEnumerator TeddySmash()
    {
        // Play an attack animation here
        // yield return new WaitForSeconds(# of seconds until the animation hits the ground)
        foreach (GameObject g in shooter.Targets)
        {
            g.GetComponent<HealthBehavior>().adjustHealth(-attackDamage);
        }
        yield return new WaitForSeconds(timeBetweenAttacks);
        State = MinionStates.Move;
    }
}
