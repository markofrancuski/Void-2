using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour 
{
    [SerializeField] private float timer;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("ActivateBomb", timer);
    }

    void ActivateBomb()
    {
        //Shoots RayCasts in all 4 direction 
        RaycastHit2D[] hits;

        //Raycast Up
        hits = Physics2D.RaycastAll(transform.position, Vector2.up, 2); // verticalSize
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Platform") || hit.collider.CompareTag("Player") ) hit.collider.gameObject.GetComponent<IDestroyable>().DestroyObject();
        }

        hits = Physics2D.RaycastAll(transform.position, Vector2.down, 2.3f); // verticalSize + 0,3f
        //Raycast Down
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Platform") || hit.collider.CompareTag("Player")) hit.collider.gameObject.GetComponent<IDestroyable>().DestroyObject();
        }

        hits = Physics2D.RaycastAll(transform.position + new Vector3(0f, -0.4f, 0f), Vector2.right, 1.125f); //horizontalSize
        //Raycast Right
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Platform") || hit.collider.CompareTag("Player")) hit.collider.gameObject.GetComponent<IDestroyable>().DestroyObject();
        }

        hits = Physics2D.RaycastAll(transform.position + new Vector3(0f, -0.4f, 0f), Vector2.left, 1.125f); //horizontalSize
        //Raycast Left
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Platform") || hit.collider.CompareTag("Player")) hit.collider.gameObject.GetComponent<IDestroyable>().DestroyObject();
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    /*void Update()
    {
        Debug.DrawRay(transform.position, Vector2.up * 2, Color.black);
        Debug.DrawRay(transform.position, Vector2.down * 2.3f, Color.black);
        Debug.DrawRay(transform.position + new Vector3(0f, -0.4f, 0f), Vector2.right * 1.125f, Color.black);
        Debug.DrawRay(transform.position + new Vector3(0f, -0.4f, 0f), Vector2.left * 1.125f, Color.black);
        
    }
    */

}
