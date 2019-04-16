using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    public string shoot = "shoot-1-pc";

    public GameObject bullet;
    public GameObject barrel;
    public GameObject ballFondler;
    public int speedOfBullet;

    private List<GameObject> bullets = new List<GameObject>();
    private const int MAX_BULLETS = 3;

    private float triggerInput;

    private Rigidbody rb;
    private int objectPoolCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        bullets.Clear();
        rb = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if (bullets.Count < MAX_BULLETS && Input.GetButtonDown(shoot))
        {
            bullet.SetActive(true);

            GameObject newBullet = Instantiate(bullet, bullet.transform.position, this.transform.rotation, this.transform);
            //newBullet.transform.rotation = newBullet.transform.localRotation;
            newBullet.transform.parent = ballFondler.transform;
            //newBullet.transform.position = this.transform.TransformPoint(this.transform.localPosition);
            newBullet.GetComponent<Rigidbody>().AddForce(speedOfBullet * barrel.transform.parent.forward, ForceMode.VelocityChange);

            Debug.Log(newBullet.GetComponent<Rigidbody>().velocity);

            bullet.SetActive(false);

            bullets.Add(newBullet);

            Debug.Log("added bullet");
        }
        else if (bullets.Count == MAX_BULLETS && Input.GetButtonDown(shoot))
        {
            bullets[objectPoolCounter].transform.position = bullet.transform.position;
            //bullets[objectPoolCounter].transform.rotation = this.transform.rotation;
            bullets[objectPoolCounter].GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            //bullets[objectPoolCounter].transform.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, 0f), ForceMode.VelocityChange);
            bullets[objectPoolCounter].GetComponent<Rigidbody>().AddForce(speedOfBullet * barrel.transform.parent.forward, ForceMode.VelocityChange);

            Debug.Log(bullets[objectPoolCounter].GetComponent<Rigidbody>().velocity);

            if (objectPoolCounter < bullets.Count - 1)
                objectPoolCounter++;
            else
                objectPoolCounter = 0;
        }

        if (bullets.Count > 0)
        {
            foreach (GameObject currentBullet in bullets)
            {
                Physics.IgnoreCollision(currentBullet.GetComponent<SphereCollider>(), barrel.GetComponent<CapsuleCollider>());
                //bullet.transform.GetComponent<Rigidbody>().AddForce(speedOfBullet * bullet.transform.forward, ForceMode.Impulse);
            }
        }
    }
}
