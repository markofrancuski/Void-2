using UnityEngine;
using Pixelplacement;

public class Globals : Singleton<Globals>
{

    public float movePaceVertical;
    public float movePaceHorizontal;

    public Vector3 upVector;
    public Vector3 downVector;
    public Vector3 rightVector;
    public Vector3 leftVector;

    public float horizontalBoundary;
    public float verticalBoundary;

    public float tweenDuration = 0.5f;

    public Transform TerrainObject;

    public int currentChapter;
    public int currentLevel;

    private void Start()
    {
        //Invoke("RetrieveMove", .15f);
    }

    public void RetrieveMove()
    {
        Level levelScript = TerrainObject.GetChild(0).GetComponent<Level>();
        movePaceHorizontal = levelScript.moveX;
        movePaceVertical = levelScript.moveY;

        upVector = new Vector3(0, movePaceVertical, 0);
        downVector = new Vector3(0, -movePaceVertical, 0);
        rightVector = new Vector3(movePaceHorizontal, 0, 0);
        leftVector = new Vector3( -movePaceHorizontal, 0, 0);

        //Set the boundary size
        horizontalBoundary = (movePaceHorizontal * 5f) / 2;
        verticalBoundary = (movePaceVertical * 5f) / 2;

    }
}
