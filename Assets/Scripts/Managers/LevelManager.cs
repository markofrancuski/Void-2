using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pixelplacement;

public class LevelManager : Singleton<LevelManager>
{

    [Header("Current Level Settings")]
    [SerializeField] private int currentChapter;
    public int maxMoves;
    [SerializeField] private int tempMaxMoves;

    public int heartToCollect;
    [SerializeField] private int tempHeartToCollect;

    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI heartsText;

    #region Unity Functions

    private void Start()
    {
        UpdateMoves();
        UpdateHearts();
        
        InputManager.Instance.ControlPlayer();
       
    }

    private void OnEnable()
    {
        InputManager.OnSwipedEvent += ReduceMoves;
        CanvasManager.OnResetLevelEventHandler += ResetLevel;

        tempMaxMoves = maxMoves;
        tempHeartToCollect = heartToCollect;
        
    }

    private void OnDisable()
    {
        InputManager.OnSwipedEvent -= ReduceMoves;
        CanvasManager.OnResetLevelEventHandler -= ResetLevel;
    }

    #endregion

    public void ReduceMoves(string t)
    {
        Debug.Log("Called");
        tempMaxMoves--;
        UpdateMoves();
        if (tempMaxMoves <= 0)
        {
            InputManager.Instance.UnControlPlayer();

            if (tempHeartToCollect <= 0)
            {
                //Level Passed
                CanvasManager.Instance.UpdatePanel( NextLevel , false, -1);
                //Invoke 
                return;
            }

            // You didn't pass the level => Out of moves!
            CanvasManager.Instance.UpdatePanel( ResetLevel , true, 0);
            // Invoke option to reset level

        }
    }

    #region UI functions

    private void UpdateMoves() => movesText.SetText(tempMaxMoves.ToString());
    private void UpdateHearts() => heartsText.SetText(tempHeartToCollect.ToString());



    

    private void ResetLevel()
    {
        tempMaxMoves = maxMoves;
        tempHeartToCollect = heartToCollect;
        CanvasManager.Instance.ResetLevel();
        //ResetLevel

    }

    private void NextLevel()
    {

    }

    #endregion

}
