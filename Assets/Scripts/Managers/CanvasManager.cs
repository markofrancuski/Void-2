using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MEC;
using UnityEngine.SceneManagement;
using Pixelplacement;
using System;

public class CanvasManager : Singleton<CanvasManager>
{
     private Action btnDel;

    public delegate void ResetLevelEvent();
    public static event ResetLevelEvent OnResetLevelEventHandler;

    public GameObject mainMenuPanel;

    public GameObject levelSelectionObject;

    public TextMeshProUGUI ChapterTitle;

    public void ShowMainMenu() { mainMenuPanel.SetActive(true); }
    public void HideMainMenu() { mainMenuPanel.SetActive(false); }

    private string chapterString = "Chapter ";

    #region Unity functions

    private void OnEnable()
    {
        OnResetLevelEventHandler += LoadLastLevel;
    }
    private void OnDisable()
    {
        OnResetLevelEventHandler -= LoadLastLevel;
    }

    #endregion

    #region UI functions

    public void ChapterClicked(int index)
    {
        Globals.Instance.currentChapter = index;

        ChapterTitle.SetText(chapterString + index.ToString());
        levelSelectionObject.SetActive(true);
    }

    public void BackClicked()
    {
        levelSelectionObject.SetActive(false);

        Globals.Instance.currentChapter = -1;
        Globals.Instance.currentLevel = -1;

    }

    #endregion

    public void LoadLastLevel() => LoadLevel(Globals.Instance.currentLevel); 

    public void ResetLevel()
    {
        OnResetLevelEventHandler?.Invoke();
    }

    public void LoadLevel(int levelIndex) => Timing.RunCoroutine(_LoadAsyncLevel(1).CancelWith(this.gameObject));

    public void LoadMainMenu() => Timing.RunCoroutine(_LoadMainMenuAsync().CancelWith(this.gameObject));

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {       
        Debug.Log("Chapter Loaded!");
        SceneManager.sceneLoaded -= OnSceneLoaded;
        HideMainMenu();
    }

    private void OnMainMenuLoaded(Scene arg0, LoadSceneMode arg1)
    {
        ShowMainMenu();
        Debug.Log("Main Menu Loaded!");
        Globals.Instance.currentLevel = -1;
        Globals.Instance.currentChapter = -1;
        SceneManager.sceneLoaded -= OnMainMenuLoaded;
    }

    private IEnumerator<float> _LoadMainMenuAsync()
    {
        SceneManager.sceneLoaded += OnMainMenuLoaded;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return Timing.WaitForOneFrame;
        }
    }

    private IEnumerator<float> _LoadAsyncLevel(int levelIndex)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return Timing.WaitForOneFrame;
        }
    }


    #region Defeat/Vicotry Panel
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI chapterTitleText;
    [SerializeField] private TextMeshProUGUI levelTitleText;

    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI buttonText;

    private string resetString = "Reset";
    private string nextString = "Next";
    private string titleWonString = "Congratulations you won!";
    private string[] titleDefeatString = new string[] { " You are out of moves!", " You have died! " };


    public void Death() => UpdatePanel(ResetLevel, true, 1);

    public void UpdatePanel(Action func, bool isDefeat, int defeatStringIndex)
    {
        btnDel = func;

        if (isDefeat)
        {
            if (defeatStringIndex != -1)
            {
                titleText.SetText(titleDefeatString[defeatStringIndex]);
            }
            buttonText.SetText(resetString);
        }
        else
        {
            titleText.SetText(titleWonString);
            buttonText.SetText(nextString);
        }

        winPanel.SetActive(true);
    }

    public void ButtonClick() => btnDel();

    public void BackButton()
    {
        //Load MainMenu
        LoadMainMenu();
    }

    #endregion

}
