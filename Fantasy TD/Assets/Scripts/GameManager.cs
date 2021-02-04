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
    public Button StartGameBTN, OpenControlsBTN, OpenCreditsBTN, CloseGameBTN;
    public Button ControlsReturnBTN, CreditsReturnBTN;
    public GameObject MusicCreditsTitle, MusicCredits, ArtCreditsTitle, ArtCredits, ProgrammingCreditsTitle, ProgrammingCredits, GameDesignCreditsTitle, GameDesignCredits;

    bool PlayingCredits = false;

    bool IsLoadingLevel;
    public GameObject LoadingBar;

    public Sprite[] Tutorials = new Sprite[18];
    public Image OpenTutorial;
    public int TutorialIndex;
    public Text UITutorialIndex;


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

    public bool AutoAttack;

    public int UnitIndex;
    public GameObject[] AllPlayerUnits;

    [Header("UI")]

    public Canvas UICanvas;
    public Text UICurrency;
    public Text UIGameOver;
    public Text UIViewPoint;
    public Text UIPrompt;
    public Text UIAutoAttack;

    public GameObject UIBarracksBlocked;

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

    public GameObject UIButtons, UIButtonPrompt;
    public Button UIKnight, UIArcher, UIPikeman, UIWizard;

    public Image UIGoldImage;

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

                TutorialIndex = 0;

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

                AutoAttack = true;
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

                ScrollTutorials();

                break;

            case "Gamefield":
                PlayerHasLost = !IsMainTowerStanding();

                if (!PlayerHasLost)
                {
                    Income();
                    SwitchCameras();
                    PlayerInput();
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
                DisplayAutoTarget();
                DisplayBarracksBlocked();

                ReturnToMenu();

                Waves();

                AllPlayerUnits = GameObject.FindGameObjectsWithTag("Player");

                // Gets the class script of the current unit
                if (CurrentUnit != null)
                {
                    CurrentUnitScript = GetUnitClass(CurrentUnit);
                }

                // Selects the barracks of the current viewpoint
                if (Barracks[Direction] != null)
                {
                    SelectedBarracks = Barracks[Direction];
                }

                // Allows the player to spawn units if the selected barracks is still standing
                if (SelectedBarracks != null)
                {
                    SpawnUnitsKeys();
                    
                }

                RunSpawnUnitsButtons();

                break;
        }  
    }

    #region Main Menu

    // Main Menu

    void MoveMenuCamera()
    {
        //POSITION -31.8, 75, 103.7 ROTATION 35, 20, 0
        MenuCamera.transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, 10 * Time.deltaTime);
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

        CloseGameBTN.onClick.AddListener(CloseGame);
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

    void ScrollTutorials()
    {
        if (ControlsPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (TutorialIndex == 0)
                {
                    TutorialIndex = 17;
                }
                else
                {
                    TutorialIndex--;
                }

            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (TutorialIndex == 17)
                {
                    TutorialIndex = 0;
                }
                else
                {
                    TutorialIndex++;
                }
            }
        }

        OpenTutorial.GetComponent<Image>().sprite = Tutorials[TutorialIndex];
        UITutorialIndex.text = (TutorialIndex + 1) + " / 18";

    }

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

    void CloseGame()
    {
        Debug.Log("Close Game");
        Application.Quit();
    }

    #endregion

    #region Player Input

    // Makes player inputs work
    void PlayerInput()
    {
        // Extra check to make sure unit spawn panel isn't showing
        if (!Input.GetKey(KeyCode.LeftShift))
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

                        // Selects a point for the selected unit to move
                        case "Ground":
                            if (CurrentUnit != null)
                            {
                                CurrentUnitScript.HoldPosition = ObjectInfo.point;

                                if (CurrentUnitScript.UnitLeader != null)
                                {
                                    CurrentUnitScript.FollowOffset = CurrentUnitScript.HoldPosition - CurrentUnitScript.UnitLeader.transform.position;
                                }
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
                                CurrentUnitScript.FollowOffset = (CurrentUnitScript.HoldPosition - CurrentUnitScript.UnitLeader.transform.position);
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
        
        // Toggle Auto Attack
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            AutoAttack = !AutoAttack;
        }

        // Quick switch between units
        if (Input.GetKeyDown(KeyCode.W))
        {
            SwitchToNextUnit(-1);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            SwitchToNextUnit(1);
        }

        // Return to main menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadingBar.SetActive(true);
            StartCoroutine(LoadAsynchronously(0, 2));
        }
    }

    // Allows the player to spawn units with HUD
    void RunSpawnUnitsButtons()
    {
        if (PlayerHasLost || SelectedBarracks == null)
        {
            UIButtons.SetActive(false);            
        }
        else
        {
            UIButtons.SetActive(true);
            UIKnight.onClick.AddListener(SpawnKnightButton);
            UIArcher.onClick.AddListener(SpawnArcherButton);
            UIPikeman.onClick.AddListener(SpawnPikemanButton);
            UIWizard.onClick.AddListener(SpawnWizardButton);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            UIButtonPrompt.SetActive(false);
            UIKnight.gameObject.SetActive(true);
            UIArcher.gameObject.SetActive(true);
            UIPikeman.gameObject.SetActive(true);
            UIWizard.gameObject.SetActive(true);
        }
        else
        {
            UIButtonPrompt.SetActive(true);
            UIKnight.gameObject.SetActive(false);
            UIArcher.gameObject.SetActive(false);
            UIPikeman.gameObject.SetActive(false);
            UIWizard.gameObject.SetActive(false);
        }
        
    }

    bool UnitSpawned = false;

    IEnumerator SpawnUnit(GameObject Prefab)
    {
        if (!UnitSpawned)
        {
            UnitSpawned = true;
            if (Currency >= 100)
            {
                if (!UnitOnGate(SelectedBarracks))
                {
                    Currency -= 100;
                    CurrentUnit = Instantiate(Prefab, SelectedBarracks.transform.Find("Unit Spawn Point").transform.position, SelectedBarracks.transform.Find("Unit Spawn Point").transform.rotation);
                }
            }
        }
        yield return new WaitForSeconds(2);
        UnitSpawned = false;       
    }

    void SpawnKnightButton()
    {
        StartCoroutine(SpawnUnit(KnightPrefab));
    }

    void SpawnArcherButton()
    {
        if (SelectedBarracks != null && Currency >= 100)
        {
            if (!UnitOnGate(SelectedBarracks))
            {
                Currency -= 100;
                CurrentUnit = Instantiate(ArcherPrefab, SelectedBarracks.transform.Find("Unit Spawn Point").transform.position, SelectedBarracks.transform.Find("Unit Spawn Point").transform.rotation);
            }
        }
    }

    void SpawnPikemanButton()
    {
        if (SelectedBarracks != null && Currency >= 100)
        {
            if (!UnitOnGate(SelectedBarracks))
            {
                Currency -= 100;
                CurrentUnit = Instantiate(PikemanPrefab, SelectedBarracks.transform.Find("Unit Spawn Point").transform.position, SelectedBarracks.transform.Find("Unit Spawn Point").transform.rotation);
            }
        }
    }

    void SpawnWizardButton()
    {
        if (SelectedBarracks != null && Currency >= 100)
        {
            if (!UnitOnGate(SelectedBarracks))
            {
                Currency -= 100;
                CurrentUnit = Instantiate(WizardPrefab, SelectedBarracks.transform.Find("Unit Spawn Point").transform.position, SelectedBarracks.transform.Find("Unit Spawn Point").transform.rotation);
            }
        }
    }

    // Allows player to spawn units using keys
    void SpawnUnitsKeys()
    {
        // Cost of spawning a unit is 100
        if (Currency >= 100)
        {
            //if (SelectedBarracks.transform.gameObject.GetComponent<Spawner>().UnitOnGate == false)
            if (!UnitOnGate(SelectedBarracks))
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
                    //Debug.Log("Barracks Blocked");
                    //StartCoroutine(BarracksBlocked());
                }
            }
        }
    }

    // Determines if the barracks is being blocked
    bool UnitOnGate(GameObject Barracks)
    {
        bool Toggle = false;

        // All player units
        GameObject[] AllUnits = GameObject.FindGameObjectsWithTag("Player");

        // For every friendly unit
        foreach (GameObject Unit in AllUnits)
        {
            // If the friendly unit is within a very close range of the spawn point
            if (Vector3.Distance(SelectedBarracks.transform.Find("Unit Spawn Point").transform.position, Unit.transform.position) <= 1.5f)
            {
                // There is a unit blocking the spawn point
                return true;
            }
        }

        AllUnits = GameObject.FindGameObjectsWithTag("Enemy");

        // For every enemy unit
        foreach (GameObject Unit in AllUnits)
        {
            // If the enemy unit is within a very close range of the spawn point
            if (Vector3.Distance(SelectedBarracks.transform.Find("Unit Spawn Point").transform.position, Unit.transform.position) <= 1.5f)
            {
                // There is a unit blocking the spawn point
                return true;
            }
        }

        return Toggle;
    }

    #endregion

    #region Game Running

    // Determines if the main tower has been destroyed
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

    // Runs the increase of the player's gold over time
    void Income()
    {
        // Only increases currency if it is less than the max of 2000
        if (Currency < 10000)
        {
            // Currency increases faster while more Income buildings are standing and stops completely when there are none
            GameObject[] IncomeBuildings = GameObject.FindGameObjectsWithTag("Income");
            Currency += (IncomeBuildings.Length * 0.5f) * Time.deltaTime;
        }
        // Sets Currency back to 2000 if it is exceeded
        else if (Currency > 10000)
        {
            Currency = 10000;
        }
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
            string Display = ((int)Currency).ToString();
            UICurrency.text = Display;
        }
        else
        {
            UICurrency.enabled = false;
            UIGoldImage.enabled = false;
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

        if (PlayerHasLost)
        {
            BarracksHealthBar.SetActive(false);
        }
        else if(Barracks != null)
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

    void DisplayBarracksBlocked()
    {
        if (!PlayerHasLost)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (UnitOnGate(SelectedBarracks))
                {
                    UIBarracksBlocked.SetActive(true);
                }
                else
                {
                    UIBarracksBlocked.SetActive(false);
                }
            }
            else
            {
                UIBarracksBlocked.SetActive(false);
            }
        }
        else
        {
            UIBarracksBlocked.SetActive(false);
        }
        
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

            if (hit && !Input.GetKey(KeyCode.LeftShift))
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

    void DisplayAutoTarget()
    {
        if (!PlayerHasLost)
        {
            UIAutoAttack.gameObject.SetActive(true);
            switch (AutoAttack)
            {
                case true:
                    UIAutoAttack.text = "Auto Attack On";
                    break;
                case false:
                    UIAutoAttack.text = "Auto Attack Off";
                    break;
            }
        }
        else
        {
            UIAutoAttack.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Enemies

    // Makes enemies spawn in waves
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

    // Spawns enemies
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

    #region Get Functions

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
        else
        {
            return null;
        }
    }

    void SwitchToNextUnit(int IndexChange)
    {
        // If there are player units
        if (AllPlayerUnits.Length > 0)
        {
            // for every player unit
            for (int x = 0; x < AllPlayerUnits.Length; x++)
            {
                // If the player unit being looked at is the current unit
                if (AllPlayerUnits[x] == CurrentUnit)
                {
                    // If this is the last unit in the array
                    if (x + IndexChange >= AllPlayerUnits.Length)
                    {
                        UnitIndex = 0;
                    }
                    // If this is the first unit in the array
                    else if (x + IndexChange < 0)
                    {
                        UnitIndex = AllPlayerUnits.Length - 1;
                    }
                    else
                    {
                        UnitIndex = x + IndexChange;
                    }

                }
            }
            CurrentUnit = AllPlayerUnits[UnitIndex];
        }
    }

    #endregion
}
