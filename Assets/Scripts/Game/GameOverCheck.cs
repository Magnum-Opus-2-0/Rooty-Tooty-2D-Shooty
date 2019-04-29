using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
//using UnityEngine.UI;


public class GameOverCheck : MonoBehaviour
{
    public GameObject playerOneBase;
    public GameObject playerTwoBase;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerOneBase.GetComponent<HealthBehavior>().currentHealth <= 0 || playerTwoBase.GetComponent<HealthBehavior>().currentHealth <= 0)
        {
            SceneManager.LoadSceneAsync("01_Title");
        }
    }
}
