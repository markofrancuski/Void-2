using UnityEngine;
using Pixelplacement;
using System.Collections.Generic;
using MEC;
using System;

public class Globals : Singleton<Globals>
{
    [Header("Movement Values")]
    public float movePaceVertical;
    public float movePaceHorizontal;

    public Vector3 upVector;
    public Vector3 downVector;
    public Vector3 rightVector;
    public Vector3 leftVector;

    public float horizontalBoundary;
    public float verticalBoundary;

    public float tweenDuration = 0.5f;

    [Header("Level/Chapter Transition Values")]
    public int currentChapter;
    public int currentLevel;
    [SerializeField] private GameObject[] chapter1;
    [SerializeField] private GameObject[] chapter2;
    [SerializeField] private GameObject[] chapter3;
    [SerializeField] private GameObject[] chapter4;
    [SerializeField] private GameObject[] chapter5;

    public GameObject levelGO;

    private void Start()
    {
        //Hard Code The number of levels in each chapter
        LevelsInChapterDictionary = new Dictionary<int, int>();
        LevelsInChapterDictionary.Add(1, 1);
        LevelsInChapterDictionary.Add(2, 2);
        LevelsInChapterDictionary.Add(3, 3);
        LevelsInChapterDictionary.Add(4, 4);
        LevelsInChapterDictionary.Add(5, 5);
    }

    private Dictionary<int, int> LevelsInChapterDictionary;

    public int GetNumberOfLevelsInCurrentChapter()
    {
        if(currentChapter != -1) return LevelsInChapterDictionary[currentChapter];
        return -1;
    }

    public void GoToNextLevelInChapter()
    {
        int val = LevelsInChapterDictionary[currentChapter];

        //There are more levels in Current Chapter
        if (val > currentLevel)
        {
            currentLevel++;
            Destroy(levelGO);            
        }
        //No more levels in Current Chapter
        else
        {
            //There are more Chapters
            if(currentChapter + 1 <= 5)
            {
                currentLevel = 1;
                currentChapter++;
                //Load Next Chapter
            }
            //No more Chapters
            //End of the game
        }
        SceneSwitcher.Instance.LoadLevel(2f, GetCurrentLevelSceneName());
    }

    public void SpawnCurrentLevel()
    {
        switch (currentChapter)
        {
            case 1: Instantiate(chapter1[currentLevel - 1], Vector2.zero, Quaternion.identity); break;
            case 2: Instantiate(chapter2[currentLevel - 1], Vector2.zero, Quaternion.identity); break;
            case 3: Instantiate(chapter3[currentLevel - 1], Vector2.zero, Quaternion.identity); break;
            case 4: Instantiate(chapter4[currentLevel - 1], Vector2.zero, Quaternion.identity); break;
            case 5: Instantiate(chapter5[currentLevel - 1], Vector2.zero, Quaternion.identity); break;

            default: break;
        }
    }

    /// <summary>
    /// In case you need to reset current level.
    /// </summary>
    /// <returns> Returns the name of the current level scene for reloading that scene.</returns>
    public string GetCurrentLevelSceneName()
    {
        return  "Level_" + currentChapter + "_" + currentLevel;
    }
}
