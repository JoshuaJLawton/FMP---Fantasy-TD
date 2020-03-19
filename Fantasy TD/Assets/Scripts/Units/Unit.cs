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

    // Finds location
    public void FindLocation()
    {
        
    }

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
            Destroy(this.gameObject);
        }
    }

    // Checks which kind of script is on the attack target based on its class
    public Unit EnemyClassScript()
    {
        if (AttackTarget.GetComponent<E_Knight>() != null)
        {
            return AttackTarget.GetComponent<E_Knight>();
        }
        else if (AttackTarget.GetComponent<E_Archer>() != null)
        {
            return AttackTarget.GetComponent<E_Archer>();
        }
        else if (AttackTarget.GetComponent<E_Pikeman>() != null)
        {
            return AttackTarget.GetComponent<E_Pikeman>();
        }
        else if (AttackTarget.GetComponent<E_Wizard>() != null)
        {
            return AttackTarget.GetComponent<E_Wizard>();
        }
        else if (AttackTarget.GetComponent<F_Knight>() != null)
        {
            return AttackTarget.GetComponent<F_Knight>();
        }
        else if (AttackTarget.GetComponent<F_Archer>() != null)
        {
            return AttackTarget.GetComponent<F_Archer>();
        }
        else if (AttackTarget.GetComponent<F_Pikeman>() != null)
        {
            return AttackTarget.GetComponent<F_Pikeman>();
        }
        else if (AttackTarget.GetComponent<F_Wizard>() != null)
        {
            return AttackTarget.GetComponent<F_Wizard>();
        }
        else
        {
            return null;
        }
    }

    // Gets the kind of script on the current unit based on its class
    public Unit GetUnitClass()
    {
        if (this.GetComponent<F_Knight>() != null)
        {
            return this.GetComponent<F_Knight>();
        }
        else if (this.GetComponent<F_Archer>() != null)
        {
            return this.GetComponent<F_Archer>();
        }
        else if (this.GetComponent<F_Pikeman>() != null)
        {
            return this.GetComponent<F_Pikeman>();
        }
        else if (this.GetComponent<F_Wizard>() != null)
        {
            return this.GetComponent<F_Wizard>();
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
}
