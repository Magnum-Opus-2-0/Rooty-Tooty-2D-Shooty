using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeddyController : MinionController
{
    public int attackDamage;
    public ParticleSystem smashEffect;

    private bool dieOnce;

    private const float DEATH_ANIM_LEN = 1.5f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        dieOnce = true;
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
        smashEffect.Play();
        // yield return new WaitForSeconds(# of seconds until the animation hits the ground)
        foreach (GameObject g in shooter.Targets)
        {
            g.GetComponent<HealthBehavior>().adjustHealth(-attackDamage);
        }
        yield return new WaitForSeconds(timeBetweenAttacks);
        State = MinionStates.Move;
        // We clear here so that the teddy gets a chance to search for new ones.
        shooter.Targets.Clear();
    }

    /// <summary>
    /// Performs the functionality when the Teddy dies. Overrides the base to
    /// include time for the death animation to play.
    /// </summary>
    public override void Die()
    {
        if (dieOnce)
        {
            anim.SetTrigger("Die");
            if (transform.parent)
            {
                StartCoroutine(WaitDisable());
            }
            else
            {
                Destroy(gameObject, DEATH_ANIM_LEN);
            }
            dieOnce = false;
        }
    }

    /// <summary>
    /// Recycle this instance. Overrides the base to reset <see cref="dieOnce"/>.
    /// </summary>
    public override void Recycle()
    {
        base.Recycle();
        dieOnce = true;
        anim.SetTrigger("Recycle");
    }

    /// <summary>
    /// A coroutine that waits the <see cref="DEATH_ANIM_LEN"/> seconds before
    /// setting the Teddy inactive.
    /// </summary>
    /// <returns>The disable.</returns>
    private IEnumerator WaitDisable()
    {
        yield return new WaitForSeconds(DEATH_ANIM_LEN);
        gameObject.SetActive(false);
    }

    public void StartSinking()
    {

    }
}
