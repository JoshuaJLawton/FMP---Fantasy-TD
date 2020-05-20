using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pikeman : Unit
{
    // Initiates the Pikeman class
    public void InitiatePikeman()
    {
        // Sets the Game Manager
        Gm = GameObject.Find("Game Manager");
        GM = Gm.GetComponent<GameManager>();

        // Sets the Knight's class
        UnitClass = GetUnitClass(this.gameObject);
        // Pikeman has high health
        MaxHealth = 75;
        Health = 75;
        // Pikeman deals low damage
        AttackDamage = 5;
        // Pikeman has close range
        Range = 3;
        // Pikeman has low attack speed
        AttackSpeed = 4f; // 4 Seconds

        agent = this.gameObject.GetComponent<NavMeshAgent>();
    }

    /*
    // Executes the Pikeman's attack routine
    public void Attack()
    {
        Debug.Log("Attack Initiated");

        if (AttackTarget != null)
        {
            // Checks to see if the enemy is a pikeman (Pikemen deal back 20% of all close range damage taken)
            if (AttackTarget.GetComponent<F_Pikeman>() != null || AttackTarget.GetComponent<E_Pikeman>() != null)
            {
                UnitClass.Health = UnitClass.Health - UnitClass.AttackDamage * 0.2f;
            }

            GetUnitClass(AttackTarget).Health = GetUnitClass(AttackTarget).Health - AttackDamage;
            Debug.Log("Successful attack");
        }
    }
    */
}
