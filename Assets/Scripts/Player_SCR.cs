using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public enum WeaponType { TWIN, MID, LONG }
public class Player_SCR : MonoBehaviour
{
    public bool playing;
    private MGR SMGR;
    public GameObject fist;
    
    //Movement
    private Rigidbody2D RB2;
    public float moveSpeed;

    //Gunplay
    private bool onCooldown;
    public float cooldown, cooldownModifier;
    public WeaponType weapon;
    private GameObject base_Bullet;

    //Stats
    private int health;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playing = false;

        RB2 = GetComponent<Rigidbody2D>();
        SMGR = MGR.SMGR;
        SMGR.player = this;

        onCooldown = false;

        weapon = WeaponType.MID;
        base_Bullet = SMGR.bulletREF[0]; //Change based on the attack type!!
    }

    // Update is called once per frame
    void Update()
    {
        if (playing)
        {
            //Each frame, get player input and move based on input.
            RB2.linearVelocity = SMGR.GetInputDir() * moveSpeed;

            //Draw line between mouse and player, normalize, 
            fist.transform.position = (Vector2)transform.position + (SMGR.GetMousePos() - (Vector2)transform.position).normalized * 1.2f;

            //While holding down fire button, shoot with cooldown
            if (Input.GetMouseButton(0) && !onCooldown)
            {
                onCooldown = true;
                //Spawn bullet
                BasicAttack();

                StartCoroutine(BulletCooldown(cooldown * cooldownModifier));
            }
        }
    }

    private void BasicAttack()
    {
        //Get direction to target
        Vector3 AimDir = (SMGR.GetMousePos() - (Vector2)transform.position).normalized;

        Vector3 SpawnPos = transform.position + AimDir * 1.2f;

        //Spawn bullet (either from lead position or spread, depending on weapon
        if (weapon != WeaponType.TWIN)
        {
            Bullet_SCR tempB = Instantiate(base_Bullet, SpawnPos, Quaternion.identity).GetComponent<Bullet_SCR>();
            tempB.INIT(AimDir);
        }
        else
        {
            Vector3 S1 = SpawnPos + new Vector3(-AimDir.y, AimDir.x).normalized * 0.5f;
            Vector3 S2 = SpawnPos + new Vector3(AimDir.y, -AimDir.x).normalized * 0.5f;

            Bullet_SCR tempB = Instantiate(base_Bullet, S1, Quaternion.identity).GetComponent<Bullet_SCR>();
            tempB.INIT(AimDir);

            tempB = Instantiate(base_Bullet, S2, Quaternion.identity).GetComponent<Bullet_SCR>();
            tempB.INIT(AimDir);
        }
        
    }

    IEnumerator BulletCooldown(float CD)
    {
        yield return new WaitForSeconds(CD);
        onCooldown = false;
    }

    
}
