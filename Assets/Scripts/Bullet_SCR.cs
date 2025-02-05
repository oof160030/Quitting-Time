using UnityEngine;
using System.Collections;

public class Bullet_SCR : MonoBehaviour
{
    public bool friendly; //If friendly, damages enemies (otherwise damages player)
    public int b_damage;
    public float b_speed, b_lifespan, b_toughness, b_ramping;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator B_Lifespan(float life)
    {
        yield return new WaitForSeconds(life);
        B_Death(0);
        //And call on death effect (miss modifier)
    }

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
            temp_E.Damage(b_damage);
            //Then initiate bullet death (with "hit enemy" modifier)
            B_Death(2);
        }
    }

    private void B_Death(int hitCode)
    {
        //0 = miss, 1 = wall, 2 = enemy, -1 = player
        Destroy(gameObject);
    }
}
