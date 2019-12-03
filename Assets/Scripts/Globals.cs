using UnityEngine;

public class Globals : MonoBehaviour
{
    public static Globals Instance;


    public float movePaceVertical;
    public float movePaceHorizontal;

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
    }
}
