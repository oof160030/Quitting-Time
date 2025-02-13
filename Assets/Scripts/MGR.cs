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
    /// <summary>
    /// 0: Prototype Bullet
    /// 1: Rifle Bullet
    /// 2: Shotgun Bullet
    /// 3: Sniper Bullet
    /// 4: Dragon Sub-Bullet
    /// 5: Goat Glare Bullet
    /// </summary>
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
    public CanvasGroup CG_Start, CG_Help, CG_Choose, CG_Results, CG_Talk;
    public TextMeshProUGUI TXT_EnemyCounter, TXT_LifeCounter;

    private Upgrade Choice1, Choice2, Choice3;
    public Upgrade_Data[] weaponUpgrades;

    //Enemy tracker
    public HashSet<Enemy_SCR> Enemies;

    private void Awake()
    {
        if(SMGR != this && SMGR != null)
            Destroy(gameObject);
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
                    //On Round 4, generate one upgrade option from each category
                    if(roundCounter == 4)
                    {
                        //Generate upgrade choices
                        Choice1 = (Upgrade)Random.Range(0, 4);
                        Choice2 = (Upgrade)Random.Range(4, 8);
                        Choice3 = (Upgrade)Random.Range(8, 12);

                        //Update the buttons
                        PB_1.UpdateButtons(weaponUpgrades[(int)Choice1]);
                        PB_2.UpdateButtons(weaponUpgrades[(int)Choice2]);
                        PB_3.UpdateButtons(weaponUpgrades[(int)Choice3]);

                        //Switch State
                        ChangeState(State.CHOOSE);
                    }
                    else if(roundCounter == 7)
                    {
                        //Generate upgrade choices for the other kinds of upgrades (WIP)
                        if(player.Fire_Upgrade == (Upgrade)12)
                        {
                            int excluded = Random.Range(0, 4);
                            Choice1 = (Upgrade)(excluded % 4);
                            Choice2 = (Upgrade)((excluded+1) % 4);
                            Choice3 = (Upgrade)((excluded + 2) % 4);
                        }
                        else if(player.Travel_Upgrade == (Upgrade)12)
                        {
                            int excluded = Random.Range(0, 4);
                            Choice1 = (Upgrade)(4+ (excluded % 4));
                            Choice2 = (Upgrade)(4 + ((excluded + 1) % 4));
                            Choice3 = (Upgrade)(4 + ((excluded + 2) % 4));
                        }
                        else if (player.Hit_Upgrade == (Upgrade)12)
                        {
                            int excluded = Random.Range(0, 4);
                            Choice1 = (Upgrade)(8 + (excluded % 4));
                            Choice2 = (Upgrade)(8 + ((excluded + 1) % 4));
                            Choice3 = (Upgrade)(8 + ((excluded + 2) % 4));
                        }

                        //Update the buttons
                        PB_1.UpdateButtons(weaponUpgrades[(int)Choice1]);
                        PB_2.UpdateButtons(weaponUpgrades[(int)Choice2]);
                        PB_3.UpdateButtons(weaponUpgrades[(int)Choice3]);

                        //Switch State
                        ChangeState(State.CHOOSE);
                    }
                    else if (roundCounter == 10)
                    {
                        //Generate upgrade choices for the other kinds of upgrades (WIP)
                        if (player.Fire_Upgrade != (Upgrade)12)
                        {
                            Choice1 = (Upgrade)Random.Range(4, 8);
                            Choice2 = (Upgrade)Random.Range(8, 12);
                            Choice3 = (Upgrade)Random.Range(4, 12);
                            while (Choice3 == Choice1 || Choice3 == Choice2)
                                Choice3 = (Upgrade)Random.Range(4, 12);
                        }
                        else if (player.Travel_Upgrade != (Upgrade)12)
                        {
                            Choice1 = (Upgrade)Random.Range(0, 4);
                            Choice2 = (Upgrade)Random.Range(8, 12);
                            Choice3 = (Upgrade)(Random.Range(0, 4) + (8 * Random.Range(0, 2)));
                            while (Choice3 == Choice1 || Choice3 == Choice2)
                                Choice3 = (Upgrade)(Random.Range(0, 4) + (8 * Random.Range(0, 2)));
                        }
                        else if (player.Hit_Upgrade != (Upgrade)12)
                        {
                            Choice1 = (Upgrade)Random.Range(0, 4);
                            Choice2 = (Upgrade)Random.Range(4, 8);
                            Choice3 = (Upgrade)Random.Range(0, 8);
                            while (Choice3 == Choice1 || Choice3 == Choice2)
                                Choice3 = (Upgrade)Random.Range(0, 8);
                        }

                        //Update the buttons
                        PB_1.UpdateButtons(weaponUpgrades[(int)Choice1]);
                        PB_2.UpdateButtons(weaponUpgrades[(int)Choice2]);
                        PB_3.UpdateButtons(weaponUpgrades[(int)Choice3]);

                        //Switch State
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
            case State.RESULTS:
                //Do nothing until the player clicks the retry button

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
                if (newState == State.START)
                {
                    //Hide help menu & show start menu again
                    SetCGActive(CG_Start, true);
                    SetCGActive(CG_Help, false);

                    //Swap state
                    gameState = State.START;
                }
                break;
            case State.GAME:
                if (newState == State.RESULTS)
                {
                    //Display the Results Screen
                    SetCGActive(CG_Results, true);

                    //Swap state
                    gameState = State.RESULTS;
                }
                else if (newState == State.CHOOSE)
                {
                    //Display the Results Screen
                    SetCGActive(CG_Choose, true);

                    player.playing = false;

                    //Swap state
                    gameState = State.CHOOSE;
                }
                break;
            case State.CHOOSE:
                //New state should be "Game" - set up next wave of enemies based on round count.
                if (newState == State.GAME)
                {
                    //Hide help menu - bring up weapon select menu
                    SetCGActive(CG_Choose, false);

                    //Player has control again
                    player.playing = true;

                    //Start spawning enemies
                    //Set enemy counter to the number of enemies to spawn
                    //Then give spawning instructions to the spawners
                    StartRound();

                    //Swap state
                    gameState = State.GAME;
                }
                break;
            case State.RESULTS:
                //Moving from gameplay to lobby scene
                if (newState == State.LOADING)
                {
                    //Hide the Results Screen
                    SetCGActive(CG_Results, false); 
                    
                    //Load the islands scene again
                    StartCoroutine(LoadNewScene(0));

                    //Swap state
                    gameState = State.LOADING;
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
                //If loading gameplay, initialize gameplay upon loading in
                else if (newState == State.LOBBY)
                {
                    //Enable Player
                    if(player_m != null)
                        player_m.playing = true;

                    //Swap state
                    gameState = State.LOBBY;
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
                spawners[5].StartSpawning(enemy2, 4, 0.7f);
                spawners[6].StartSpawning(enemy2, 4, 0.7f);
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
                enemyCounter = 32;
                spawners[0].StartSpawning(enemy1, 4, enemy2, 4, 0.7f);
                spawners[1].StartSpawning(enemy1, 4, enemy2, 4, 0.7f);
                spawners[5].StartSpawning(enemy1, 4, enemy2, 4, 0.7f);
                spawners[6].StartSpawning(enemy1, 4, enemy2, 4, 0.7f);
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

    public void ReceiveSpanwers(Spawner_SCR[] X)
    {
        spawners = X;
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
        TXT_EnemyCounter.text = "Enemies: " + enemyCounter;
    }

    public void UpdateHP(int H)
    {
        TXT_LifeCounter.text = "Health: " + H;
    }

    public void PlayerDied()
    {
        ChangeState(State.RESULTS);
    }

    public void ConnectMini(Player_Mini PM)
    {
        player_m = PM;

        if (gameState == State.LOBBY)
            PM.playing = true;
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
                    player.SetPlayerStats(WeaponType.TWIN, bulletREF[2], (1.0f/9.0f));
                    break;
                case 2:
                    player.SetPlayerStats(WeaponType.MID, bulletREF[1], (1.0f/12.0f));
                    break;
                case 3:
                    player.SetPlayerStats(WeaponType.LONG, bulletREF[3], (1.0f/3.0f));
                    break;
            }
            //Free the player to move
            Debug.Log("Button pressed");
            ChangeState(State.GAME);
        }
        //Otherwise, choice represents upgrades - choose based on MGR stored upgrades, then switch state
        else
        {
            switch (C)
            {
                case 1:
                    player.ReceiveUpgrade(Choice1);
                    break;
                case 2:
                    player.ReceiveUpgrade(Choice2);
                    break;
                case 3:
                    player.ReceiveUpgrade(Choice3);
                    break;
            }
            Debug.Log("Button pressed??");
            ChangeState(State.GAME);
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

    public void B_ResultsPress()
    {
        MoveScenes(0);
    }
}
