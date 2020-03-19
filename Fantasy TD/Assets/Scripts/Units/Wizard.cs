using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Unit
{
    // Initiates the Wizard class
    public void InitiateWizard()
    {
        // Sets the Game Manager
        Gm = GameObject.Find("Game Manager");
        GM = Gm.GetComponent<GameManager>();

        // Sets the Knight's class
        UnitClass = GetUnitClass();
        // Wizard has low health
        MaxHealth = 25;
        Health = 25;
        // Wizard deals medium damage
        AttackDamage = 7.5f;
        // Wizard has medium range
        Range = 25;
        // Wizard has medium attack speed
        AttackSpeed = 3.5f; // 5 Seconds
    }

    // Executes the Wizard's attack routine
    public void CastSpell()
    {
        if (AttackTarget != null)
        {
            Debug.Log("Attack Initiated");
            // If the enemy will survive the next attack
            if (EnemyClassScript().Health > AttackDamage)
            {
                EnemyClassScript().Health = EnemyClassScript().Health - AttackDamage;
                Debug.Log("Successful attack");
            }
            else
            {
                EnemyClassScript().Health = EnemyClassScript().Health - AttackDamage;
                Debug.Log("Successful attack");
                AttackTarget = null;
            }
        }
    }
}
