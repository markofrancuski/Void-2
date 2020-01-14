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
    [SerializeField] private TextMeshProUGUI defeatText;

    [SerializeField] private GameObject levelGO;

    public int Players;

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

        //InputManager.Instance.ControlPlayer();
        Invoke("SceneReady", .2f);
    }

    void SceneReady()
    {
       Globals.Instance.isSceneReady = true;
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
        tempMaxMoves--;
        UpdateMoves();
        if (tempMaxMoves <= 0)
        {   
            // You didn't pass the level => Out of moves!
            ShowDefeatPanel();
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
    /// <summary>
    /// Loads up the Main Menu Scene
    /// </summary>
    public void OnBackButtonClicked()
    {
        if (isClicked) return;
        isClicked = true;
        //Load Main Menu scene
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Shows up the animation and loads next level or chapter.
    /// </summary>
    public void OnNextLevelClicked()
    {
        canvasAnimator.SetBool("WaitForLevelSetup", true);
        Globals.Instance.GoToNextLevelInChapter();
        //Transition in black
        //Async Load
    }

    /// <summary>
    /// Shows the defeat panel with provided text displayed
    /// </summary>
    public void ShowDefeatPanel(string txt)
    {
        InputManager.Instance.UnControlPlayer();
        defeatText.SetText("Died from " + txt);
        canvasAnimator.SetTrigger("Defeat");
    }

    /// <summary>
    /// Shows the defeat panel with 'Out of the moves' text displayed
    /// </summary>
    public void ShowDefeatPanel()
    {
        InputManager.Instance.UnControlPlayer();
        defeatText.SetText("Out of the moves!");
        canvasAnimator.SetTrigger("Defeat");
    }
    
    #endregion

}
