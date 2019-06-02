using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShootController : NPCShootController
{
    [SerializeField]
    protected Transform barrel_right;
    [SerializeField]
    protected Transform barrel_left;
    [SerializeField]
    private Transform orb;
    [SerializeField]
    private float waitTime; 
    [SerializeField]
    private float aimSpeed;
    [SerializeField]
    private float timeStep;

    [SerializeField]
    private float coolDown;

    private float timeBetweenShots;

    private bool targetAcquired;
    GameObject target;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        targetAcquired = false;
        timeBetweenShots = 0.0f;
    }


    protected override void Update(){
        if(health.isNotDead()){
            if(target == null || !IsInRange(target)){
                StartCoroutine("FindTarget");
           
            }
            else{
                if(IsValidTarget(target) && targetHealth.isNotDead()){
                    Point();
                }
            }
            timeBetweenShots += Time.deltaTime;
        }
    }

  
    IEnumerator FindTarget(){
        
        yield return new WaitForSeconds(waitTime);
        DetectTargets();
        target = AcquireTarget();
        
    }

    void Point(){
        
        Quaternion shootDir = Quaternion.LookRotation((target.transform.position - orb.position).normalized);

        orb.rotation = Quaternion.Slerp(orb.rotation, shootDir, Time.deltaTime*aimSpeed);
       /*  while (Vector3.Dot(orb.forward, shootDir) < .9995f) // threshold because chances are this won't be exact
        {
            turnSpeed = Vector3.AngleBetween(orb.forward, shootDir);
            orb.rotation = Quaternion.LookRotation(
                
                Vector3.RotateTowards(orb.forward, shootDir, turnSpeed*Time.deltaTime, 0.0f)
                );
            yield return null;
        } 
        */
        bool linedUp = (Vector3.Dot(orb.forward, (target.transform.position - orb.position).normalized) > .99f);
        if(IsInRange(target) && timeBetweenShots >= rateOfFire && linedUp){
            Shoot();
            timeBetweenShots = 0.0f;
        }
        
    }
    
    private bool IsInRange(GameObject t){
        if(t != null){
            float distance = (t.transform.position - orb.position).magnitude;
            return distance <= range;
        }
        else{
            return false;
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
        priorities.Add("P1_Teddy", 1);
        priorities.Add("P2_Teddy", 1);

        priorities.Add("P1_Soldier", 2);
        priorities.Add("P2_Soldier", 2);

        priorities.Add("P1_Base", 5);
        priorities.Add("P2_Base", 5);
        priorities.Add("P1_Spawner", 5);
        priorities.Add("P2_Spawner", 5);
        priorities.Add("P1_Turret", 4);
        priorities.Add("P2_Turret", 4);
        priorities.Add("P1_Healer", 5);
        priorities.Add("P2_Healer", 5);

        priorities.Add("Player1_obj", 3);
        priorities.Add("Player2_obj", 3);
        // Not sure if these last two are necessary, but it can't hurt to add them
        
    }
}
