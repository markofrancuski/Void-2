using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MEC;

public class InitManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        isAnimationDone = false;
        Timing.RunCoroutine(_LoadMainMenu().CancelWith(this.gameObject));
        Debug.Log("Starting initManager");
    }

    private bool isAnimationDone = false;
    public void AnimationDone() => isAnimationDone = true;
    public void AnimationNotDone() => isAnimationDone = false;

    private IEnumerator<float> _LoadMainMenu()
    {

        AsyncOperation operation = SceneManager.LoadSceneAsync(1);
        operation.allowSceneActivation = false;

        while (!operation.isDone && !isAnimationDone)
        {
            yield return Timing.WaitForOneFrame;
        }

        Debug.Log("Switching to the MainMenu Scene");
        operation.allowSceneActivation = true;
    }

    

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        //splashBackground.SetActive(false);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
