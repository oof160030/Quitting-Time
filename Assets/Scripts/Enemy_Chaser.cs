using UnityEngine;

public class Enemy_Chaser : Enemy_SCR
{
    //Enemy type that moves towards the player
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        //every frame, move towards the player at movement speed;
        Movement();
    }

    protected override void Movement()
    {
        base.Movement();
        //Set velocity towards player
        if(accellerates == false)
            RB2.linearVelocity = (SMGR.player.transform.position - transform.position).normalized * moveSpeed;
        else
        {
            RB2.linearVelocity = RB2.linearVelocity + (Vector2)((SMGR.player.transform.position - transform.position).normalized * (accell_Rate * Time.deltaTime));
            RB2.linearVelocity = RB2.linearVelocity.normalized * Mathf.Clamp(RB2.linearVelocity.magnitude, 0, moveSpeed);
        }
            
    }
}
