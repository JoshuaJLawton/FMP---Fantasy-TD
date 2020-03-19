﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pikeman : Unit
{
    // Initiates the Pikeman class
    public void InitiatePikeman()
    {
        // Sets the Game Manager
        Gm = GameObject.Find("Game Manager");
        GM = Gm.GetComponent<GameManager>();

        // Sets the Knight's class
        UnitClass = GetUnitClass();
        // Pikeman has high health
        MaxHealth = 75;
        Health = 75;
        // Pikeman deals low damage
        AttackDamage = 5;
        // Pikeman has close range
        Range = 3;
        // Pikeman has low attack speed
        AttackSpeed = 3.5f; // 2.5 Seconds
    }

    // Executes the Pikeman's attack routine
    public void Attack()
    {
        Debug.Log("Attack Initiated");

        // Checks to see if the enemy is a pikeman (Pikemen deal back 20% of all close range damage taken)
        if (AttackTarget.GetComponent<F_Pikeman>() != null || AttackTarget.GetComponent<E_Pikeman>() != null)
        {
            UnitClass.Health = UnitClass.Health - UnitClass.AttackDamage * 0.2f;
        }

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