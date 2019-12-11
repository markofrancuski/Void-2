using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rigidBody;

    [SerializeField] private Vector2 direction;

    public int Team;

    public void SetUpProjectile(int team)
    {
        Team = team;
    }

    private void FixedUpdate()
    {
        rigidBody.velocity = direction * speed * Time.deltaTime;

    }

    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}
