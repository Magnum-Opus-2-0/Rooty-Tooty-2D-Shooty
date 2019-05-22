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
    public string upDown;
    public string leftRight;
    private MinionShootController shooter;

    public NavMeshAgent nv_agent;
    private static GameObject P1_Base;
    private static GameObject P2_Base;

    private Transform homeBase;
    private Transform enemyBase;
    public bool doNavMeshDemo;
    public bool useMouseControls;



    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<HealthBehavior>();
        health.setHealth(maxHealth);

        shooter = GetComponent<MinionShootController>();

        leftRight = getAxisString(true);
        upDown = getAxisString(false);


        // Set base information
        if (P1_Base == null)
            P1_Base = GameObject.Find("P1_Base");
        if (P2_Base == null)
            P2_Base = GameObject.Find("P2_Base");

        Assert.IsTrue(tag.Equals("P1_Minion") || tag.Equals("P2_Minion"),
            "Minion tag " + tag.ToString() + " improperly set");

        homeBase  = (tag.Equals("P1_Minion") ? P1_Base.transform : P2_Base.transform);
        enemyBase = (tag.Equals("P1_Minion") ? P2_Base.transform : P1_Base.transform);



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

            // NavMeshDemo with mouse controls
            if (useMouseControls) {

                #region Mouse Controls
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
                #endregion
            }

            // NavMeshDemo with pad controls
            else {

                //Debug.Log(Input.GetAxis(leftRight));

                #region Pad Controls
                // If left, defend
                if (Input.GetAxisRaw(leftRight) < 0) {

                    SetDefend();
                }

                // Else if right, attack
                else if (Input.GetAxisRaw(leftRight) > 0) {

                    SetAttack();
                }

                // Else if down, halt
                else if (Input.GetAxisRaw(upDown) < 0) {

                    SetHalt();
                }
                #endregion
            }

        } else {
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
        
        if (nv_agent.isActiveAndEnabled && !nv_agent.pathPending)
            Assert.IsTrue(nv_agent.SetDestination(dest),
                "NavMeshAgent failed to set destination to enemy base.\n"
                + "Target destination: " + dest.ToString());
    }

    public void SetDefend() {

        Assert.IsTrue(nv_agent != null, "Somehow, nv_agent hasn't been initialized by this line\n"
            + "This minion: " + this.ToString());
        nv_agent.isStopped = false;

        Vector3 dest = homeBase.position;
        //Vector3 dest = homeBase.localPosition;

        if (!nv_agent.pathPending)
            Assert.IsTrue(nv_agent.SetDestination(dest),
                "NavMeshAgent failed to set destination to home base.\n"
                + "Target destination: " + dest.ToString());
    }

    public void SetHalt() {

        nv_agent.isStopped = true;
    }

    private string getAxisString(bool leftRight) {

        switch (Application.platform) {
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:

                if (leftRight)
                    return (tag == "P1_Minion" ? "dpad-1-leftright-mac" : "dpad-2-leftright-mac");
                else
                    return (tag == "P1_Minion" ? "dpad-1-updown-mac" : "dpad-2-updown-mac");

            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:

                if (leftRight)
                    return (tag == "P1_Minion" ? "dpad-1-leftright-win" : "dpad-2-leftright-win");
                else
                    return (tag == "P1_Minion" ? "dpad-1-updown-win" : "dpad-2-updown-win");

            default:
                Debug.LogError("Mappings not setup for operating systems other than Windows or Mac OS");
                return "";
        }

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
