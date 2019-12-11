using UnityEngine;

public class Globals : MonoBehaviour
{
    public static Globals Instance;


    public float movePaceVertical;
    public float movePaceHorizontal;

    public Vector3 upVector;
    public Vector3 downVector;
    public Vector3 rightVector;
    public Vector3 leftVector;

    public float horizontalBoundary;
    public float verticalBoundary;

    public float tweenDuration = 0.5f;

    [SerializeField] private Transform TerrainObject;


    private void Awake()
    {
        Instance = this;

    }
    private void Start()
    {
        Invoke("RetrieveMove", .15f);
    }
    void RetrieveMove()
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
