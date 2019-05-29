using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShootController : NPCShootController
{
    [SerializeField]
    protected Transform barrel_right;
    [SerializeField]
    protected Transform barrel_left;
    


    private bool targetAcquired;

    private float t;
    // Start is called before the first frame update
    protected override void Start()
    {
        targetAcquired = true;
        base.Start();
    }

    protected override void Update(){
        if(t >= this.rateOfFire && targetAcquired){
            Shoot();
            t -= this.rateOfFire;
        }
        else{
            t += Time.deltaTime;
        }

    }

    

    public override void Shoot(){
        RecyclableBullet b_f = bulletPool.Request(barrel);
        RecyclableBullet b_r = bulletPool.Request(barrel_right);
        RecyclableBullet b_l = bulletPool.Request(barrel_left);
        GameObject go_f = b_f.gameObject;
        GameObject go_r = b_r.gameObject;
        GameObject go_l = b_l.gameObject;
        Rigidbody rb_f = go_f.GetComponent<Rigidbody>();
        Rigidbody rb_r = go_r.GetComponent<Rigidbody>();
        Rigidbody rb_l = go_l.GetComponent<Rigidbody>();
        rb_f.AddForce(bulletSpeed * barrel.forward, ForceMode.VelocityChange);
        rb_r.AddForce(bulletSpeed * barrel_right.forward, ForceMode.VelocityChange);
        rb_l.AddForce(bulletSpeed * barrel_left.forward, ForceMode.VelocityChange);
    }

    public override void DefinePriorities()
    {
        throw new System.NotImplementedException();
    }
}
