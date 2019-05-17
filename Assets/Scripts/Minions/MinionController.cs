using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour, IRecyclable
{
    public int maxHealth;
    private HealthBehavior health;

    public float reloadTime;
    private MinionShootController shooter;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<HealthBehavior>();
        health.setHealth(maxHealth);

        shooter = GetComponent<MinionShootController>();

        StartCoroutine(DebugShoot());
    }

    // Update is called once per frame
    void Update()
    {
        if (!health.isNotDead())
        {
            Die();
        }

        DebugMovement();
    }

    /// <summary>
    /// Performs any necessary routines before murdering this minion.
    /// </summary>
    public void Die()
    {
        Debug.Log("Minion dying");
        // Maybe a happy little death animation would be fun here

        // If the Minion is not a child of its fondler then it is an orphan.
        // Orphaned Minions must be destroyed.
        // What I'm saying is orphaned Minions have had their spawner destroyed
        // and therefore to not respawn.
        if (transform.parent)
        {
            gameObject.SetActive(false);
        } 
        else
        {
            Destroy(gameObject);
        }
    }

    public void Recycle()
    {
        health.setHealth(maxHealth);
    }

    #region DEBUG_METHODS
    private void DebugMovement()
    {
        transform.Translate(0, 0, 5.0f * Time.deltaTime);
    }

    private IEnumerator DebugShoot()
    {
        while (health.isNotDead())
        {
            yield return new WaitForSeconds(reloadTime);
            shooter.Shoot();
        }
    }
    #endregion
}
