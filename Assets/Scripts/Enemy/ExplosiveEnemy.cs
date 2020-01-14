using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class ExplosiveEnemy : MonoBehaviour
{
    private bool hasBeenTouched;
    public float exploadTimer;

    [Range(-2, 2)]
    public int xPos;
    [Range(-2, 2)]
    public int yPos;

    private void Start()
    {
        //Position the Enemy on a platform.
        gameObject.transform.position = new Vector3( xPos * Globals.Instance.movePaceHorizontal, yPos * Globals.Instance.movePaceVertical , 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.CompareTag("Player") && !hasBeenTouched)
        {
            hasBeenTouched = true;
            Invoke("Expload", exploadTimer);
            //Timing.RunCoroutine(_StartTimer().CancelWith(gameObject));
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private IEnumerator<float> _StartTimer()
    {
        //Wait for 'X' seconds
        yield return Timing.WaitForSeconds(exploadTimer);
        //Expload
        //Destroy all surrounding platforms(UP, DOWN, LEFT, RIGHT)
        Expload();

    }

    void Expload()
    {
        //Shoots RayCasts in all 4 direction 
        RaycastHit2D[] hits;

        //Raycast Up
        hits = Physics2D.RaycastAll(transform.position, Vector2.up, 2); // verticalSize
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Platform") || hit.collider.CompareTag("Player")) hit.collider.gameObject.GetComponent<IDestroyable>().DestroyObject();
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

    
}
