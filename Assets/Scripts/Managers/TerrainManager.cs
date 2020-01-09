using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public float moveX;
    public float moveY;

    [SerializeField] private List<GameObject> prefabs = new List<GameObject>();
    [SerializeField] private Vector3 currentPosition = Vector3.zero;
    [SerializeField] private Transform parentTerrain;
    [SerializeField] private float cameraSize;
    public bool isStarted;

    public List<GameObject> spawnedLevels;

    private void Start() 
    {
        cameraSize = Camera.main.orthographicSize*2;

        SpawnLevel(true);
    }

    private void FixedUpdate() 
    {
         //if(spawnedLevels.Count <= 2) SpawnNextChuck(false);
    }

    public void SpawnLevel(bool isStart)
    {
        //if (!isStart) currentPosition = spawnedLevels[spawnedLevels.Count - 1].transform.position + new Vector3(0, cameraSize - Time.deltaTime, 0); // -0.03f => Time.deltaTime
        GameObject go;
        if (Globals.Instance.currentLevel == -1) { go = Instantiate(prefabs[0]); }
        else { go = Instantiate(prefabs[Globals.Instance.currentLevel]); }
        go.transform.SetParent(parentTerrain);
        go.transform.position = currentPosition;

        currentPosition += new Vector3(0, cameraSize, 0);
        spawnedLevels.Add(go);

        /*for (int i = 0; i < prefabs.Count; i++)
        {
            if(!isStart) currentPosition = spawnedLevels[spawnedLevels.Count-1].transform.position + new Vector3(0,cameraSize - Time.deltaTime, 0); // -0.03f => Time.deltaTime
            GameObject go = Instantiate(prefabs[i]);
            go.transform.SetParent(parentTerrain);
            go.transform.position = currentPosition;

            currentPosition += new Vector3(0, cameraSize, 0);
            spawnedLevels.Add(go);
        }
        */
    }
}
