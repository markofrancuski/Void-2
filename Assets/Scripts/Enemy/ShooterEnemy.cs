using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShootDirection
{
    UP, DOWN, RIGHT, LEFT
}
public class ShooterEnemy : MonoBehaviour
{

    [Range(-2, 2)]
    public int horizontalGrid;
    [Range(-2, 2)]
    public int verticalGrid;


    [Header("Shoot Settings")]
    public ShootDirection projectileDirection;
    [Range(0.1f, 3f)]
    public float fireRate;
    private float tempFireRate;
    
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Quaternion angle;

    [SerializeField] private Vector2 pos = Vector2.zero;

    public bool active;
    // Start is called before the first frame update
    void Start()
    {

        if(projectileDirection == ShootDirection.DOWN || projectileDirection == ShootDirection.UP)
        {
            //Modify X(Horizontal)
            pos.x = Globals.Instance.movePaceHorizontal * horizontalGrid;

            if (projectileDirection == ShootDirection.DOWN) pos.y = 5;
            else pos.y = -5;

            gameObject.transform.position = pos;
        }

        if (projectileDirection == ShootDirection.RIGHT || projectileDirection == ShootDirection.LEFT)
        {
            //Modify Y(Vertical)
            pos.y = Globals.Instance.movePaceVertical * verticalGrid;

            if (projectileDirection == ShootDirection.RIGHT) pos.x = -2.8f;
            else pos.x = 2.8f;

            gameObject.transform.position = pos;
        }

        tempFireRate = fireRate;
        RotateProjectile();


    }

    // Update is called once per frame
    void Update()
    {
        if (!InputManager.Instance.isControllable || !active) return;

        if (tempFireRate <= 0) FireUpProjectile();
        else tempFireRate -= Time.deltaTime;
        
    }

    private void FireUpProjectile()
    {
        GameObject GO = Instantiate(projectilePrefab) as GameObject;
        GO.transform.position = transform.position;
        GO.transform.rotation = angle;

        GO.GetComponent<Projectile>().SetUpProjectile(2, GetMoveDirection());
        tempFireRate = fireRate;

    }

    private void RotateProjectile()
    {
        switch (projectileDirection)
        {
            case ShootDirection.UP:
                angle = Quaternion.Euler(new Vector3(0, 0, 90));
                break;
            case ShootDirection.DOWN:
                angle = Quaternion.Euler(new Vector3(0, 0, -90));
                break;
            case ShootDirection.RIGHT:
                angle = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            case ShootDirection.LEFT:
                angle = Quaternion.Euler(new Vector3(0, 0, -180));
                break;
        }
        
    }

    Vector2 GetMoveDirection()
    {
        switch (projectileDirection)
        {
            case ShootDirection.UP: return Vector2.up;
            case ShootDirection.DOWN: return Vector2.down;
            case ShootDirection.RIGHT: return Vector2.right;
            case ShootDirection.LEFT: return Vector2.left;
            default: return Vector2.right;

        }
    }
}
