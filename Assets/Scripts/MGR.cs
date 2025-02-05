using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum State { START, HELP, GAME, CHOOSE, END }
public class MGR : MonoBehaviour
{
    static public MGR SMGR;
    private State gameState;

    //Input management
    private int xIn, yIn;
    private Camera Cam;
    private Vector2 mousePos;

    public GameObject[] bulletREF;
    public Spawner_SCR[] spawners;

    public GameObject enemy1, enemy2;
    private int enemyCounter;

    //Stores player during gameplay
    public Player_SCR player;
    public int roundCounter;

    //UI & Buttons
    public Button BT_Start, BT_Help, BT_Choose1, BT_Choose2, BT_Choose3;
    private PaintButtons PB_1, PB_2, PB_3;
    public Sprite Gun1, Gun2, Gun3;
    public CanvasGroup CG_Start, CG_Help, CG_Choose;

    private void Awake()
    {
        if(SMGR != this && SMGR != null)
            Destroy(this);
        else
        {
            SMGR = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState = State.START;

        Cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        mousePos = Vector2.zero;

        //Connect buttons
        BT_Start.onClick.AddListener(B_StartPress);
        BT_Help.onClick.AddListener(B_HelpPress);

        PB_1 = BT_Choose1.gameObject.GetComponent<PaintButtons>();
        PB_2 = BT_Choose2.gameObject.GetComponent<PaintButtons>();
        PB_3 = BT_Choose3.gameObject.GetComponent<PaintButtons>();

        //spawners[0].StartSpawning(enemy1, 5, enemy2, 9, 0.7f);
    }

    // Update is called once per frame
    void Update()
    {
        //Reading Inputs
        xIn = 0 + (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
        yIn = 0 + (Input.GetKey(KeyCode.S) ? -1 : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0);
        if(Cam)
            mousePos = Cam.ScreenToWorldPoint(Input.mousePosition);

        switch(gameState)
        {
            case State.START:
                //Do nothing until the player clicks start

                break;
            case State.HELP:
                //Do nothing until the player clicks start again, moving to gameplay

                break;
            case State.GAME:
                //Wait until enemy count hits zero, then move to next wave
                if(enemyCounter == 0)
                {
                    //Congrats! Round num increases
                    roundCounter++;
                    //On specific rounds, move to choice instead of game again
                    if(roundCounter == 4 || roundCounter == 7 || roundCounter == 10)
                    {
                        ChangeState(State.CHOOSE);
                    }
                    //else, spawn the next wave
                    else
                    {
                        StartRound();
                    }
                }
                break;
            case State.CHOOSE:
                //Do nothing until the player clicks start, or the start option is chosen

                break;
            case State.END:
                //Do nothing until the player clicks start, or the start option is chosen

                break;
        }
    }

    private void ChangeState(State newState)
    {
        //Switch for current state
        switch (gameState)
        {
            case State.START:
                //if new state is "Help," display help menu
                if(newState == State.HELP)
                {
                    //Hide start menu & show help menu
                    SetCGActive(CG_Start, false);
                    SetCGActive(CG_Help, true);

                    //Swap state
                    gameState = State.HELP;
                }
                //Maybe include straight to game option?
                break;
            case State.HELP:
                //If new state is "choose," game is about to start - initialize!
                if (newState == State.CHOOSE)
                {
                    //Hide help menu - bring up weapon select menu
                    SetCGActive(CG_Help, false);
                    SetCGActive(CG_Choose, true);

                    //Initialize Gameplay
                    NewGame();

                    //Swap state
                    gameState = State.CHOOSE;
                }
                break;
            case State.GAME:
                


                break;
            case State.CHOOSE:
                //New state should be "Game" - set up next wave of enemies based on round count.
                if (newState == State.GAME)
                {
                    //Hide help menu - bring up weapon select menu
                    SetCGActive(CG_Choose, false);

                    //Start spawning enemies
                    //Set enemy counter to the number of enemies to spawn
                    //Then give spawning instructions to the spawners
                    StartRound();

                    //Swap state
                    gameState = State.GAME;
                }
                break;
            case State.END:
                //Do nothing until the player clicks start, or the start option is chosen

                break;
        }
    }

    //SETS UP WEAPON CHOICES AT GAME START
    private void NewGame()
    {
        //Select the display image and text for each of the three buttons on CG_CHOOSE (to display weapons)
        PB_1.UpdateButtons("Shotgun", "Close range weapon with twin projectiles. Fires very rapidly; strongest up close.", Gun1);
        PB_2.UpdateButtons("Rifle", "Mid range weapon with decent attack speed. Bullet damage consistent at all ranges.", Gun2);
        PB_3.UpdateButtons("Sniper", "Long range weapon with slow fire rate. Swift projectiles deal more damage far away.", Gun3);

        //Set round to 1 (or 0?), reset the player position
        roundCounter = 1;
        player.transform.position = Vector3.zero;
    }

    private void StartRound()
    {
        //Based on round number, begin spawning enemies
        switch(roundCounter)
        {
            case 1:
                enemyCounter = 16;
                spawners[0].StartSpawning(enemy1, 4, 0.7f);
                spawners[1].StartSpawning(enemy1, 4, 0.7f);
                spawners[5].StartSpawning(enemy1, 4, 0.7f);
                spawners[6].StartSpawning(enemy1, 4, 0.7f);
                break;
            case 2:
                enemyCounter = 16;
                spawners[0].StartSpawning(enemy1, 4, 0.7f);
                spawners[1].StartSpawning(enemy1, 4, 0.7f);
                spawners[5].StartSpawning(enemy1, 4, 0.7f);
                spawners[6].StartSpawning(enemy1, 4, 0.7f);
                break;
            case 3:
                enemyCounter = 32;
                spawners[0].StartSpawning(enemy1, 4, enemy2, 4, 0.7f);
                spawners[1].StartSpawning(enemy1, 4, enemy2, 4, 0.7f);
                spawners[5].StartSpawning(enemy1, 4, enemy2, 4, 0.7f);
                spawners[6].StartSpawning(enemy1, 4, enemy2, 4, 0.7f);
                break;
            case 4:
                enemyCounter = 32;
                spawners[0].StartSpawning(enemy1, 4, enemy2, 4, 0.7f);
                spawners[1].StartSpawning(enemy1, 4, enemy2, 4, 0.7f);
                spawners[5].StartSpawning(enemy1, 4, enemy2, 4, 0.7f);
                spawners[6].StartSpawning(enemy1, 4, enemy2, 4, 0.7f);
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            default:
                break;
        }
    }

    public Vector3 GetInputDir()
    {
        return (new Vector3(xIn, yIn).normalized);
    }

    public Vector2 GetMousePos()
    {
        return mousePos;
    }

    private void SetCGActive(CanvasGroup cg, bool a)
    {
        cg.alpha = a ? 1 : 0;
        cg.interactable = a;
        cg.blocksRaycasts = a;
    }

    public void B_StartPress()
    {
        ChangeState(State.HELP);
    }

    public void B_HelpPress()
    {
        ChangeState(State.CHOOSE);
    }

    public void EnemyDown()
    {
        enemyCounter--;
    }

    public void B_Choice(int C)
    {
        //If on round 1, buttons represent weapons: set the player's weapon and then switch state
        if(roundCounter == 1)
        {
            switch (C)
            {
                case 1:
                    player.weapon = WeaponType.TWIN;
                    break;
                case 2:
                    player.weapon = WeaponType.MID;
                    break;
                case 3:
                    player.weapon = WeaponType.LONG;
                    break;
            }
            //Free the player to move
            Debug.Log("Button pressed");
            player.playing = true;
            ChangeState(State.GAME);
        }
        //Otherwise, choice represents upgrades - choose based on MGR stored upgrades, then switch state
        else
        {
            Debug.Log("Button pressed??");
        }
    }
}
