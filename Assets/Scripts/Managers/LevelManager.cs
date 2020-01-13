using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pixelplacement;
using UnityEngine.SceneManagement;
using MEC;

public class LevelManager : MonoBehaviour
{

    #region Singleton
    public static LevelManager Instance;
    #endregion

    public delegate void ResetLevelDelegate();
    public static event ResetLevelDelegate ResetLevel;

    [SerializeField] private Animator canvasAnimator;
    [Header("Current Level Settings")]
    [SerializeField] private int currentChapter;

    public int maxMoves;
    [SerializeField] private int tempMaxMoves;

    public int heartToCollect;
    [SerializeField] private int tempHeartToCollect = 0;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI heartsText;
    [SerializeField] private TextMeshProUGUI currentChapterText;
    [SerializeField] private TextMeshProUGUI currentLevelText;

    [SerializeField] private GameObject levelGO;

    #region Unity Functions

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
        Globals.Instance.SpawnCurrentLevel();
        //Instantiate(levelGO, Vector3.zero, Quaternion.identity);

        tempMaxMoves = maxMoves;
        tempHeartToCollect = heartToCollect;

        currentChapterText.SetText("Chapter:" + Globals.Instance.currentChapter);
        currentLevelText.SetText("Level:" + Globals.Instance.currentLevel);

        UpdateMoves();
        UpdateHearts();
        InputManager.Instance.ControlPlayer();

    }

    private void OnEnable()
    {
        InputManager.OnSwipedEvent += ReduceMoves;
        ResetLevel += ResetCurrentLevel;

        //tempMaxMoves = maxMoves;
        //tempHeartToCollect = heartToCollect;

    }

    private void OnDisable()
    {
        InputManager.OnSwipedEvent -= ReduceMoves;
        ResetLevel -= ResetCurrentLevel;
    }

    #endregion

    /// <summary>
    /// Gets called when you swipe to move your character
    /// </summary>
    public void ReduceMoves(string t)
    {
        Debug.Log("Reduce move is called");
        tempMaxMoves--;
        UpdateMoves();
        if (tempMaxMoves <= 0)
        {
            InputManager.Instance.UnControlPlayer();
            // You didn't pass the level => Out of moves!
            Debug.Log("Out of moves, you are Defeated! ");
            canvasAnimator.SetTrigger("Defeat");
            // Invoke option to reset level

        }
    }

    /// <summary>
    /// Gets called when you collect heart in current level
    /// </summary>
    public void HeartCollected()
    {
        tempHeartToCollect--;      
        if(tempHeartToCollect <= 0 && tempMaxMoves > 0)
        {
            Debug.Log("Victory! ");
            InputManager.Instance.UnControlPlayer();
            canvasAnimator.SetTrigger("Victory");
            //Switch to the next level/chapter
        }
        UpdateHearts();
    }

    private void ResetCurrentLevel()
    {
        tempMaxMoves = maxMoves;
        tempHeartToCollect = heartToCollect;
    }

    #region UI functions

    private void UpdateMoves() => movesText.SetText(tempMaxMoves.ToString());
    private void UpdateHearts() => heartsText.SetText(tempHeartToCollect.ToString());

    private void NextLevel()
    {
        Globals.Instance.GoToNextLevelInChapter();
        //Set Up Animation => black Screen
        //Destroy current Level
        //Instantate new level
        //Position Player
        //Control Player
        //Set values for that level => moves, hearts
    }

    public void OnResetCurrentLevelClicked()
    {
        canvasAnimator.SetBool("WaitForLevelSetup", true);
        SceneSwitcher.Instance.LoadLevel(2f, Globals.Instance.GetCurrentLevelSceneName());
        //ResetLevel?.Invoke();
    }

    private bool isClicked;
    public void OnBackButtonClicked()
    {
        if (isClicked) return;
        isClicked = true;
        //Load Main Menu scene
        SceneManager.LoadScene(1);
    }

    public void OnNextLevelClicked()
    {
        canvasAnimator.SetBool("WaitForLevelSetUp", true);
        Globals.Instance.GoToNextLevelInChapter();
        //Transition in black
        //Async Load
    }

    #endregion

}
