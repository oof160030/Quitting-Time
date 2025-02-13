using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public enum WeaponType { TWIN, MID, LONG }
public enum Upgrade { DRAGON, RABBIT, HORSE, GOAT, OX, RAT, DOG, SNAKE, PIG, MONKEY, ROOSTER, TIGER, NONE }
public class Player_SCR : MonoBehaviour
{
    public bool playing;
    private MGR SMGR;
    public GameObject fist;
    
    //Movement
    private Rigidbody2D RB2;
    public float moveSpeed, moveSpeedModifier;

    //Gunplay
    private bool onCooldown;
    public float cooldown, cooldownModifier;
    public WeaponType weapon;
    private GameObject base_Bullet;

    //Upgrades
    public Upgrade Fire_Upgrade, Travel_Upgrade, Hit_Upgrade;
    private bool dragonCooldown;

    //Stats
    public int health;
    private bool iFrames;
    
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

        Fire_Upgrade = Upgrade.NONE;
        Travel_Upgrade = Upgrade.NONE;
        Hit_Upgrade = Upgrade.NONE;
    }

    // Update is called once per frame
    void Update()
    {
        if (playing)
        {
            //Each frame, get player input and move based on input.
            RB2.linearVelocity = SMGR.GetInputDir() * moveSpeed * moveSpeedModifier;

            //Draw line between mouse and player, normalize, 
            fist.transform.position = (Vector2)transform.position + (SMGR.GetMousePos() - (Vector2)transform.position).normalized * 1.2f;

            //While holding down fire button, shoot with cooldown
            if (Input.GetMouseButton(0))
            {
                if(!onCooldown)
                {
                    onCooldown = true;
                    //Spawn bullet
                    BasicAttack();

                    // [[HORSE]] - If moving (velocity > 0 & receiving axis input), use shorter cooldown.
                    if(Fire_Upgrade == Upgrade.HORSE && RB2.linearVelocity.magnitude > 0.2f && (SMGR.GetInputDir() != Vector3.zero))
                        StartCoroutine(BulletCooldown(cooldown * cooldownModifier * 0.6f));
                    else
                        StartCoroutine(BulletCooldown(cooldown * cooldownModifier));
                }
                // [[DRAGON]] - also fire an additional bullet at regular intervals while holding button down
                if(Fire_Upgrade == Upgrade.DRAGON && !dragonCooldown)
                {
                    //Get direction to target
                    Vector3 AimDir = (SMGR.GetMousePos() - (Vector2)transform.position).normalized;

                    Vector3 SpawnPos = transform.position - AimDir * 1.2f;

                    Bullet_SCR tempB = Instantiate(SMGR.bulletREF[4], SpawnPos, Quaternion.identity).GetComponent<Bullet_SCR>();
                    tempB.INIT(-AimDir + new Vector3(Random.Range(-0.5f, 0.5f) , Random.Range(-0.3f, 0.5f) , 0));

                    dragonCooldown = true;
                    StartCoroutine(DragonCD());
                }
            }
        }
    }

    private void BasicAttack()
    {
        //Get direction to target
        Vector3 AimDir = (SMGR.GetMousePos() - (Vector2)transform.position).normalized;

        Vector3 SpawnPos = transform.position + AimDir * 1.2f;

        // [[RABBIT]] Check if this bullet is lucky (1/7 chance)
        bool lucky = (Random.Range(0,7) == 0);

        //Spawn bullet (either from lead position or spread, depending on weapon
        if (weapon != WeaponType.TWIN)
        {
            Bullet_SCR tempB = Instantiate(base_Bullet, SpawnPos, Quaternion.identity).GetComponent<Bullet_SCR>();
            tempB.INIT(AimDir);

            if (Fire_Upgrade == Upgrade.RABBIT && lucky)
                tempB.FX_Lucky();

            else if (Travel_Upgrade == Upgrade.DOG)
                tempB.FX_Hound();
        }
        else
        {
            Vector3 S1 = SpawnPos + new Vector3(-AimDir.y, AimDir.x).normalized * 0.5f;
            Vector3 S2 = SpawnPos + new Vector3(AimDir.y, -AimDir.x).normalized * 0.5f;

            Bullet_SCR tempB = Instantiate(base_Bullet, S1, Quaternion.identity).GetComponent<Bullet_SCR>();
            tempB.INIT(AimDir);

            Bullet_SCR tempB2 = Instantiate(base_Bullet, S2, Quaternion.identity).GetComponent<Bullet_SCR>();
            tempB2.INIT(AimDir);

            if (Fire_Upgrade == Upgrade.RABBIT && lucky)
            {
                tempB.FX_Lucky(); tempB2.FX_Lucky();
            }
            else if (Travel_Upgrade == Upgrade.DOG)
            {
                tempB.FX_Hound(); tempB2.FX_Hound();
            }
        }

        // [[GOAT]] - Spawn an additional bullet next to cursor. Bullet damage scales with fire rate.
        if(Fire_Upgrade == Upgrade.GOAT)
        {
            Bullet_SCR tempC = Instantiate(SMGR.bulletREF[5], SMGR.GetMousePos(), Quaternion.identity).GetComponent<Bullet_SCR>();
            tempC.INIT(AimDir);

            if (weapon == WeaponType.TWIN)
                tempC.FX_GoatMod(1.0f / 3.0f);
            else if (weapon == WeaponType.TWIN)
                tempC.FX_GoatMod(1.0f / 4.0f);
        }

    }

    IEnumerator BulletCooldown(float CD)
    {
        yield return new WaitForSeconds(CD);
        onCooldown = false;
    }

    IEnumerator DragonCD()
    {
        yield return new WaitForSeconds(0.08f);
        dragonCooldown = false;
    }

    public void SetPlayerStats(WeaponType WT, GameObject B, float CD)
    {
        weapon = WT;
        base_Bullet = B;
        cooldown = CD;
        cooldownModifier = 1;
        moveSpeedModifier = 1;
        health = 5;
        SMGR.UpdateHP(health);
    }

    public void Damage()
    {
        if(!iFrames && playing)
        {
            health--;
            SMGR.UpdateHP(health);
            if (health > 0)
            {
                iFrames = true;
                StartCoroutine(IFrameTimer(1));
            }
            else
            {
                //Player Dies
                playing = false;
                RB2.linearVelocity = Vector2.zero;
                SMGR.PlayerDied();
            }
        }
    }

    IEnumerator IFrameTimer(float T)
    {
        yield return new WaitForSeconds(T);
        iFrames = false;
    }

    public void ReceiveUpgrade(Upgrade U)
    {
        //Assign upgrade to character slot
        if ((int)U < 4)
            Fire_Upgrade = U;
        else if ((int)U < 8)
            Travel_Upgrade = U;
        else
            Hit_Upgrade = U;

        switch(U)
        {
            case Upgrade.DRAGON:
                //Do Nothing - just check flag when firing
                break;
            case Upgrade.RABBIT:
                //Do Nothing - just check flag when firing
                break;
            case Upgrade.HORSE:
                //Increase movement speed
                moveSpeedModifier = 1.5f;
                //Also check for horse flag and movement when firing
                break;
            case Upgrade.GOAT:
                //Do Nothing - just check flag when firing
                break;
            case Upgrade.OX:
                //Do Nothing - just check flag when firing
                break;
            case Upgrade.RAT:
                //Do Nothing - just check flag when firing
                break;
            case Upgrade.DOG:
                //Do Nothing - just check flag when firing
                break;
            case Upgrade.SNAKE:
                //Do Nothing - just check flag when firing
                break;
            case Upgrade.PIG:
                //Do Nothing - just check flag when firing
                break;
            case Upgrade.MONKEY:
                //Do Nothing - just check flag when firing
                break;
            case Upgrade.ROOSTER:
                //Do Nothing - just check flag when firing
                break;
            case Upgrade.TIGER:
                //Do Nothing - just check flag when firing
                break;
        }
    }
}
