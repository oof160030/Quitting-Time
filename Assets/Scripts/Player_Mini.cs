using UnityEngine;

public class Player_Mini : MonoBehaviour
{
    public bool playing;
    private MGR SMGR;

    private Rigidbody2D RB2;
    public float moveSpeed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playing = false;

        RB2 = GetComponent<Rigidbody2D>();
        SMGR = MGR.SMGR;

        SMGR.player_m = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(playing)
        {
            //Each frame, get player input and move based on input.
            RB2.linearVelocity = SMGR.GetInputDir() * moveSpeed;
        }
    }
}
