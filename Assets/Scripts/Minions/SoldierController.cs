using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierController : MinionController
{
    public int dieTime;
    #region SHOOT_MEMBERS

    #endregion

    private int turnIters;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        turnIters = 0;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public IEnumerator PointAtThenShoot(GameObject target)
    {
        Vector3 targetDir = (target.transform.position - transform.position).normalized;
        //Debug.Log(name + "Forward: " + transform.forward + ", Target Dir: " + targetDir);

        #pragma warning disable CS0618
        // Vector3.AngleBetween is obsolete because it 
        // returns radians, but I want rads/sec for the
        // following Vector3.RotateTowards call.
        float turnSpeed = Vector3.AngleBetween(transform.forward, targetDir);
        #pragma warning restore CS0618


        // First point the minion at the target
        while (Vector3.Dot(transform.forward, targetDir) < .9999f) // threshold because chances are this won't be exact
        {
            transform.rotation = Quaternion.LookRotation(
                Vector3.RotateTowards(transform.forward, targetDir, turnSpeed * Time.deltaTime, 0.0f),
                transform.up
                );

            // Sometimes minions get stuck trying to turn this is my filthy attempt at fixing it
            // I'm so ashamed it made me figuratively sick to my stomach.
            if (turnIters++ > 200) 
            {
                break;
            }
            //Debug.Log(name + ": Turning towards: " + target.name + " (iterations turning = " + turnIters + ")");
            yield return null;
        }

        //Debug.Log(name + ": Begin shooting");
        // Then shoot the target
        shooter.Shoot();
        // Now let's wait in this coroutine until for the Minion to "reload"
        // so that we don't just shoot bullets forever.
        yield return new WaitForSeconds(timeBetweenAttacks);
        // Finally let's let the minion move again.
        //Debug.Log(name + ": Finished Attack");
        State = MinionStates.Move;
        // Make sure we clear the target list so that we actually have to search
        // again. If we don't do this, then we immediately start attacking again
        // because we don't have enough time to detect targets (which calls clear)
        shooter.Targets.Clear();
        turnIters = 0;
    }
    
    /// <summary>
    /// Shoots at the target we've acquired. 
    /// </summary>
    public override void Attack()
    {
        GameObject target = shooter.AcquireTarget();
        if (target)
        {
            //Debug.Log(name + ": Attacking " + target);
            // We know this function is called only after CanAttack() has been
            // called, so we can assume that our target list is full and just
            // acquire a target.
            StartCoroutine(PointAtThenShoot(target));
        }
        else
        {
            State = MinionStates.Move;
        }
    }

    public override void Die()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        StartCoroutine(WaitUp(dieTime));
    }

    IEnumerator WaitUp(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        base.Die();
    }
}
