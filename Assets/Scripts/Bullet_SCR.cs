using UnityEngine;
using System.Collections;

public class Bullet_SCR : MonoBehaviour
{
    public bool friendly; //If friendly, damages enemies (otherwise damages player)
    public int b_damage, b_toughness;
    private float b_damageMod;
    public float b_speed, b_lifespan, b_rampMin, b_rampMax;
    private float birthTime;

    //Dog Stuff
    private bool homing; private Enemy_SCR target;

    //Snake Stuff
    private bool snake;

    Coroutine B_Timer;

    private Rigidbody2D RB2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void INIT(Vector3 DIR)
    {
        //Set bullet life
        B_Timer = StartCoroutine(B_Lifespan(b_lifespan));

        RB2 = GetComponent<Rigidbody2D>();

        //Set bullet direction
        RB2.linearVelocity = DIR * b_speed;

        birthTime = Time.time;

        b_damageMod = 1;
    }

    public void FX_Lucky()
    {
        b_damageMod = 7;
        //Add visual effect
    }

    public void FX_GoatMod(float MD)
    {
        b_damageMod = MD;
    }

    public void FX_Ox()
    {
        b_toughness = 2;
    }

    public void FX_Hound()
    {
        homing = true;
        StartCoroutine(CheckTarget());
    }

    // Update is called once per frame
    void Update()
    {
        //If homing
    }

    IEnumerator B_Lifespan(float life)
    {
        yield return new WaitForSeconds(life);
        B_Death(0);
        //And call on death effect (miss modifier)
    }

    IEnumerator CheckTarget()
    {
        while(homing)
        {
            //If you have no target, find one
            if(target == null && MGR.SMGR.Enemies.Count != 0)
            {
                float dist = 4;
                foreach(var X in MGR.SMGR.Enemies)
                {
                    if((X.transform.position - transform.position).magnitude < dist)
                    {
                        dist = (X.transform.position - transform.position).magnitude;
                        target = X;
                    }
                }
            }
            //Now, home in on your target
            if(target != null)
            {
                RB2.linearVelocity = (target.transform.position - transform.position).normalized * b_speed;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator SpawnSwarm()
    {
        yield return new WaitForSeconds(0.2f);
    }

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If colliding with a wall, destroy the projectile
        if (collision.gameObject.CompareTag("Wall"))
        {
            B_Death(1);
        }
        else if (collision.gameObject.CompareTag("Enemy") && friendly)
        {
            //Call enemy damage script
            Enemy_SCR temp_E = collision.gameObject.GetComponent<Enemy_SCR>();
            temp_E.Damage(b_damage * CalculateRamping() * b_damageMod);
            //Then initiate bullet death (with "hit enemy" modifier)
            B_Death(2);
        }
    }
    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If colliding with a wall, destroy the projectile
        if (collision.gameObject.CompareTag("Wall"))
        {
            B_Death(1);
        }
        else if (collision.gameObject.CompareTag("Enemy") && friendly)
        {
            //Call enemy damage script
            Enemy_SCR temp_E = collision.gameObject.GetComponent<Enemy_SCR>();

            //Ignore enemies with negative health (already dead)
            if(temp_E.health > 0)
            {
                //Note - snake effect applied in ramping calculation
                temp_E.Damage(b_damage * CalculateRamping() * b_damageMod);
                //If bullet toughness is expended, destroy the bullet.
                if (b_toughness > 0)
                    b_toughness--;
                else
                    B_Death(2);
            } 
        }
    }

    private float CalculateRamping()
    {
        //Calculate elapsed time as proportion of lifespan
        float E = (Time.time - birthTime) / b_lifespan;

        //Calculate lerp based on bullet ramping and travel time
        float F = Mathf.Lerp(b_rampMin, b_rampMax, E);

        // [[SNAKE]] - If ratio is greater than 0.9, multiple effect by 2
        if (snake && E >= 0.9f)
            F = F * 2.0f;

        //Return ramping modifier multiplied by porportional life
        return F;
    }

    private void B_Death(int hitCode)
    {
        //0 = miss, 1 = wall, 2 = enemy, -1 = player
        Destroy(gameObject);
        StopAllCoroutines();
    }
}
