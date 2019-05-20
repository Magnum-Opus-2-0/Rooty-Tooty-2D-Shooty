using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField]
    public const int BULLET_DAMAGE = 5;
    //public ShootController shootControlReference;
    //public GameObject templateBullet;

    protected TrailRenderer trail;

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
                    collision.gameObject.GetComponent<HealthBehavior>().adjustHealth(-BULLET_DAMAGE);
                }
                ResetBullet();
                break;
            case "Player2_obj":
                if (this.gameObject.tag != "P2_Bullet")
                {
                    collision.gameObject.GetComponent<HealthBehavior>().adjustHealth(-BULLET_DAMAGE);
                }
                ResetBullet();
                break;
            case "P1_Base":
            case "P1_Spawner":
            case "P1_Turret":
            case "P1_Healer":
                if (this.gameObject.tag != "P1_Bullet")
                {
                    collision.gameObject.GetComponent<HealthBehavior>().adjustHealth(-BULLET_DAMAGE);
                }
                ResetBullet();
                break;
            case "P2_Base":
            case "P2_Spawner":
            case "P2_Turret":
            case "P2_Healer":
                if (this.gameObject.tag != "P2_Bullet")
                {
                    collision.gameObject.GetComponent<HealthBehavior>().adjustHealth(-BULLET_DAMAGE);
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
