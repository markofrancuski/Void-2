﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityEngine.SceneManagement;
using MEC;
using TMPro;

public class LevelManager : Singleton<LevelManager>
{
    public delegate void ResetLevelEvent();

    public static event ResetLevelEvent OnResetLevelEventHandler;

    [SerializeField] private int currentLevel;

    public int maxMoves;
    private int tempMaxMoves;

    public void ResetLevel()
    {
        OnResetLevelEventHandler?.Invoke();
    }

    public void LoadLevel(int levelIndex)
    {
        currentLevel = levelIndex;
        Timing.RunCoroutine(_LoadAsyncLevel(currentLevel));
    }

    private IEnumerator<float> _LoadAsyncLevel(int indexLevel)
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(indexLevel);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return Timing.WaitForOneFrame;
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log("Level Loaded!");
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }
}