using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rigidBody;

    [SerializeField] private Vector2 direction;

    private void FixedUpdate()
    {
        rigidBody.velocity = direction * speed * Time.deltaTime;

    }

    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}
