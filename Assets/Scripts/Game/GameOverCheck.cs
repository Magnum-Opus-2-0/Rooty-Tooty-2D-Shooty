using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameOverCheck : MonoBehaviour
{
    public GameObject playerOneBase;
    public GameObject playerTwoBase;

    public Transform pivot;
    public Image winningPlayer;
    public Canvas endCanvas;

    public Sprite imgPlayer1;
    public Sprite imgPlayer2;

    private GameObject winner;
    private bool once;

    // Start is called before the first frame update
    void Start()
    {
        winner = null;
        endCanvas.gameObject.SetActive(false);
        once = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Uncomment to set a random winner via click

        if ((playerOneBase.GetComponent<HealthBehavior>().currentHealth <= 0 || playerTwoBase.GetComponent<HealthBehavior>().currentHealth <= 0) && once)
        {
            SetWinner();
            EndGame();
            once = false;
        }
    }

    /// <summary>
    /// Performs the end game processes such as switching the cameras, setting
    /// the winning player text, etc.
    /// </summary>
    private void EndGame()
    {
        GameObject.Find("Cam_P1(Clone)").SetActive(false);
        GameObject.Find("Cam_P2(Clone)").SetActive(false);
        pivot.gameObject.SetActive(true);

        if (winner == playerOneBase)
        {
            winningPlayer.sprite = imgPlayer1;
        }
        else if (winner == playerTwoBase)
        {
            winningPlayer.sprite = imgPlayer2;
        }

        endCanvas.gameObject.SetActive(true);
    }

    /// <summary>
    /// Sets the winner.
    /// </summary>
    private void SetWinner()
    {
        if (playerOneBase.GetComponent<HealthBehavior>().currentHealth <= 0)
        {
            winner = playerTwoBase;
        }
        else if (playerTwoBase.GetComponent<HealthBehavior>().currentHealth <= 0)
        {
            winner = playerOneBase;
        }
    }

    public void BackToTitle()
    {
        SceneManager.LoadSceneAsync("05_LoadingTitle");
    }
}
