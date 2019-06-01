using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierController : MinionController
{
    #region SHOOT_MEMBERS

    #endregion

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

    public IEnumerator PointAtThenShoot(GameObject target)
    {
        Vector3 targetDir = (target.transform.position - transform.position).normalized;

        #pragma warning disable CS0618
        // Vector3.AngleBetween is obsolete because it 
        // returns radians, but I want rads/sec for the
        // following Vector3.RotateTowards call.
        float turnSpeed = Vector3.AngleBetween(transform.forward, targetDir);
        #pragma warning restore CS0618

        // First point the minion at the target
        while (Vector3.Dot(transform.forward, targetDir) < 0.9999f) // threshold because chances are this won't be exact
        {
            transform.rotation = Quaternion.LookRotation(
                Vector3.RotateTowards(transform.forward, targetDir, turnSpeed * Time.deltaTime, 0.0f)
                );
            Debug.Log(name + ": Turning towards " + target.name);
            yield return null;
        }

        Debug.Log(name + ": Begin shooting");
        // Then shoot the target
        shooter.Shoot();
        // Now let's wait in this coroutine until for the Minion to "reload"
        // so that we don't just shoot bullets forever.
        yield return new WaitForSeconds(timeBetweenAttacks);
        // Finally let's let the minion move again.
        Debug.Log(name + ": Finished Attack");
        State = MinionStates.Move;
    }
    
    /// <summary>
    /// Shoots at the target we've acquired. 
    /// </summary>
    public override void Attack()
    {
        GameObject target = shooter.AcquireTarget();
        if (target)
        {
            Debug.Log(name + ": Attacking " + target);
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
}
