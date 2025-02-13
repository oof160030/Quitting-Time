using UnityEngine;

public class Enemy_SCR : MonoBehaviour
{
    public bool vulnerable;
    public float health;
    public int touchDamage;

    public float moveSpeed;
    public bool accellerates; // false = linear movement, true = acceleration
    public float accell_Rate;

    protected MGR SMGR;
    public GameObject e_bullet;
    protected Rigidbody2D RB2;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        RB2 = gameObject.GetComponent<Rigidbody2D>();
        SMGR = MGR.SMGR;
        SMGR.Enemies.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void Movement()
    {
        //Do whatever basic movement is required
    }

    protected virtual void DirectAttack(Player_SCR P)
    {
        P.Damage();
    }

    protected virtual void RangeAttack()
    {

    }

    public virtual void Damage(float D)
    {
        if (vulnerable && health > 0)
        {
            health -= (float)System.Math.Round(D, 2);
            if (health <= 0)
                Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
        SMGR.Enemies.Remove(this);
        SMGR.EnemyDown();
        //Plus other inherited effects
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            DirectAttack(collision.gameObject.GetComponent<Player_SCR>());
        }
    }
}
