using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

public enum State { START, HELP, LOBBY, GAME, CHOOSE, RESULTS, END, LOADING }
public class MGR : MonoBehaviour
{
    static public MGR SMGR;
    [SerializeField] private State gameState;

    //Input management
    private int xIn, yIn;
    public Camera Cam;
    private Vector2 mousePos;

    public GameObject[] bulletREF;
    //public GameObject spawnerPF;
    public Spawner_SCR[] spawners;

    public GameObject enemy1, enemy2;
    private int enemyCounter;

    //Stores player during gameplay
    public Player_SCR player;
    public Player_Mini player_m;
    public int roundCounter;

    //UI & Buttons
    public Button BT_Start, BT_Start2, BT_Help, BT_Choose1, BT_Choose2, BT_Choose3;
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
        BT_Start.onClick.AddListener(() => B_StartPress(1));
        BT_Start2.onClick.AddListener(() => B_StartPress(0));
        BT_Help.onClick.AddListener(B_HelpPress);

        BT_Choose1.onClick.AddListener(() => B_Choice(1));
        BT_Choose2.onClick.AddListener(() => B_Choice(2));
        BT_Choose3.onClick.AddListener(() => B_Choice(3));

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
                //

                break;
            case State.LOBBY:
                //

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
                else if (newState == State.LOBBY)
                {
                    //Hide start menu & start the game
                    SetCGActive(CG_Start, false);

                    //Enable Player
                    player_m.playing = true;

                    //Swap state
                    gameState = State.LOBBY;
                }
                //Maybe include straight to game option?
                break;
            case State.LOBBY:
                //Moving from lobby to gameplay scene
                if (newState == State.LOADING)
                {
                    StartCoroutine(LoadNewScene(1));

                    //Swap state
                    gameState = State.LOADING;
                }
                break;
            case State.HELP:
                //Hide help menu & show start menu again
                SetCGActive(CG_Start, true);
                SetCGActive(CG_Help, false);

                //Swap state
                gameState = State.START;
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
            case State.LOADING:
                //If loading gameplay, initialize gameplay upon loading in
                if (newState == State.CHOOSE)
                {
                    //Show choices
                    SetCGActive(CG_Choose, true);

                    //Initialize Gameplay
                    NewGame();

                    //Swap state
                    gameState = State.CHOOSE;
                }
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

    public void MoveScenes(int X)
    {
        //If X = 1, we want to move to the gameplay scene (from the lobby)
        if (X == 1)
            ChangeState(State.LOADING);
        //else if X == 0, we want to go to the islands scene (from gameplay, maybe from end too)
        //Those cases are handled differently, but the change state logic will differentiate them!
        else if (X == 0)
            ChangeState(State.LOADING);
        //But if X == 2, we want to go to the end (also from gameplay)
        //HANDLE THIS TRANSITION HERE! Start the load scene from here.
    }

    public void AddInvocation(string X, UnityEvent Y)
    {
        switch (X)
        {
            case "TO_TEMPLE":
                Y.RemoveAllListeners();
                Y.AddListener(() => MoveScenes(1));
                break;
        }
    }

    public void EnemyDown()
    {
        enemyCounter--;
    }

    private IEnumerator LoadNewScene(int X)
    {
        //0 = load islands; 1 = load game scene; 2 = load end scene

        int loadOp = X;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(X);

        while(!asyncLoad.isDone)
        {
            yield return null;
        }

        if (loadOp == 1)
            ChangeState(State.CHOOSE);
        else if(loadOp == 0)
            ChangeState(State.LOBBY);
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
    
    public void B_StartPress(int X)
    {
        if (X == 0)
            ChangeState(State.HELP);
        else if (X == 1)
            ChangeState(State.LOBBY);
    }

    public void B_HelpPress()
    {
        ChangeState(State.START);
    }
}
