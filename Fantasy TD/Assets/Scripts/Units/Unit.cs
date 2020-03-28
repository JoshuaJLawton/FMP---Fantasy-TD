using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    // Holds the Game Manager
    public GameObject Gm;
    public GameManager GM;

    // Controls the NavMesh
    public NavMeshAgent agent;

    // Holds the unit's attack target
    public GameObject AttackTarget;
    // Holds the unit's target location
    public Vector3 MoveLocationTarget;

    // Holds the unit's class script
    public Unit UnitClass;
    // Holds the Unit's Maximum amount of health
    public float MaxHealth;
    // Holds the unit's health
    public float Health;
    // Holds the unit's Attack Damage
    public float AttackDamage;
    // Holds the distance the unit has to be within to the enemy in order to attack
    public float Range;
    // Holds the time between attacks
    public float AttackSpeed;

    // Whether the unit is being healed by the apothecary
    public bool IsBeingHealed;

    // Holds the state machine which will control the AI's behaviour
    public StateMachine AIBehaviour;

    // Moves the unit to the target location
    public Vector3 MoveTo(Vector3 MoveLocationTarget)
    {
        if (AttackTarget != null)
        {
            return AttackTarget.gameObject.transform.position;
        }
        else
        {
            return AttackTarget.gameObject.transform.position;
        }
    }

    public void FaceEnemy()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(AttackTarget.transform.position - transform.position), 2.5f * Time.deltaTime);
    }

    public void IsAlive()
    {
        if (this.Health <= 0)
        {
            // Holds all opposition units 
            GameObject[] OppositionUnits;

            // Determines whether the dying unit is a Player or Enemy unit and gets stores all opposition units in an array
            if (this.gameObject.tag == "Player")
            {
                OppositionUnits = GameObject.FindGameObjectsWithTag("Enemy");
            }
            else
            {
                OppositionUnits = GameObject.FindGameObjectsWithTag("Player");
            }

            // Checks each opposition unit and clears their attack target if they are targeting this unit
            foreach (GameObject Unit in OppositionUnits)
            {
                // If the opposition unit is targeting this unit
                if (GetUnitClass(Unit).AttackTarget = this.gameObject)
                {
                    // Clears the AttackTarget
                    GetUnitClass(Unit).AttackTarget = null;
                }
            }

            Destroy(this.gameObject);
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
        else
        {
            return null;
        }
    }

    // Heals the unit over time
    public void HealUnit(float MaxHealth)
    {
        if (IsBeingHealed && Health < MaxHealth)
        {
            UnitClass.Health += 0.75f * Time.deltaTime;
        }
        else if (UnitClass.Health > MaxHealth)
        {
            // When healing to max, sometimes health goes higher than max. This resets that
            UnitClass.Health = MaxHealth;
        }
    }

    #region AI Functions

    public bool CanAttack()
    {
        RaycastHit ObjectInfo = new RaycastHit();

        Debug.DrawRay(this.transform.position, this.transform.forward * Range, Color.red);

        if (Vector3.Distance(this.gameObject.transform.position, AttackTarget.transform.position) < Range)
        {
            Debug.Log("Check 1");
            if (Physics.Raycast(this.transform.position, this.transform.forward, out ObjectInfo, Range))
            {
                Debug.Log("Check 2");
                if (ObjectInfo.transform.gameObject == AttackTarget)
                {
                    Debug.Log("Check 3");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            
        }
        else
        {
            return false;
        }
    }

    // Gets a unit threat;
    public GameObject Threat()
    {
        GameObject ClosestThreat = null;
        GameObject[] Units = GameObject.FindGameObjectsWithTag("Player");

        // If there is at least one threat (enemy unit)
        if (Units.Length > 0)
        {
            // Checks every threat
            foreach (GameObject Unit in Units)
            {
                Unit Class = GetUnitClass(Unit);

                // If the threat is targeting this unit
                if (Class.AttackTarget == this.gameObject)
                {
                    // If the threat is within threatening distance
                    if (Vector3.Distance(this.gameObject.transform.position, Unit.transform.position) > 50)
                    {
                        // If this is the first threat in range
                        if (ClosestThreat == null)
                        {
                            ClosestThreat = Unit;
                        }
                        // If the current closest threat is further away than the unit being looked at
                        else if (Vector3.Distance(this.gameObject.transform.position, ClosestThreat.transform.position) > Vector3.Distance(this.gameObject.transform.position, Unit.transform.position))
                        {
                            ClosestThreat = Unit;
                        }
                    }
                }
            }

            return ClosestThreat;
        }
        else
        {
            return null;
        }
    }

    // Gets the closest building to the unit
    public GameObject TargetBuilding()
    {
        GameObject ClosestBuilding = null;
        string BuildingType = null;
        GameObject[] Buildings;

        // Runs this code 4 times
        for (int x = 0; x < 4; x++)
        {
            // Changes the tag being assessed
            switch (x)
            {
                case 0:
                    BuildingType = "Income";
                    break;
                case 1:
                    BuildingType = "Apothecary";
                    break;
                case 2:
                    BuildingType = "Barracks";
                    break;
                case 3:
                    BuildingType = "Main Castle";
                    break;
            }

            // Gets all the buildings under tag
            Buildings = GameObject.FindGameObjectsWithTag(BuildingType);

            // Checks all buildings of that tag
            foreach (GameObject Building in Buildings)
            {
                // If there is at least one building in the array
                if (Buildings.Length > 0)
                {
                    // If this is the first threat in range
                    if (ClosestBuilding == null)
                    {
                        ClosestBuilding = Building;
                    }
                    // If the current closest building is further away than the building being looked at
                    else if (Vector3.Distance(this.gameObject.transform.position, ClosestBuilding.transform.position) > Vector3.Distance(this.gameObject.transform.position, Building.transform.position))
                    {
                        ClosestBuilding = Building;
                    }
                }
            }
        }

        return ClosestBuilding;
    }

    public void GetEnemiesInRange()
    {

    }

    public void LocateTarget()
    {

    }


    #endregion
}
