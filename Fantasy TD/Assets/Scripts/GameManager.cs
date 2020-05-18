using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [Header("Main Menu")]

    public Camera MenuCamera;

    public GameObject MainMenuPanel, ControlsPanel, CreditsPanel, LoadingPanel;
    public Button StartGameBTN, OpenControlsBTN, OpenCreditsBTN;
    public Button ControlsReturnBTN, CreditsReturnBTN;
    public GameObject MusicCreditsTitle, MusicCredits, ArtCreditsTitle, ArtCredits, ProgrammingCreditsTitle, ProgrammingCredits, GameDesignCreditsTitle, GameDesignCredits;

    bool PlayingCredits = false;

    bool IsLoadingLevel;
    public GameObject LoadingBar;

    [Header("Game Management")]
    public int SpawnCount;
    public bool WaveInProgress;
    public int Wave;
    public float Seconds;

    public bool PlayerHasLost;
    public float Currency;

    public GameObject CurrentUnit;
    public Unit CurrentUnitScript;

    public bool BarracksSelected;
    public GameObject SelectedBarracks;
    public GameObject[] Barracks = new GameObject[4];

    public GameObject Apothecary;

    public GameObject[] SpawnGates;
    public bool SpawnWait;

    [Header("UI")]

    public Canvas UICanvas;
    public Text UICurrency;
    public Text UIGameOver;
    public Text UIViewPoint;
    public Text UIPrompt;

    public GameObject UIBarracksBlocked;
    bool ShowingBarracksBlocked = false;

    public GameObject UnitHealthBar;
    public Text CurrentUnitName;
    public GameObject EnemyHealthBar;
    public Text EnemyUnitName;
    public GameObject LeaderHealthBar;
    public Text LeaderUnitName;
    public GameObject MainTowerHealthBar;
    public GameObject BarracksHealthBar;
    public Text BarracksName;

    public GameObject UICurrentUnit, UIEnemyUnit, UILeadingUnit;

    public Text UISeconds, UIWave;

    [Header("Cameras")]
    public Camera MainCam;
    public Camera[] InCam = new Camera[4];
    public Camera[] OutCam = new Camera[4];

    public int Direction;
    // 0 = North
    // 1 = West
    // 2 = South
    // 3 = East

    [Header("Prefabs")]
    public GameObject KnightPrefab;
    public GameObject ArcherPrefab;
    public GameObject WizardPrefab;
    public GameObject PikemanPrefab;
    public GameObject[] EnemyPrefabs = new GameObject[4];

    // Start is called before the first frame update
    void Start()
    { 
        // Determines which scene is open
        switch (SceneManager.GetActiveScene().name)
        {
            case "Main Menu":
                IsLoadingLevel = false;

                MainMenuPanel.SetActive(true);
                ControlsPanel.SetActive(false);
                CreditsPanel.SetActive(false);
                LoadingPanel.SetActive(false);

                MusicCreditsTitle.SetActive(false);
                MusicCredits.SetActive(false);
                ArtCreditsTitle.SetActive(false);
                ArtCredits.SetActive(false);
                ProgrammingCreditsTitle.SetActive(false);
                ProgrammingCredits.SetActive(false);
                GameDesignCreditsTitle.SetActive(false);
                GameDesignCredits.SetActive(false);
                break;
            case "Gamefield":
                IsLoadingLevel = false;

                // Holds the amount of time which has elapsed
                Seconds = 60.0f;
                WaveInProgress = true;
                // Holds the wave of enemies
                Wave = 1;
                // Determines whether the lose conditions of the game has been met
                PlayerHasLost = false;

                Direction = 0;

                InitiateCameras();

                SelectedBarracks = Barracks[0];

                UIBarracksBlocked.SetActive(false);
                UIPrompt.gameObject.SetActive(false);

                Currency = 1000;
                SpawnGates = GameObject.FindGameObjectsWithTag("Spawn Gate");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Determines which scene is open
        switch (SceneManager.GetActiveScene().name)
        {
            case "Main Menu":
                MoveMenuCamera();
                RunButtons();
                if (CreditsPanel.activeSelf)
                {
                    PlayCredits();
                }
                break;

            case "Gamefield":
                PlayerHasLost = !IsMainTowerStanding();

                if (!PlayerHasLost)
                {
                    Income();
                    SwitchCameras();
                    Click();
                }

                DisplayGameOver();
                DisplayCurrency();
                DisplayViewPointText();
                DisplayCurrentUnitHealthBar();
                DisplayEnemyUnitHealthBar();
                DisplayLeadingUnitHealthBar();
                DisplayMainTowerHealthBar();
                DisplayBarracksHealth();
                DisplayCurrentUnitUI();
                DisplayEnemyUnitUI();
                DisplayLeadingUnitUI();
                DisplayWaves();
                DisplayPrompt();

                ReturnToMenu();

                Waves();

                if (CurrentUnit != null)
                {
                    CurrentUnitScript = GetUnitClass(CurrentUnit);
                }

                // CLEAN THIS UP
                if (Barracks[Direction] != null)
                {
                    SelectedBarracks = Barracks[Direction];
                }

                if (SelectedBarracks != null)
                {
                    SpawnUnits(SelectedBarracks);
                }

                
                break;
        }  
    }

    #region Main Menu

    // Main Menu

    void MoveMenuCamera()
    {
        //POSITION -31.8, 75, 103.7 ROTATION 35, 20, 0
        MenuCamera.transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, 15 * Time.deltaTime);
    }

    IEnumerator MoveMenuCam()
    {
        yield return new WaitForSeconds(1);
    }

    // If buttons are clicked
    void RunButtons()
    {
        StartGameBTN.onClick.AddListener(StartGame);
        OpenControlsBTN.onClick.AddListener(Controls);
        OpenCreditsBTN.onClick.AddListener(Credits);

        ControlsReturnBTN.onClick.AddListener(Return);
        CreditsReturnBTN.onClick.AddListener(Return);
    }

    // Main Menu Panel

    // Begins the game
    void StartGame()
    {
        MainMenuPanel.SetActive(false);
        LoadingPanel.SetActive(true);
        
        StartCoroutine(LoadAsynchronously(1, 2));
    }

    // Loads a level in the background
    IEnumerator LoadAsynchronously (int LevelIndex, int Wait)
    {
        if (IsLoadingLevel == false)
        {
            IsLoadingLevel = true;

            yield return new WaitForSeconds(Wait);

            AsyncOperation Operation = SceneManager.LoadSceneAsync(LevelIndex);

            while (!Operation.isDone)
            {
                Debug.Log("Loading level");
                float Progress = Mathf.Clamp01(Operation.progress / 0.9f);

                LoadingBar.GetComponent<Slider>().value = Progress;

                yield return null;
            }

            if (!Operation.isDone)
            {
                IsLoadingLevel = false;
            }
        }
    }

    // Opens the Controls Panel
    void Controls()
    {
        MainMenuPanel.SetActive(false);
        ControlsPanel.SetActive(true);
    }

    // Opens the Credits Panel
    void Credits()
    {
        MainMenuPanel.SetActive(false);
        CreditsPanel.SetActive(true);
    }

    // Controls Panel



    // Credits Panel

    // Makes the Credits appear        
    void PlayCredits()
    {
        if (!PlayingCredits)
        {
            StartCoroutine(Play());
        }
    }

    IEnumerator Play()
    {
        GameObject[] Titles = {MusicCreditsTitle, ArtCreditsTitle, GameDesignCreditsTitle, ProgrammingCreditsTitle};
        GameObject[] Credits = {MusicCredits, ArtCredits, GameDesignCredits, ProgrammingCredits};

        PlayingCredits = true;

        Color Original = Titles[0].GetComponent<Text>().color;

        for (int x = 0; x < 4; x++)
        {
            Titles[x].GetComponent<Text>().color = Color.clear;
            Credits[x].GetComponent<Text>().color = Color.clear;
            Titles[x].SetActive(true);
            Credits[x].SetActive(true);
            StartCoroutine(FadeTextIn(Titles[x], Original));
            StartCoroutine(FadeTextIn(Credits[x], Original));
            yield return new WaitForSeconds(5);
            StartCoroutine(FadeTextOut(Titles[x], Original));
            StartCoroutine(FadeTextOut(Credits[x], Original));
            yield return new WaitForSeconds(3);
            Titles[x].SetActive(false);
            Credits[x].SetActive(false);
            Titles[x].GetComponent<Text>().color = Original;
            Credits[x].GetComponent<Text>().color = Original;
            if (x == 3)
            {
                PlayingCredits = false;
            }
        }
    }

    IEnumerator FadeTextIn(GameObject Text, Color Original)
    {
        for (float t = 0.01f; t < 1; t += Time.deltaTime)
        {
            Text.GetComponent<Text>().color = Color.Lerp(Color.clear, Original, Mathf.Min(1, t / 1));
            yield return null;
        }
    }

    IEnumerator FadeTextOut(GameObject Text, Color Original)
    {

        for (float t = 0.01f; t < 1; t += Time.deltaTime)
        {
            Text.GetComponent<Text>().color = Color.Lerp(Original, Color.clear, Mathf.Min(1, t / 1));
            yield return null;
        }
    }

    // Return Buttons

    // Closes all panels other than the Main Menu
    void Return()
    {
        if (ControlsPanel.activeSelf)
        {
            ControlsPanel.SetActive(false);
        }
        if (CreditsPanel.activeSelf)
        {
            CreditsPanel.SetActive(false);
        }

        MainMenuPanel.SetActive(true);
    }

    #endregion

    #region Game Running

    bool IsMainTowerStanding()
    {
        if (GameObject.Find("Main Tower") != null)
        {
            Debug.Log("Tower Standing");
            return true;    
        }
        else
        {
            Debug.Log("Tower Gone");
            return false;
        }
    }

    void Income()
    {
        // Only increases currency if it is less than the max of 2000
        if (Currency < 2000)
        {
            // Currency increases faster while more Income buildings are standing and stops completely when there are none
            GameObject[] IncomeBuildings = GameObject.FindGameObjectsWithTag("Income");
            Currency += (IncomeBuildings.Length * 0.5f) * Time.deltaTime;
        }
        // Sets Currency back to 2000 if it is exceeded
        else if (Currency > 2000)
        {
            Currency = 2000;
        }
    }

    void Waves()
    {
        if ((int)Seconds == 0)
        {
            Seconds = 61;
            Wave++;
            WaveInProgress = true;
        }

        if (WaveInProgress)
        {
            UISeconds.text = "Next wave coming soon.";
            if (!SpawnWait)
            {
                float RandomEnemy = Random.Range(0, EnemyPrefabs.Length);

                float RandomSpawn = Random.Range(0, SpawnGates.Length - 1);

                StartCoroutine(SpawnEnemies((int)RandomEnemy, (int)RandomSpawn));
            }
        }
        else
        {
            Seconds -= Time.deltaTime;
            UISeconds.text = (int)Seconds + " seconds until next wave...";
        }

        UIWave.text = "WAVE " + Wave;
    }

    #endregion

    #region Cameras

    void InitiateCameras()
    {
        for (int x = 0; x < 4; x++)
        {
            OutCam[x].enabled = false;
        }
        for (int x = 0; x < 4; x++)
        {
            InCam[x].enabled = false;
        }

        MainCam = InCam[0];
        MainCam.enabled = true;
    }
    
    void SwitchCameras()
    {       
        if (Input.GetKeyDown(KeyCode.A))
        {
            Direction--;
            if (Direction < 0)
            {
                Direction = 3;
            }

            MainCam.enabled = false;
            MainCam = InCam[Direction];
            MainCam.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Direction++;
            if (Direction > 3)
            {
                Direction = 0;
            }

            MainCam.enabled = false;
            MainCam = InCam[Direction];
            MainCam.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            MainCam.enabled = false;
            MainCam = OutCam[Direction];
            MainCam.enabled = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            MainCam.enabled = false;
            MainCam = InCam[Direction];
            MainCam.enabled = true;
        }

    }

    #endregion

    #region UI

    void DisplayCurrency()
    {
        if (!PlayerHasLost)
        {
            string Display = "Gold: " + ((int)Currency).ToString();
            UICurrency.text = Display;
        }
        else
        {
            UICurrency.enabled = false;
        }
    }
        
    void DisplayGameOver()
    {
        if (PlayerHasLost)
        {
            UIGameOver.enabled = true;
        }
        else
        {
            UIGameOver.enabled = false;
        }
    }

    void DisplayViewPointText()
    {
        if (!PlayerHasLost)
        {
            if (MainCam == InCam[0])
            {
                UIViewPoint.text = "North Gate";
            }
            else if (MainCam == InCam[1])
            {
                UIViewPoint.text = "West Gate";
            }
            else if (MainCam == InCam[2])
            {
                UIViewPoint.text = "South Gate";
            }
            else if (MainCam == InCam[3])
            {
                UIViewPoint.text = "East Gate";
            }
        }
        else
        {
            UIViewPoint.gameObject.SetActive(false);
        }
    }

    void DisplayCurrentUnitHealthBar()
    {
        if (CurrentUnit != null)
        {
            if (CurrentUnit.GetComponent<F_Knight>() != null)
            {
                CurrentUnitName.text = "KNIGHT";
            }
            else if (CurrentUnit.GetComponent<F_Archer>() != null)
            {
                CurrentUnitName.text = "ARCHER";
            }
            else if (CurrentUnit.GetComponent<F_Pikeman>() != null)
            {
                CurrentUnitName.text = "PIKEMAN";
            }
            else if (CurrentUnit.GetComponent<F_Wizard>() != null)
            {
                CurrentUnitName.text = "WIZARD";
            }

            UnitHealthBar.SetActive(true);
            UnitHealthBar.GetComponent<Slider>().minValue = 0;
            UnitHealthBar.GetComponent<Slider>().maxValue = GetUnitClass(CurrentUnit).MaxHealth;
            UnitHealthBar.GetComponent<Slider>().value = GetUnitClass(CurrentUnit).Health;
        }
        else
        {
            UnitHealthBar.gameObject.SetActive(false);
        }
    }

    void DisplayEnemyUnitHealthBar()
    {
        if (CurrentUnit != null)
        {
            Unit CurrentUnitScript = GetUnitClass(CurrentUnit);

            if (CurrentUnitScript.AttackTarget != null)
            {
                Unit EnemyScript = GetUnitClass(CurrentUnitScript.AttackTarget);

                if (CurrentUnitScript.AttackTarget.GetComponent<E_Knight>() != null)
                {
                    EnemyUnitName.text = "ENEMY KNIGHT";
                }
                else if (CurrentUnitScript.AttackTarget.GetComponent<E_Archer>() != null)
                {
                    EnemyUnitName.text = "ENEMY ARCHER";
                }
                else if (CurrentUnitScript.AttackTarget.GetComponent<E_Pikeman>() != null)
                {
                    EnemyUnitName.text = "ENEMY PIKEMAN";
                }
                else if (CurrentUnitScript.AttackTarget.GetComponent<E_Wizard>() != null)
                {
                    EnemyUnitName.text = "ENEMY WIZARD";
                }

                EnemyHealthBar.SetActive(true);
                EnemyHealthBar.GetComponent<Slider>().minValue = 0;
                EnemyHealthBar.GetComponent<Slider>().maxValue = EnemyScript.MaxHealth;
                EnemyHealthBar.GetComponent<Slider>().value = EnemyScript.Health;
            }
            else
            {
                EnemyHealthBar.gameObject.SetActive(false);
            }
        }
        else
        {
            EnemyHealthBar.gameObject.SetActive(false);
        }
    }

    void DisplayLeadingUnitHealthBar()
    {
        if (CurrentUnit != null)
        {
            Unit CurrentUnitScript = GetUnitClass(CurrentUnit);

            if (CurrentUnitScript.UnitLeader != null)
            {
                Unit LeaderScript = GetUnitClass(CurrentUnitScript.UnitLeader);

                if (CurrentUnitScript.UnitLeader.GetComponent<F_Knight>() != null)
                {
                    LeaderUnitName.text = "KNIGHT";
                }
                else if (CurrentUnitScript.UnitLeader.GetComponent<F_Archer>() != null)
                {
                    LeaderUnitName.text = "ARCHER";
                }
                else if (CurrentUnitScript.UnitLeader.GetComponent<F_Pikeman>() != null)
                {
                    LeaderUnitName.text = "PIKEMAN";
                }
                else if (CurrentUnitScript.UnitLeader.GetComponent<F_Wizard>() != null)
                {
                    LeaderUnitName.text = "WIZARD";
                }

                LeaderHealthBar.SetActive(true);
                LeaderHealthBar.GetComponent<Slider>().minValue = 0;
                LeaderHealthBar.GetComponent<Slider>().maxValue = LeaderScript.MaxHealth;
                LeaderHealthBar.GetComponent<Slider>().value = LeaderScript.Health;
            }
            else
            {
                LeaderHealthBar.gameObject.SetActive(false);
            }
        }
        else
        {
            LeaderHealthBar.gameObject.SetActive(false);
        }
    }

    void DisplayMainTowerHealthBar()
    {
        GameObject MainTower = GameObject.Find("Main Tower");
        if (MainTower != null)
        {
            MainTowerHealthBar.SetActive(true);
            MainTowerHealthBar.GetComponent<Slider>().minValue = 0;
            MainTowerHealthBar.GetComponent<Slider>().maxValue = 500;
            MainTowerHealthBar.GetComponent<Slider>().value = MainTower.GetComponent<Building>().Health;
        }
        else
        {
            MainTowerHealthBar.SetActive(false);
        }
    }

    void DisplayBarracksHealth()
    {
        GameObject Barracks = null;
        string Name = null;

        switch (Direction)
        {
            //North
            case 0:
                if (GameObject.Find("North Barracks") != null)
                {
                    Barracks = GameObject.Find("North Barracks");
                    Name = "NORTH BARRACKS";
                }
                else
                {
                    Barracks = null;
                    Name = null;
                }
                break;
            // West
            case 1:
                if (GameObject.Find("West Barracks") != null)
                {
                    Barracks = GameObject.Find("West Barracks");
                    Name = "WEST BARRACKS";
                }
                else
                {
                    Barracks = null;
                    Name = null;
                }
                break;
            // South
            case 2:
                if (GameObject.Find("South Barracks") != null)
                {
                    Barracks = GameObject.Find("South Barracks");
                    Name = "SOUTH BARRACKS";
                }
                else
                {
                    Barracks = null;
                    Name = null;
                }
                break;
            // East
            case 3:
                if (GameObject.Find("East Barracks") != null)
                {
                    Barracks = GameObject.Find("East Barracks");
                    Name = "EAST BARRACKS";
                }
                else
                {
                    Barracks = null;
                    Name = null;
                }
                break;
        }

        if (Barracks != null)
        {
            BarracksHealthBar.SetActive(true);
            BarracksHealthBar.GetComponent<Slider>().minValue = 0;
            BarracksHealthBar.GetComponent<Slider>().maxValue = 300;
            BarracksHealthBar.GetComponent<Slider>().value = Barracks.GetComponent<Building>().Health;
            BarracksName.text = Name;
        }
        else
        {
            BarracksHealthBar.SetActive(false);
        }
    }

    IEnumerator BarracksBlocked()
    {
        UIBarracksBlocked.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        UIBarracksBlocked.SetActive(false);
    }

    void DisplayCurrentUnitUI()
    {
        if (CurrentUnit != null)
        {
            UICurrentUnit.SetActive(true);
            UICurrentUnit.transform.position = new Vector3(CurrentUnit.transform.position.x, CurrentUnit.transform.position.y + 0.3f, CurrentUnit.transform.position.z);
            UICurrentUnit.transform.Rotate(0, 0, 50 * Time.deltaTime);
        }
        else
        {
            UICurrentUnit.SetActive(false);
        }
    }

    void DisplayEnemyUnitUI()
    {
        if (CurrentUnit != null)
        {
            if (GetUnitClass(CurrentUnit).AttackTarget != null)
            {
                UIEnemyUnit.SetActive(true);
                GameObject ET = GetUnitClass(CurrentUnit).AttackTarget;
                UIEnemyUnit.transform.position = new Vector3(ET.transform.position.x, ET.transform.position.y + 0.3f, ET.transform.position.z);
                UIEnemyUnit.transform.Rotate(0, 0, 50 * Time.deltaTime);
            }
            else
            {
                UIEnemyUnit.SetActive(false);
            }
        }
        else
        {
            UIEnemyUnit.SetActive(false);
        }
    }

    void DisplayLeadingUnitUI()
    {
        if (CurrentUnit != null)
        {
            if (GetUnitClass(CurrentUnit).UnitLeader != null)
            {
                UILeadingUnit.SetActive(true);
                GameObject LU = GetUnitClass(CurrentUnit).UnitLeader;
                UILeadingUnit.transform.position = new Vector3(LU.transform.position.x, LU.transform.position.y + 0.3f, LU.transform.position.z);
                UILeadingUnit.transform.Rotate(0, 0, 50 * Time.deltaTime);
            }
            else
            {
                UILeadingUnit.SetActive(false);
            }
        }
        else
        {
            UILeadingUnit.SetActive(false);
        }
    }

    void ReturnToMenu()
    {
        if (PlayerHasLost)
        {
            LoadingBar.SetActive(true);
            StartCoroutine(LoadAsynchronously(0, 4));
        }
        else
        {
            LoadingBar.SetActive(false);
        }
    }

    void DisplayWaves()
    {
        if (PlayerHasLost)
        {
            UIWave.gameObject.SetActive(false);
            UISeconds.gameObject.SetActive(false);
        }
        else
        {
            UIWave.gameObject.SetActive(true);
            UISeconds.gameObject.SetActive(true);
        }
    }

    void DisplayPrompt()
    {
        if (!PlayerHasLost)
        {
            RaycastHit ObjectInfo = new RaycastHit();
            bool hit = Physics.Raycast(MainCam.ScreenPointToRay(Input.mousePosition), out ObjectInfo);

            if (hit)
            {
                switch (ObjectInfo.transform.gameObject.tag)
                {
                    case "Player":
                        if (ObjectInfo.transform.gameObject == CurrentUnit)
                        {
                            UIPrompt.gameObject.SetActive(false);
                        }
                        else if (ObjectInfo.transform.gameObject == GetUnitClass(CurrentUnit).UnitLeader)
                        {
                            UIPrompt.gameObject.SetActive(true);
                            UIPrompt.text = "Left Click to select unit.";
                        }
                        else if (ObjectInfo.transform.gameObject != GetUnitClass(CurrentUnit).UnitLeader)
                        {
                            UIPrompt.gameObject.SetActive(true);
                            UIPrompt.text = "Left Click to select unit.\nRight Click to follow unit.";
                        }
                        break;
                    case "Enemy":
                        UIPrompt.gameObject.SetActive(true);
                        UIPrompt.text = "Left Click to target enemy.";
                        break;
                    case "Ground":
                        if (CurrentUnit != false && GetUnitClass(CurrentUnit).UnitLeader != null)
                        {
                            UIPrompt.gameObject.SetActive(true);
                            UIPrompt.text = "Left Click to move unit here.\nRight Click to stop following.";
                        }
                        else if (CurrentUnit != false)
                        {
                            UIPrompt.gameObject.SetActive(true);
                            UIPrompt.text = "Left Click to move unit here.";
                        }
                        break;
                    default:
                        UIPrompt.gameObject.SetActive(false);
                        break;
                }
            }
            else
            {
                UIPrompt.gameObject.SetActive(false);
            }
        }
        else
        {
            UIPrompt.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Enemies

    IEnumerator SpawnEnemies(int RandomEnemy, int RandomSpawn)
    {
        SpawnWait = true;
        GameObject EnemyUnit1 = Instantiate(EnemyPrefabs[RandomEnemy], SpawnGates[RandomSpawn].transform.Find("Unit Spawn Point").transform.position, SpawnGates[RandomSpawn].transform.Find("Unit Spawn Point").transform.rotation);
        SpawnCount++;
        
        if (SpawnCount == Wave * 4)
        {
            Debug.Log("Start count for next wave");
            WaveInProgress = false;
            SpawnCount = 0;
        }
        yield return new WaitForSeconds(2);
        SpawnWait = false;
    }

    #endregion

    void Click()
    {
        // Left Click
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit ObjectInfo = new RaycastHit();
            bool hit = Physics.Raycast(MainCam.ScreenPointToRay(Input.mousePosition), out ObjectInfo);

            if (hit)
            {
                Debug.Log(hit);
                switch (ObjectInfo.transform.gameObject.tag)
                {
                    // Sets the Unit as the current unit (The unit to be controlled)
                    case "Player":
                        Debug.Log("Current Unit is " + ObjectInfo.transform.gameObject.name);
                        CurrentUnit = ObjectInfo.transform.gameObject;

                        BarracksSelected = false;
                        break;

                    // Sets the enemy unit as the current unit's attack target
                    case "Enemy":
                        // Can only set an attack target if there is a current unit
                        if (CurrentUnit != null)
                        {
                            CurrentUnitScript.AttackTarget = ObjectInfo.transform.gameObject;
                            Debug.Log("Set " + CurrentUnit + "'s target to " + CurrentUnitScript.AttackTarget);
                        }

                        BarracksSelected = false;
                        break;

                        // Selects the barracks so that units may be spawned
                    case "Barracks":
                        Debug.Log("BarracksClicked");
                        BarracksSelected = true;
                        CurrentUnit = null;

                        SelectedBarracks = ObjectInfo.transform.gameObject;
                        break;

                        // Selects a point for the selected unit to move
                    case "Ground":
                        if (CurrentUnit != null)
                        {
                            CurrentUnitScript.HoldPosition = ObjectInfo.point;

                            if (CurrentUnitScript.UnitLeader != null)
                            {
                                CurrentUnitScript.FollowOffset = CurrentUnitScript.HoldPosition - CurrentUnitScript.UnitLeader.transform.position;
                            }

                            /*
                            CurrentUnitScript.agent.isStopped = false;
                            CurrentUnitScript.AttackTarget = null;
                            // If the point clicked is within the castle walls
                            if ((ObjectInfo.point.x <= 100 && ObjectInfo.point.x >= -100) && (ObjectInfo.point.z <= 100 && ObjectInfo.point.z >= -100))
                            {
                                // Move the Unit
                                CurrentUnitScript.agent.SetDestination(ObjectInfo.point);
                                Debug.Log("Move " + CurrentUnit.gameObject + " to " + ObjectInfo.point);
                            }*/                          
                        }
                        BarracksSelected = false;
                        break;
                }
            }
        }
        // Right Click
        else if (Input.GetMouseButtonDown(1))
        {
            RaycastHit ObjectInfo = new RaycastHit();
            bool hit = Physics.Raycast(MainCam.ScreenPointToRay(Input.mousePosition), out ObjectInfo);

            if (hit)
            {
                Debug.Log(hit);
                switch (ObjectInfo.transform.gameObject.tag)
                {
                    case "Player":
                        if (CurrentUnit != null)
                        {
                            CurrentUnitScript.UnitLeader = ObjectInfo.transform.gameObject;
                            CurrentUnitScript.FollowOffset = CurrentUnitScript.HoldPosition - CurrentUnitScript.UnitLeader.transform.position;
                        }
                        break;
                    case "Ground":
                        if (CurrentUnit != null)
                        {
                            CurrentUnitScript.UnitLeader = null;
                        }
                        break;
                }
            }
        }
    }

    void SpawnUnits(GameObject Barracks)
    {
        // Cost of spawning a unit is 100
        if (Currency >= 100)
        {
            if (SelectedBarracks.transform.gameObject.GetComponent<Spawner>().UnitOnGate == false)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Currency -= 100;
                    CurrentUnit = Instantiate(KnightPrefab, SelectedBarracks.transform.Find("Unit Spawn Point").transform.position, SelectedBarracks.transform.Find("Unit Spawn Point").transform.rotation);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Currency -= 100;
                    CurrentUnit = Instantiate(ArcherPrefab, SelectedBarracks.transform.Find("Unit Spawn Point").transform.position, SelectedBarracks.transform.Find("Unit Spawn Point").transform.rotation);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Currency -= 100;
                    CurrentUnit = Instantiate(PikemanPrefab, SelectedBarracks.transform.Find("Unit Spawn Point").transform.position, SelectedBarracks.transform.Find("Unit Spawn Point").transform.rotation);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    Currency -= 100;
                    CurrentUnit = Instantiate(WizardPrefab, SelectedBarracks.transform.Find("Unit Spawn Point").transform.position, SelectedBarracks.transform.Find("Unit Spawn Point").transform.rotation);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha4))
                {
                    StartCoroutine(BarracksBlocked());
                }
            }
        }
    }

    

    // Gets which kind of script is on the a particular unit
    public Unit GetUnitClass(GameObject Unit)
    {
        if (Unit.GetComponent<E_Knight>() != null)
        {
            return Unit.GetComponent<E_Knight>();
        }
        else if (Unit.GetComponent<E_Archer>() != null)
        {
            return Unit.GetComponent<E_Archer>();
        }
        else if (Unit.GetComponent<E_Pikeman>() != null)
        {
            return Unit.GetComponent<E_Pikeman>();
        }
        else if (Unit.GetComponent<E_Wizard>() != null)
        {
            return Unit.GetComponent<E_Wizard>();
        }
        else if (Unit.GetComponent<F_Knight>() != null)
        {
            return Unit.GetComponent<F_Knight>();
        }
        else if (Unit.GetComponent<F_Archer>() != null)
        {
            return Unit.GetComponent<F_Archer>();
        }
        else if (Unit.GetComponent<F_Pikeman>() != null)
        {
            return Unit.GetComponent<F_Pikeman>();
        }
        else if (Unit.GetComponent<F_Wizard>() != null)
        {
            return Unit.GetComponent<F_Wizard>();
        }
        // Including buildings here to allow them to be considered an attack target
        // Saves the trouble of rewriting code elsewhere
        else if (Unit.GetComponent<Building>() != null)
        {
            return Unit.GetComponent<F_Wizard>();
        }
        else
        {
            return null;
        }
    }
}
