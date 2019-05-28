using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseHealthIconController : MonoBehaviour
{
    [SerializeField]
    private bool isP1;

    private HealthBehavior hb;
    private Image star;
    private float denominator;
    // Start is called before the first frame update
    void Start()
    {
        GameObject playerBase;
        if(isP1){
            playerBase = GameObject.FindWithTag("P1_Base");
        }
        else{
            playerBase = GameObject.FindWithTag("P2_Base");
        }

        hb = playerBase.GetComponent<HealthBehavior>();
        star = GetComponent<Image>();
        denominator = (float) hb.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        star.fillAmount = hb.currentHealth / denominator;
    }
}
