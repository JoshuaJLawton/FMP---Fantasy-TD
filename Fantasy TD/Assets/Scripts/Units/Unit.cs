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

    void GetEnemiesInRange()
    {

    }

    void LocateTarget()
    {

    }

    void MoveUnit()
    {

    }

    #endregion
}
