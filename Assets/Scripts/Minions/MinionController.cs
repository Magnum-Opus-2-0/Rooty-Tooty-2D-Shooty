using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public abstract class MinionController : MonoBehaviour, IRecyclable
{
    private static TagManager taggyboi = new TagManager();

    [SerializeField]
    public int maxHealth;
    protected HealthBehavior health;

    #region INPUT_MEMBERS
    [SerializeField]
    public string upDown;
    [SerializeField]
    public string leftRight;
    [SerializeField]
    public string altHalt;
    [SerializeField]
    public bool useAltHalt = true;
    #endregion

    public NavMeshAgent nv_agent;
    private static GameObject P1_Base;
    private static GameObject P2_Base;

    private Transform homeBase;
    private Transform enemyBase;
    public bool doNavMeshDemo;
    public bool useMouseControls;

    protected MinionShootController shooter;
    [SerializeField]
    public float timeBetweenAttacks = 3f;

    /// <summary>
    /// The time we wait between target searches. It's probably a bad idea to be
    /// calling <see cref="Physics.OverlapSphere"/> every frame.
    /// </summary>
    protected const float TIME_BETWEEN_SEARCHES = .25f;

    #region STATE_MEMBERS
    private MinionStates state;
    public MinionStates State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
            switch (value)
            {
                case MinionStates.Attack:
                    SetHalt();
                    break;

                case MinionStates.Move:
                    DetermineDestination();
                    break;

                case MinionStates.Dead:
                    SetHalt();
                    break;
            }
        }
    }
    #endregion

    #region MOVE_STATE_MEMBERS
    protected MinionMoveTypes moveType;
    public MinionMoveTypes MoveType
    {
        get
        {
            return moveType;
        }
    }

    protected MinionMoveTypes lastType;
    #endregion

    #region ANIMATION_MEMBERS

    protected Animator anim;

    #endregion



    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = GetComponent<HealthBehavior>();
        health.setHealth(maxHealth);

        shooter = GetComponent<MinionShootController>();

        anim = GetComponent<Animator>();

        leftRight = getAxisString(true);
        upDown = getAxisString(false);
        altHalt = getAltHalt();


        // Set base information
        if (P1_Base == null)
        {
            P1_Base = GameObject.FindWithTag("P1_Base");
        }
            
        if (P2_Base == null)
        {
            P2_Base = GameObject.FindWithTag("P2_Base");
        }

        Assert.IsTrue(taggyboi.isMinion(tag),
            "Minion tag " + tag + " improperly set");

        homeBase  = (taggyboi.isP1Tag(tag) ? P1_Base.transform : P2_Base.transform);
        enemyBase = (taggyboi.isP1Tag(tag) ? P2_Base.transform : P1_Base.transform);


        // Start them off by standing still until they get an order.
        moveType = lastType = MinionMoveTypes.Halt;
        // Both Soldiers and Teddies are allowed to start moving immediately
        State = MinionStates.Move;

        StartCoroutine(TargetSearch());

        if (debugOnce)
        {
            //StartCoroutine(PrintStates());
            //StartCoroutine(PrintDpad());
            debugOnce = false;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {


        if (!health.isNotDead())
        {
            Die();
        }

        if (CanAttack())
        {
            State = MinionStates.Attack;
            Attack();
        }

        if (doNavMeshDemo) {

            // NavMeshDemo with mouse controls
            if (useMouseControls) {

                #region Mouse Controls
                // Both mouse buttons = stay
                if (Input.GetKey(KeyCode.Mouse0) && Input.GetKey(KeyCode.Mouse1)) {
                    moveType = MinionMoveTypes.Halt;
                }

            // Right mouse button = attack
            else if (Input.GetKeyDown(KeyCode.Mouse1)) {
                    moveType = MinionMoveTypes.Attack;
                }

            // Left mouse button = defend
            else if (Input.GetKeyDown(KeyCode.Mouse0)) {
                    moveType = MinionMoveTypes.Defend;
                }
                #endregion
            }

            // NavMeshDemo with pad controls
            else {
                #region Pad Controls
                // If down, defend
                if (Input.GetAxisRaw(upDown) < 0) {
                    //Debug.Log("Attempting to change move type to DEFEND");
                    moveType = MinionMoveTypes.Defend;
                }

                // Else if up, attack
                else if (Input.GetAxisRaw(upDown) > 0) {
                    //Debug.Log("Attempting to change move type to ATTACK");
                    moveType = MinionMoveTypes.Attack;
                }

                // Else if right or altHalt button, halt
                else if ((useAltHalt && Input.GetButtonDown(altHalt)) || (!useAltHalt && Input.GetAxisRaw(leftRight) < 0)) {
                    //Debug.Log("Attempting to change move type to HALT");
                    moveType = MinionMoveTypes.Halt;
                }



                #endregion
            }

        } else {
            //DebugMovement();
        }

        UpdateDestination();
        if (anim)
        {
            UpdateAnimator();
        }
    }

    /// <summary>
    /// Performs any necessary routines before murdering this minion.
    /// </summary>
    public virtual void Die()
    {
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

    public virtual void Recycle()
    {
        health.setHealth(maxHealth);
        State = MinionStates.Move;
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
                    return (taggyboi.isP1Tag(tag) ? "dpad-1-leftright-mac" : "dpad-2-leftright-mac");
                else
                    return (taggyboi.isP1Tag(tag) ? "dpad-1-updown-mac" : "dpad-2-updown-mac");

            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:

                if (leftRight)
                    return (taggyboi.isP1Tag(tag) ? "dpad-1-leftright-win" : "dpad-2-leftright-win");
                else
                    return (taggyboi.isP1Tag(tag) ? "dpad-1-updown-win" : "dpad-2-updown-win");

            default:
                Debug.LogError("Mappings not setup for operating systems other than Windows or Mac OS");
                return "";
        }

    }

    private string getAltHalt()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                return (taggyboi.isP1Tag(tag) ? "minion-halt-1-mac" : "minion-halt-2-mac");

            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                return (taggyboi.isP1Tag(tag) ? "minion-halt-1-win" : "minion-halt-2-win");

            default:
                Debug.LogError("Mappings not setup for operating systems other than Windows or Mac OS");
                return "";
        }
    }

    /// <summary>
    /// Updates the destination if it has changed since the last time.
    /// </summary>
    private void UpdateDestination()
    {
        if (lastType != moveType)
        {
            DetermineDestination();
        }
        lastType = moveType;
    }

    /// <summary>
    /// Calls the Nav Mesh method depending on the Minion's move type.
    /// </summary>
    private void DetermineDestination()
    {
        switch (moveType)
        {
            case MinionMoveTypes.Attack:
                SetAttack();
                break;

            case MinionMoveTypes.Defend:
                SetDefend();
                break;

            case MinionMoveTypes.Halt:
                SetHalt();
                break;

            default:
                Debug.LogError("Oof something went horribly, horribly " +
                    "wrong. The MinionMoveType is set to an enum that has " +
                    "not been defined.");
                break;
        }
    }

    #region ATTACK_METHODS
    /// <summary>
    /// Determines whether a Minion can attack. Minions can attack if they are
    /// moving, and have encountered a target.
    /// </summary>
    /// <returns><c>true</c>, if the Minion can attack, <c>false</c> otherwise.</returns>
    public bool CanAttack()
    {
        if (state != MinionStates.Move)
        {
            return false;
        }

        if (shooter.Targets.Count == 0)
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// This method shall perform the actual attack. It is the implementing class'
    /// responsibility to set the state back to <see cref="MinionStates.Move"/>
    /// after the attack has finished.
    /// </summary>
    public abstract void Attack();

    public IEnumerator TargetSearch()
    {
        while (state != MinionStates.Dead)
        {
            yield return new WaitForSeconds(TIME_BETWEEN_SEARCHES);
            // If we're already in the process of attacking then don't look for
            // new targets
            while (state == MinionStates.Attack)
            {
                yield return null;
            }

            //Otherwise yeah go ahead bro
            //Debug.Log(name + ": Searching for targets...");
            shooter.DetectTargets();
            //Debug.Log(name + ": Targets found: " + shooter.Targets.Count);
        }
    }
    #endregion


    #region DEBUG_METHODS
    private bool debugOnce = true;

    private void DebugMovement()
    {
        transform.Translate(0, 0, 5.0f * Time.deltaTime);
    }

    private void DebugAutoMovement(Vector3 dest) {


    }

    private IEnumerator PrintStates()
    {
        while (true)
        {
            Debug.Log(name + ": Current (State, Dest): (" + state + ", " + moveType + ")");
            yield return new WaitForSeconds(.25f);
        }
    }

    private IEnumerator PrintDpad()
    {
        while (true)
        {
            Debug.Log("Dpad (LR, UD): (" + Input.GetAxisRaw(leftRight) + ", " + Input.GetAxisRaw(upDown) + ")");
            yield return new WaitForSeconds(.1f);
        }
    }

    #endregion

    #region ANIMATION_METHODS
    /// <summary>
    /// Updates the animator depending on which state the Minion is in. Only call
    /// this method if the <see cref="anim"/> is not <c>null</c>.
    /// </summary>
    protected void UpdateAnimator()
    {
        anim.SetBool("IsMoving", moveType != MinionMoveTypes.Halt);
    }
    #endregion
}
