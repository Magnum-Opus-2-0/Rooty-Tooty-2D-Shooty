using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public string nextScene;

    public float dotWaitTime;

    public Image[] dots;

    private AsyncOperation sceneLoad;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BlinkDotsWhileLoading());
        StartCoroutine(LoadScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator BlinkDotsOnce()
    {
        TurnDotsOff();
        yield return new WaitForSeconds(dotWaitTime);

        for (int i = 0; i < dots.Length /*&& !sceneLoad.isDone*/; i++)
        {
            dots[i].enabled = true;
            yield return new WaitForSeconds(dotWaitTime);
        }
    }

    public void TurnDotsOff()
    {
        foreach (Image dot in dots)
        {
            dot.enabled = false;
        }
    }

    public IEnumerator BlinkDotsWhileLoading()
    {
        while (/*!sceneLoad.isDone/**/true/**/)
        {
            StartCoroutine(BlinkDotsOnce());
            yield return new WaitForSeconds(dotWaitTime * 4);
        }
    }

    private IEnumerator LoadScene()
    {
        sceneLoad = SceneManager.LoadSceneAsync(nextScene);
        while (!sceneLoad.isDone)
        {
            yield return null;
        }
    }
}
