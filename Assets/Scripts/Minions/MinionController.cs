using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class MinionController : MonoBehaviour, IRecyclable
{
    public int maxHealth;
    private HealthBehavior health;

    public float reloadTime;
    private MinionShootController shooter;

    public NavMeshAgent nv_agent;
    public Transform homeBase;
    public Transform enemyBase;
    public bool doNavMeshDemo;

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



        if (doNavMeshDemo) {

            // Both mouse buttons = stay
            if (Input.GetKey(KeyCode.Mouse0) && Input.GetKey(KeyCode.Mouse1)) {

                SetHalt();
            }

            // Right mouse button = attack
            else if (Input.GetKeyDown(KeyCode.Mouse1)) {

                SetAttack();
            }

            // Left mouse button = defend
            else if (Input.GetKeyDown(KeyCode.Mouse0)) {

                SetDefend();
            }
        }
        
        
        
        else {
            DebugMovement();
        }
    }

    /// <summary>
    /// Performs any necessary routines before murdering this minion.
    /// </summary>
    public void Die()
    {
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

    public void SetAttack() {

        nv_agent.isStopped = false;

        Vector3 dest = enemyBase.position;
        //Vector3 dest = enemyBase.localPosition;

        Assert.IsTrue(nv_agent.SetDestination(dest),
            "NavMeshAgent failed to set destination to enemy base.\n"
            + "Target destination: " + dest.ToString());
    }

    public void SetDefend() {

        nv_agent.isStopped = false;

        Vector3 dest = homeBase.position;
        //Vector3 dest = homeBase.localPosition;

        Assert.IsTrue(nv_agent.SetDestination(dest),
            "NavMeshAgent failed to set destination to home base.\n"
            + "Target destination: " + dest.ToString());
    }

    public void SetHalt() {

        nv_agent.isStopped = true;
    }

    #region DEBUG_METHODS
    private void DebugMovement()
    {
        transform.Translate(0, 0, 5.0f * Time.deltaTime);
    }

    private void DebugAutoMovement(Vector3 dest) {


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
