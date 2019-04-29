using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public string playScene;
    public string tutorialScene;
    public string optionsScene;
    public string creditsScene;

    public Dropdown dd;

    /// <summary>
    /// The name of the developer whose test scene we want to switch to.
    /// </summary>
    private string testName;


    // Start is called before the first frame update
    void Start()
    {

        SelectName();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Switch to the play scene.
    /// </summary>
    public void SwitchToPlay()
    {
        SceneManager.LoadSceneAsync(playScene);
    }

    /// <summary>
    /// Switch to the tutorial scene.
    /// </summary>
    public void SwitchToTutorial()
    {
        SceneManager.LoadSceneAsync(tutorialScene);
    }

    /// <summary>
    /// Switch to the options scene.
    /// </summary>
    public void SwitchToOptions()
    {
        SceneManager.LoadSceneAsync(optionsScene);
    }

    /// <summary>
    /// Switch to the credits scene.
    /// </summary>
    public void SwitchToCredits()
    {
        SceneManager.LoadSceneAsync(creditsScene);
    }

    /// <summary>
    /// Quits the game in both the editor and player.
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /// <summary>
    /// Selects the developer's name depending on the dropdown menu's value.
    /// </summary>
    public void SelectName()
    {
        switch (dd.value)
        {
            case 0:
                testName = "Michael";
                break;

            case 1:
                testName =  "Andrew";
                break;

            case 2:
                testName =  "Ross";
                break;

            case 3:
                testName =  "Ryan";
                break;

            default:
                testName =  "";
                break;
        }
    }

    /// <summary>
    /// Switchs to the developer's test scene. Specified by <see cref=" testName"/>.
    /// </summary>
    public void SwitchToTest()
    {
        if (testName == "Michael")
        {
            SceneManager.LoadSceneAsync("00_Michael_Test");
        }
        else if (testName == "Andrew")
        {
            SceneManager.LoadSceneAsync("00_Andrew_Test");
        }
        else if (testName == "Ross")
        {
            SceneManager.LoadSceneAsync("00_Ross_Test");
        }
        else if (testName == "Ryan")
        {
            SceneManager.LoadSceneAsync("00_Ryan_Test");
        }
    }
}
