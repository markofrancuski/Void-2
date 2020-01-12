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
    [SerializeField] private int tempHeartToCollect = 0;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI heartsText;

    #region Unity Functions

    private void Start()
    {
        tempHeartToCollect = 0;

        UpdateMoves();
        UpdateHearts();
        
        InputManager.Instance.ControlPlayer();

       
    }

    private void OnEnable()
    {
        InputManager.OnSwipedEvent += ReduceMoves;
        

        tempMaxMoves = maxMoves;
        tempHeartToCollect = heartToCollect;
        
    }

    private void OnDisable()
    {
        InputManager.OnSwipedEvent -= ReduceMoves;
        
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
                
                //Invoke 
                return;
            }

            // You didn't pass the level => Out of moves!
            
            // Invoke option to reset level

        }
    }

    public void HeartCollected()
    {
        tempHeartToCollect++;
    }

    #region UI functions

    private void UpdateMoves() => movesText.SetText(tempMaxMoves.ToString());
    private void UpdateHearts() => heartsText.SetText(tempHeartToCollect.ToString());

    private void ResetLevel()
    {
        tempMaxMoves = maxMoves;
        tempHeartToCollect = heartToCollect;
        
        //ResetLevel

    }

    private void NextLevel()
    {

    }

    #endregion



}
