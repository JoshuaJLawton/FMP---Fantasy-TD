using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit
{
    // Initiates the Archer class
    public void InitiateArcher()
    {
        // Sets the Game Manager
        Gm = GameObject.Find("Game Manager");
        GM = Gm.GetComponent<GameManager>();

        // Sets the Knight's class
        UnitClass = GetUnitClass();
        // Archer has low health
        MaxHealth = 25;
        Health = 25;
        // Archer deals high damage
        AttackDamage = 20;
        // Archer has long range
        Range = 40;
        // Archer has low attack speed (Reload time)
        AttackSpeed = 5; // 5 Seconds
    }

    // Executes the Archer's attack routine
    public void FireArrow()
    {
        Debug.Log("Attack Initiated");
        if (AttackTarget != null)
        {
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
