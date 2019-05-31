using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{

    //private static TagManager taggyboi = new TagManager();

    /// <summary>
    /// A bullet struct that holds the GameObject and if it was already fired.
    /// </summary>
    struct Bullet
    {
        public GameObject bullet_GameObject;
        public bool alreadyFired;

        public Bullet(GameObject bulletGO, bool wasFired)
        {
            bullet_GameObject = bulletGO;
            alreadyFired = wasFired;
        }
    }

    /// <summary>
    /// A string that denotes which input to use.
    /// </summary>
    public string shoot;
    /// <summary>
    /// The template bullet to use to generate others.
    /// </summary>
    public GameObject templateBullet;
    /// <summary>
    /// The barrel of the tank, used to determine where the turret is facing
    /// </summary>
    public GameObject barrel;
    /// <summary>
    /// A GameObject that holds all bullets instantiated by player.
    /// After maxBullets are instantiated, each bullet is recycled.
    /// </summary>
    public GameObject bulletFondler;
    /// <summary>
    /// The speed of bullet.
    /// </summary>
    public int speedOfBullet;
    /// <summary>
    /// The reload time of the tank between bullets shot.
    /// </summary>
    public float reloadTime;

    /// <summary>
    /// The max number of bullets of the player at any time during gameplay.
    /// </summary>
    public int maxBullets;

    /// <summary>
    /// Where all bullets are held and referenced.
    /// </summary>
    private List<Bullet> bullets;
    /// <summary>
    /// Counter to know which bullet is to be reset and fired next.
    /// </summary>
    private int objectPoolCounter;

    /// <summary>
    /// States for the shooting state machine.
    /// </summary>
    private enum SHOOT_States { WAIT, FIRE, COUNT_ON, COUNT_OFF }
    SHOOT_States state;

    /// <summary>
    /// The timer that starts counting when the player fires a bullet, and 
    /// stops when it reaches the set reloadTime.
    /// </summary>
    private float reloadTimer;

    private TankController tc;
    AudioSource shootSound;

    // Start is called before the first frame update
    void Start()
    {

        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                shoot = (tag == "Player1_obj" ? "shoot-1-mac" : "shoot-2-mac");
                break;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                shoot = (tag == "Player1_obj" ? "shoot-1-pc" : "shoot-2-pc");
                break;

            default:
                Debug.LogError("Mappings not setup for operating systems other than Windows or Mac OS");
                break;
        }
        reloadTimer = 0f;
        bullets = new List<Bullet>();
        // Start counter at 0, used in RecycleBullets()
        objectPoolCounter = 0;
        // Sets initial state for shooting SM
        state = SHOOT_States.WAIT;

        tc = GetComponent<TankController>();
        shootSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // FixedUpdate is called at a constant rate, independent from framerate.
    // Used for all physics calculations.
    void FixedUpdate()
    {
        // Pass the current state of the button into the shooting state machine.
        // Only if the player is currently alive.
        //if (!taggyboi.isMinion(tag))
        //{
        if (tc.State == TankStates.Alive)
            FireRateStateMachine(Input.GetButton(shoot));
        //}


        for (int i = 0; i < bullets.Count; i++)
        {
            // Only add force if bullet hasn't been fired yet.
            if (!bullets[i].alreadyFired)
            {
                // Reset velocity, just in case
                bullets[i].bullet_GameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                // Add force to bullet
                bullets[i].bullet_GameObject.GetComponent<Rigidbody>().AddForce(speedOfBullet * barrel.transform.parent.forward, ForceMode.VelocityChange);
                // Update bullet's alreadyFired bool from false to true
                bullets[i] = new Bullet(bullets[i].bullet_GameObject, true);
            }

            // Since bullets are stored at the tip of the barrel, there is a chance they might collide with the barrel itself,
            // which will cause incorrect trajectories. This should fix it.
            // Physics.IgnoreCollision(bullets[i].bullet_GameObject.GetComponent<SphereCollider>(), barrel.GetComponent<CapsuleCollider>());
        }
    }

    // LateUpdate is called at the end of every frame
    private void LateUpdate()
    {
        // bullet fondler ignores rotation of tank so bullets don't rotate along with tank.
        bulletFondler.transform.rotation = Quaternion.identity;
    }

    // Resets the bullet (gameobject, not struct)
    // Called by HealthBehavior script when colliding with damagable obect.
    //public void ResetBullet(GameObject bullet)
    //{
    //    // make it invisible
    //    bullet.SetActive(false);
    //    // Reset velocity
    //    bullet.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
    //    // put it back in the turret
    //    bullet.transform.position = templateBullet.transform.position;
    //}

    /// <summary>
    /// Instantiates a bullet and adds it to the list.
    /// </summary>
    private void CreateAndAddBullet()
    {
        templateBullet.SetActive(true);
        GameObject newBulletGO = Instantiate(templateBullet, templateBullet.transform.position, this.transform.rotation, this.transform);
        templateBullet.SetActive(false);
        newBulletGO.transform.parent = bulletFondler.transform;
        Bullet bullet = new Bullet(newBulletGO, false);
        bullets.Add(bullet);

    }

    /// <summary>
    /// Recycles bullets (will be called the most throughout the game)
    /// </summary>
    private void RecycleBullets()
    {
        bullets[objectPoolCounter].bullet_GameObject.SetActive(false);
        bullets[objectPoolCounter].bullet_GameObject.SetActive(true);
        bullets[objectPoolCounter] = new Bullet(bullets[objectPoolCounter].bullet_GameObject, false);
        bullets[objectPoolCounter].bullet_GameObject.transform.position = templateBullet.transform.position;
        bullets[objectPoolCounter].bullet_GameObject.GetComponent<TrailRenderer>().Clear();

        if (objectPoolCounter < maxBullets - 1)
            objectPoolCounter++;
        else
            objectPoolCounter = 0;
    }

    /// <summary>
    /// Decides whether to instantiate new bullets or reuse them.
    /// </summary>
    /// <param name="buttonPressed">If set to <c>true</c> button pressed.</param>
    private void Fire(bool buttonPressed)
    {
        if (bullets.Count < maxBullets && buttonPressed)
        {
            CreateAndAddBullet();
        }
        else if (bullets.Count == maxBullets && buttonPressed)
        {
            RecycleBullets();
        }
    }

    /// <summary>
    /// A state machine that determines how fast and how often to fire a bullet,
    /// even if the fire button is pressed many times or held down for a while.
    /// </summary>
    /// <param name="buttonPressed">If set to <c>true</c> button pressed.</param>
    private void FireRateStateMachine(bool buttonPressed)
    {
        switch (state) // Transitions
        {
            case SHOOT_States.WAIT:
                if (buttonPressed)
                    state = SHOOT_States.FIRE;
                break;
            case SHOOT_States.FIRE:
                state = SHOOT_States.COUNT_ON;
                break;
            case SHOOT_States.COUNT_ON:
                if (reloadTimer >= reloadTime && buttonPressed)
                    state = SHOOT_States.WAIT;
                if (reloadTimer < reloadTime && !buttonPressed)
                    state = SHOOT_States.COUNT_OFF;
                break;
            case SHOOT_States.COUNT_OFF:
                if (reloadTimer >= reloadTime && !buttonPressed)
                    state = SHOOT_States.WAIT;
                if (buttonPressed)
                    state = SHOOT_States.COUNT_ON;

                break;
            default:
                Debug.LogError("Not supposed to be here - transition sm");
                break;
        }

        switch (state) // Actions
        {
            case SHOOT_States.WAIT:
                reloadTimer = 0f;
                break;
            case SHOOT_States.FIRE:
                reloadTimer = 0f;
                Fire(true);
                shootSound.Play();
                break;
            case SHOOT_States.COUNT_ON:
                reloadTimer += Time.fixedDeltaTime;
                break;
            case SHOOT_States.COUNT_OFF:
                reloadTimer += Time.fixedDeltaTime;
                break;
            default:
                Debug.LogError("Not supposed to be here - action sm");
                break;
        }
    }
}
