﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public GameObject KnightPrefab;
    public GameObject ArcherPrefab;
    public GameObject WizardPrefab;
    public GameObject PikemanPrefab;

    public GameObject CurrentUnit;
    public Unit CurrentUnitScript;

    public bool BarracksSelected;
    public GameObject SelectedBarracks;

    public GameObject Apothecary;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Click();

        if (CurrentUnit != null)
        {
            CurrentUnitScript = GetClassScript();
        }

        if (SelectedBarracks != null)
        {
            SpawnUnits(SelectedBarracks);
        }   
    }

    void Click()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit ObjectInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out ObjectInfo);

            if (hit)
            {
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
                        if (CurrentUnit != null)
                        {
                            CurrentUnitScript.AttackTarget = ObjectInfo.transform.gameObject;
                            Debug.Log("Set " + CurrentUnit + "'s target to " + CurrentUnitScript.AttackTarget);
                        }

                        BarracksSelected = false;
                        break;

                    case "Barracks":
                        Debug.Log("BarracksClicked");
                        BarracksSelected = true;
                        CurrentUnit = null;

                        SelectedBarracks = ObjectInfo.transform.gameObject;
                        break;

                    case "Ground":
                        if (CurrentUnit != null)
                        {
                            CurrentUnitScript.agent.isStopped = false;
                            CurrentUnitScript.AttackTarget = null;
                            // Move the Unit
                            CurrentUnitScript.agent.SetDestination(ObjectInfo.point);
                            Debug.Log("Move " + CurrentUnit.gameObject + " to " + ObjectInfo.point);
                        }

                        BarracksSelected = false;
                        break;

                    case "Spawn Gate":
                        if (ObjectInfo.transform.gameObject.GetComponent<SpawnGate>().UnitOnGate == false)
                        {
                            Debug.Log("Spawn Gate");
                            Instantiate(KnightPrefab, ObjectInfo.transform.Find("Unit Spawn Point").transform.position, ObjectInfo.transform.Find("Unit Spawn Point").transform.rotation);
                        }

                        BarracksSelected = false;
                        break;
                }
            }
        }
    }

    void SpawnUnits(GameObject Barracks)
    {
        if (BarracksSelected)
        {
            if (SelectedBarracks.transform.gameObject.GetComponent<Spawner>().UnitOnGate == false)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Instantiate(KnightPrefab, SelectedBarracks.transform.Find("Unit Spawn Point").transform.position, SelectedBarracks.transform.Find("Unit Spawn Point").transform.rotation);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Instantiate(ArcherPrefab, SelectedBarracks.transform.Find("Unit Spawn Point").transform.position, SelectedBarracks.transform.Find("Unit Spawn Point").transform.rotation);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Instantiate(PikemanPrefab, SelectedBarracks.transform.Find("Unit Spawn Point").transform.position, SelectedBarracks.transform.Find("Unit Spawn Point").transform.rotation);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    Instantiate(WizardPrefab, SelectedBarracks.transform.Find("Unit Spawn Point").transform.position, SelectedBarracks.transform.Find("Unit Spawn Point").transform.rotation);
                }
            }
        }
    }


    // Gets the kind of script on the current unit based on its class
    public Unit GetClassScript()
    {
        if (CurrentUnit.GetComponent<F_Knight>() != null)
        {
            return CurrentUnit.GetComponent<F_Knight>();
        }
        else if (CurrentUnit.GetComponent<F_Archer>() != null)
        {
            return CurrentUnit.GetComponent<F_Archer>();
        }
        else if (CurrentUnit.GetComponent<F_Pikeman>() != null)
        {
            return CurrentUnit.GetComponent<F_Pikeman>();
        }
        else if (CurrentUnit.GetComponent<F_Wizard>() != null)
        {
            return CurrentUnit.GetComponent<F_Wizard>();
        }
        else
        {
            return null;
        }
    }
}
