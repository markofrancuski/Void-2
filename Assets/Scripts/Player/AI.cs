using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : Person, IDestroyable
{

    private int platformUp;
    private int platformUpRight;
    private int platformUpLeft;

    private int platformDown;
    private int platformDownRight;
    private int platformDownLeft;

    private int platformRight;
    private int platformLeft;

    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        gameObject.SetActive(true);
        CheckSurroundingPlatforms();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == PersonState.IDLE && currentState != PersonState.STUNNED) Jump();
    }

    void CheckSurroundingPlatforms()
    {
        RaycastHit2D hit;
        //Up Raycast
        hit = Physics2D.Raycast(transform.position, Vector2.up, 2f, 1 << 8);
        if (!hit) platformUp = 0;
        else platformUp = 1;

        //down Raycast
        hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, 1 << 8);
        if (!hit) platformDown = 0;
        else platformDown = 1;

        //Right Raycast
        hit = Physics2D.Raycast(transform.position, Vector2.right, 2f, 1 << 8);
        if (!hit) platformRight = 0;
        else platformRight = 1;

        //Left Raycast
        hit = Physics2D.Raycast(transform.position, Vector2.left, 2f, 1 << 8);
        if (!hit) platformLeft = 0;
        else platformLeft = 1;

    }

    void Jump()
    {
        Tween.Position(gameObject.transform, gameObject.transform.position + Vector3.up, .5f, 0, Tween.EaseInOutStrong, Tween.LoopType.None, HandleTweenStarted, HandleTweenFinished);
    }

    public void DestroyObject()
    {
        Debug.Log($"AI is dead!");
        gameObject.SetActive(false);
    }


}
