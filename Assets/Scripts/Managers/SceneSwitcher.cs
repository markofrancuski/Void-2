using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using Pixelplacement;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneSwitcher : Singleton<SceneSwitcher>
{

    /// <summary>
    /// Async loading main menu scene
    /// </summary>
    /// <param name="waitTime">Wait for seconds after finished loading scene. Used for playing some animations, etc...</param>
    /// <param name="SceneLoadedCallback">Optional parametar to close/open some ui elements or reset some values </param>
    public void LoadMainMenu(float waitTime, Action SceneLoadedCallback = null)
    {
        Debug.Log($"Switching from: {SceneManager.GetActiveScene().name}" + " to: Main Menu scene" );
        Timing.RunCoroutine(_LoadMainMenu(waitTime, SceneLoadedCallback).CancelWith(gameObject));
    }
    private IEnumerator<float> _LoadMainMenu(float waitTime, Action SceneLoadedCallback = null)
    {
        Globals.Instance.isSceneReady = false;
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);
        operation.allowSceneActivation = false;

        while(!operation.isDone)
        {
            if (operation.progress >= .9f) break;
            yield return Timing.WaitForOneFrame;
        }

        yield return Timing.WaitForSeconds(waitTime);
        SceneLoadedCallback?.Invoke();
        
        operation.allowSceneActivation = true;
        Debug.Log($"Switched to the Main Menu scene");
    }

    /// <summary>
    /// Async loading the scene you provide
    /// </summary>
    /// <param name="waitTime">Wait for seconds after finished loading scene. Used for playing some animations, etc...</param>
    /// <param name="sceneName"> Name of the scene to load </param>
    /// <param name="SceneLoadedCallback">Optional parametar to close/open some ui elements or reset some values </param>
    public void LoadLevel(float waitTime, string sceneName, Action SceneLoadedCallback = null, Action SceneBeforeStartCallback = null)
    {
        Debug.Log($"Switching from: {SceneManager.GetActiveScene().name}" + " to: " + sceneName + " scene");
        Timing.RunCoroutine(_LoadLevel(waitTime, sceneName, SceneLoadedCallback, SceneBeforeStartCallback).CancelWith(gameObject));
    }
    private IEnumerator<float> _LoadLevel(float waitTime, string sceneName, Action SceneLoadedCallback = null, Action SceneBeforeStartCallback = null)
    {
        Globals.Instance.isSceneReady = false;
        SceneBeforeStartCallback?.Invoke();
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while(!operation.isDone)
        {
            if (operation.progress >= .9f) break;
            yield return Timing.WaitForOneFrame;
        }

        yield return Timing.WaitForSeconds(waitTime);
        SceneLoadedCallback?.Invoke();
        
        operation.allowSceneActivation = true;
        //Debug.Log($"Switched to the " + sceneName + " scene");
    }

}
