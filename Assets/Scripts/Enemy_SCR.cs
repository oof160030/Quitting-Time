using UnityEngine;

public class Enemy_SCR : MonoBehaviour
{
    public bool vulnerable;
    public int health;
    public int touchDamage;

    public float moveSpeed;
    public bool accellerates; // false = linear movement, true = acceleration
    public float accell_Rate;

    protected MGR SMGR;
    public GameObject e_bullet;
    protected Rigidbody2D RB2;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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

    }

    protected virtual void RangeAttack()
    {

    }

    public virtual void Damage(int D)
    {
        if (vulnerable)
        {
            health -= D;
            if (health <= 0)
                Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
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
