using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Canvas UICanvas;
    public Text UICurrency;

    public float Currency;

    public Camera MainCam;
    public Camera MainCamOut;
    public Camera MainCamIn;
    public Camera[] InCam = new Camera[4];
    public Camera[] OutCam = new Camera[4];

    public GameObject KnightPrefab;
    public GameObject ArcherPrefab;
    public GameObject WizardPrefab;
    public GameObject PikemanPrefab;
    public GameObject[] EnemyPrefabs = new GameObject[4];

    public GameObject CurrentUnit;
    public Unit CurrentUnitScript;

    public bool BarracksSelected;
    public GameObject SelectedBarracks;

    public GameObject Apothecary;

    public GameObject[] SpawnGates;
    public bool SpawnWait;

    

    // Start is called before the first frame update
    void Start()
    {
        Currency = 1000;
        SpawnGates = GameObject.FindGameObjectsWithTag("Spawn Gate");

        OutCam[0].enabled = true;
        for (int x = 1; x < 4; x++)
        {
            OutCam[x].enabled = false;
        }
        for (int x = 0; x < 4; x++)
        {
            InCam[x].enabled = false;
        }

        MainCamIn = InCam[0];
        MainCamOut = OutCam[0];       
        MainCam = MainCamOut;
    }

    // Update is called once per frame
    void Update()
    {
        Income();
        DisplayCurrency();

        //EnemiesController();

        SwitchCameras();

        RaycastHit ObjectInfo = new RaycastHit();
        bool hit = Physics.Raycast(MainCam.ScreenPointToRay(Input.mousePosition), out ObjectInfo);

        if (hit)
        {
            //Debug.Log(ObjectInfo.transform.gameObject);
        }


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

    #region Cameras

    public int x = 0;
    
    void SwitchCameras()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            x--;
            if (x < 0)
            {
                x = 3;
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            x++;
            if (x > 3)
            {
                x = 0;
            }
        }

        MainCamIn.enabled = false;
        MainCamIn = InCam[x];
        MainCamIn.enabled = true;

        MainCamOut.enabled = false;
        MainCamOut = InCam[x];
        MainCamOut.enabled = true;

        MainCam = MainCamIn;


        /*

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            x++;
            if (x > 7)
            {
                x = 0;
            }

            MainCam.enabled = false;
            Cam[x].enabled = true;
            MainCam = Cam[x];
            MainCam.enabled = true;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            x--;

            if (x < 0)
            {
                x = 7;
            }

            MainCam.enabled = false;
            Cam[x].enabled = true;
            MainCam = Cam[x];
            MainCam.enabled = true;
        }

        */

    }

    #endregion

    #region UI
    void DisplayCurrency()
    {
        string Display = "Gold: " + ((int)Currency).ToString();
        UICurrency.text = Display;
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
                            CurrentUnitScript.agent.isStopped = false;
                            CurrentUnitScript.AttackTarget = null;
                            // If the point clicked is within the castle walls
                            if ((ObjectInfo.point.x <= 100 && ObjectInfo.point.x >= -100) && (ObjectInfo.point.z <= 100 && ObjectInfo.point.z >= -100))
                            {
                                // Move the Unit
                                CurrentUnitScript.agent.SetDestination(ObjectInfo.point);
                                Debug.Log("Move " + CurrentUnit.gameObject + " to " + ObjectInfo.point);
                            }                          
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

    // Checks which kind of script is on the a particular unit
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
