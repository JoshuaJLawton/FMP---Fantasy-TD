using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Knight : Unit
{
    // Initiates the Knight class
    public void InitiateKnight()
    {
        // Sets the Game Manager
        Gm = GameObject.Find("Game Manager");
        GM = Gm.GetComponent<GameManager>();

        // Sets the Knight's class
        UnitClass = GetUnitClass(this.gameObject);
        // Knight has medium health
        MaxHealth = 50;
        Health = 50;
        // Knight deals medium damage
        AttackDamage = 10;
        // Knight has close range
        Range = 3;
        // Knight has fast attack speed
        AttackSpeed = 1.5f; // 1 Second

        agent = this.gameObject.GetComponent<NavMeshAgent>();
    }

    /*
    // Executes the Knight's attack routine
    public void Attack()
    {
        Debug.Log("Attack Initiated");
        if (AttackTarget != null)
        {
            // If the Attack Target is a building
            if (AttackTarget.GetComponent<Building>() != null)
            {
                AttackTarget.GetComponent<Building>().Health -= AttackDamage;
            }
            else
            {
                // Checks to see if the enemy is a pikeman (Pikemen deal back 20% of all close range damage taken)
                if (AttackTarget.GetComponent<F_Pikeman>() != null || AttackTarget.GetComponent<E_Pikeman>() != null)
                {
                    UnitClass.Health -= UnitClass.AttackDamage * 0.2f;
                }

                GetUnitClass(AttackTarget).Health -= AttackDamage;
            } 
        }      
    }
    */
}
