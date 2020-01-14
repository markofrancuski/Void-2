using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : BasePlatform
{


    private void EnablePlatform()
    {
        gameObject.SetActive(true);
    }

    public override void Interact(Person controller)
    {
        //PrintObjectInteracting(controller, "Breakable");
        if (controller.IsFreeFall)
        {
            //controller.AddFirstMove("DOWN");
            gameObject.SetActive(false);
            Invoke("EnablePlatform", 2f);

        }

        //base.Interact(controller);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Interact(collision.GetComponent<Person>());
        }
    }

}
