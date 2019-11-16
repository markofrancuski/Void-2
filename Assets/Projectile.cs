using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rigidBody;

    private void FixedUpdate()
    {
        rigidBody.velocity = Vector2.right * speed * Time.deltaTime;

    }

    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}
