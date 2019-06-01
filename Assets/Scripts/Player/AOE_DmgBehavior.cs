using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AOE_DmgBehavior : MonoBehaviour
{
    public float radius;
    public int damage;
    public RecyclableBullet bullet;
    public Collider bullet_collider;
    public Collider hitbox_collider;

    public GameObject groundColliderGO;

    private readonly TagManager tagManager = new TagManager();


    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(groundColliderGO.GetComponent<Collider>() != null,
            "Ground collider GameObject doesn't actually have a collider");
    }

    // Update is called once per frame
    void Update()
    {

        // If touching a floor, explode
        Collider bulletCollider = bullet.GetComponent<Collider>();
        Assert.IsTrue(bulletCollider != null,
            bullet.name + " (who is trying to run AOE_DmgBehavior)\n" +
            "does not have a Collider attached!");

    }

    private void OnTriggerEnter(Collider other) {

        Collider groundCollider = groundColliderGO.GetComponent<Collider>();

        if (other.Equals(groundCollider)) {

            StartExplosion();
            DealDamage();
        }
    }

    private void StartExplosion() {

        // bullet better have a Rigidbody!!
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Assert.IsTrue(rb != null, "bullet " + bullet.ToString() + " does not have a Rigidbody attached");



        // Give bullet a collider, set its parameters
        //SphereCollider sc = bullet.gameObject.AddComponent<SphereCollider>();
        SphereCollider sc = bullet.gameObject.GetComponent<SphereCollider>();
        Assert.IsTrue(sc != null, "bullet " + bullet.ToString() + " does not have a SphereCollider attached");

        // Inflate radius, because it's exploding!
        // We'll restore it later, don't worry
        float old_radius = sc.radius;
        sc.radius = radius;




        // Restore the radius to its value before we inflated it,
        // because I'm paranoid
        sc.radius = old_radius;

        // Finally, remove bullet and let its object pooler know
        bullet.Recycle();
    }

    private void DealDamage() {

        // Deal damage to all objects that sc touches
        Collider[] colliders = Physics.OverlapSphere(bullet.transform.position, radius);
        foreach (Collider c in colliders) {

            GameObject go = c.gameObject;

            if (tagManager.isShootable(go.tag)) {

                HealthBehavior hb = go.GetComponent<HealthBehavior>();
                Assert.IsTrue(hb != null,
                    go.name + " does not have a HealthBehavior attached,\n" +
                    "and " + bullet.name + " just tried to deal damage to it");

                hb.adjustHealth(-damage);
            }
        }
    }
}
