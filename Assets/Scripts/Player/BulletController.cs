using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public const int BULLET_DAMAGE = 10;
    public GameObject templateBullet;

    // Start is called before the first frame update
    void Start()
    {
        
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
                if (this.gameObject.tag != "P1_Bullet")
                {
                    collision.gameObject.GetComponent<HealthBehavior>().adjustHealth(-BULLET_DAMAGE);
                }
                ResetBullet();
                break;
            case "P2_Base":
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
            
        }
    }

    private void ResetBullet()
    {
        // make it invisible
        this.gameObject.SetActive(false);
        // Reset velocity
        this.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        // put it back in the turret
        this.transform.position = templateBullet.transform.position;
    }
}
