using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
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

    public string shoot;
    public GameObject templateBullet;
    public GameObject barrel;
    public GameObject ballFondler;
    public int speedOfBullet;

    private List<Bullet> bullets;
    private int objectPoolCounter;

    private const int MAX_BULLETS = 3;

    private enum FIRE_States { WAIT, FIRE, COUNT_ON, COUNT_OFF }
    FIRE_States state;

    public float reloadTime;
    private float reloadDelta;

    // Start is called before the first frame update
    void Start()
    {
        reloadDelta = 0f;
        bullets = new List<Bullet>();
        objectPoolCounter = 0;
        state = FIRE_States.WAIT;
    }

    void Update()
    {
        FireRateStateMachine(Input.GetButton(shoot));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (bullets.Count > 0)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                if (!bullets[i].alreadyFired)
                {
                    bullets[i].bullet_GameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                    bullets[i].bullet_GameObject.GetComponent<Rigidbody>().AddForce(speedOfBullet * barrel.transform.parent.forward, ForceMode.VelocityChange);
                    bullets[i] = new Bullet(bullets[i].bullet_GameObject, true);
                }

                Physics.IgnoreCollision(bullets[i].bullet_GameObject.GetComponent<SphereCollider>(), barrel.GetComponent<CapsuleCollider>());
            }
        }
    }

    private void CreateAndAddBullet()
    {
        templateBullet.SetActive(true);
        GameObject newBulletGO = Instantiate(templateBullet, templateBullet.transform.position, this.transform.rotation, this.transform);
        templateBullet.SetActive(false);
        newBulletGO.transform.parent = ballFondler.transform;
        Bullet bullet = new Bullet(newBulletGO, false);
        bullets.Add(bullet);

    }

    private void RecycleBullets()
    {
        bullets[objectPoolCounter] = new Bullet(bullets[objectPoolCounter].bullet_GameObject, false);
        bullets[objectPoolCounter].bullet_GameObject.transform.position = templateBullet.transform.position;

        if (objectPoolCounter < MAX_BULLETS - 1)
            objectPoolCounter++;
        else
            objectPoolCounter = 0;
    }

    private void Fire(bool buttonPressed)
    {
        if (bullets.Count < MAX_BULLETS && buttonPressed)
        {
            CreateAndAddBullet();
        }
        else if (bullets.Count == MAX_BULLETS && buttonPressed)
        {
            RecycleBullets();
        }
    }

    private void FireRateStateMachine(bool buttonPressed)
    {
        switch (state) // Transitions
        {
            case FIRE_States.WAIT:
                if (buttonPressed)
                    state = FIRE_States.FIRE;
                break;
            case FIRE_States.FIRE:
                state = FIRE_States.COUNT_ON;
                break;
            case FIRE_States.COUNT_ON:
                if (reloadDelta >= reloadTime && buttonPressed)
                    state = FIRE_States.WAIT;
                if (reloadDelta < reloadTime && !buttonPressed)
                    state = FIRE_States.COUNT_OFF;
                break;
            case FIRE_States.COUNT_OFF:
                if (reloadDelta >= reloadTime && !buttonPressed)
                    state = FIRE_States.WAIT;
                if (buttonPressed)
                    state = FIRE_States.COUNT_ON;

                break;
            default:
                Debug.Log("Not supposed to be here - transition sm");
                break;
        }

        switch (state) // Actions
        {
            case FIRE_States.WAIT:
                reloadDelta = 0f;
                break;
            case FIRE_States.FIRE:
                reloadDelta = 0f;
                Fire(true);
                break;
            case FIRE_States.COUNT_ON:
                reloadDelta += Time.fixedDeltaTime;
                break;
            case FIRE_States.COUNT_OFF:
                reloadDelta += Time.fixedDeltaTime;
                break;
            default:
                Debug.Log("Not supposed to be here - action sm");
                break;
        }
    }
}
