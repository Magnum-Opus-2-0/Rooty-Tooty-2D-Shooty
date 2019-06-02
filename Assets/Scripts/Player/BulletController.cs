using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField]
    public int bulletDamage = 5;
    //public ShootController shootControlReference;
    //public GameObject templateBullet;

    protected TrailRenderer trail;

    protected HealthBehavior collisionHB;

    // Start is called before the first frame update
    void Start()
    {
        trail = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player1_obj":
                if (this.gameObject.tag != "P1_Bullet")
                {
                    collisionHB = collision.gameObject.transform.root.GetComponent<HealthBehavior>();
                    if (collisionHB)
                    {
                        collisionHB.adjustHealth(-bulletDamage);
                    }
                }
                ResetBullet();
                break;
            case "Player2_obj":
                if (this.gameObject.tag != "P2_Bullet")
                {
                    collisionHB = collision.gameObject.transform.root.GetComponent<HealthBehavior>();
                    if (collisionHB)
                    {
                        collisionHB.adjustHealth(-bulletDamage);
                    }
                }
                ResetBullet();
                break;
            case "P1_Base":
            case "P1_Spawner":
            case "P1_Turret":
            case "P1_Healer":
            case "P1_Soldier":
            case "P1_Teddy":
                if (this.gameObject.tag != "P1_Bullet")
                {
                    collisionHB = collision.gameObject.transform.GetComponent<HealthBehavior>();
                    if (collisionHB)
                    {
                        collisionHB.adjustHealth(-bulletDamage);
                    }
                }
                ResetBullet();
                break;
            case "P2_Base":
            case "P2_Spawner":
            case "P2_Turret":
            case "P2_Healer":
            case "P2_Soldier":
            case "P2_Teddy":
                if (this.gameObject.tag != "P2_Bullet")
                {
                    collisionHB = collision.gameObject.transform.GetComponent<HealthBehavior>();
                    if (collisionHB)
                    {
                        collisionHB.adjustHealth(-bulletDamage);
                    }
                }
                ResetBullet();
                break;
            case "Floor":
            case "Wall":
                ResetBullet();
                break;
            default:
                ResetBullet();
                break;
        }
    }

    protected virtual void ResetBullet()
    {
        // make it invisible
        this.gameObject.SetActive(false);
        // Reset velocity
        this.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        // put it back in the turret
        // this.transform.position = shootControlReference.templateBullet.transform.position;
    }
}
