using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("Game Management")]
    public bool PlayerHasLost;
    public float Currency;

    public GameObject CurrentUnit;
    public Unit CurrentUnitScript;

    public bool BarracksSelected;
    public GameObject SelectedBarracks;

    public GameObject Apothecary;

    public GameObject[] SpawnGates;
    public bool SpawnWait;

    [Header("UI")]
    public Canvas UICanvas;
    public Text UICurrency;
    public Text UIGameOver;
    public Text UIViewPoint;

    public GameObject UnitHealthBar;
    public Text CurrentUnitName;
    public GameObject EnemyHealthBar;
    public Text EnemyUnitName;
    public GameObject MainTowerHealthBar;
    public GameObject BarracksHealthBar;
    public Text BarracksName;

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
        // Determines whether the lose conditions of the game has been met
        PlayerHasLost = false;

        Direction = 0;

        InitiateCameras();

        Currency = 1000;
        SpawnGates = GameObject.FindGameObjectsWithTag("Spawn Gate");

    }

    // Update is called once per frame
    void Update()
    {
        PlayerHasLost = !IsMainTowerStanding();

        Income();
        DisplayCurrency();
        DisplayGameOver();
        DisplayViewPointText();
        DisplayCurrentUnitHealthBar();
        DisplayEnemyUnitHealthBar();
        DisplayMainTowerHealthBar();
        DisplayBarracksHealth();

        EnemiesController();

        SwitchCameras();

        // Debug to test if mouse raycast is working
        /*
        RaycastHit ObjectInfo = new RaycastHit();
        bool hit = Physics.Raycast(MainCam.ScreenPointToRay(Input.mousePosition), out ObjectInfo);

        if (hit)
        {
            //Debug.Log(ObjectInfo.transform.gameObject);
        }
        */

        Click();

        if (CurrentUnit != null)
        {
            CurrentUnitScript = GetUnitClass(CurrentUnit);
        }

        if (SelectedBarracks != null)
        {
            SpawnUnits(SelectedBarracks);
        }   
    }

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


    #endregion

    #region Enemies

    void EnemiesController()
    {
        float Rand1 = Random.Range(0, SpawnGates.Length - 1);
        float Rand2 = Random.Range(0, EnemyPrefabs.Length - 1);

        if (!SpawnWait)
        {
            StartCoroutine(SpawnEnemies((int) Rand1, (int) Rand2));
        }
        
    }

    IEnumerator SpawnEnemies(int Random1, int Random2)
    {
        SpawnWait = true;
        GameObject EnemyUnit = Instantiate(EnemyPrefabs[Random2], SpawnGates[Random1].transform.Find("Unit Spawn Point").transform.position, SpawnGates[Random1].transform.Find("Unit Spawn Point").transform.rotation);
        yield return new WaitForSeconds(5);
        SpawnWait = false;
    }

    #endregion

    void Click()
    {
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
    }

    void SpawnUnits(GameObject Barracks)
    {
        // Cost of spawning a unit is 100
        if (BarracksSelected && Currency >= 100)
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
