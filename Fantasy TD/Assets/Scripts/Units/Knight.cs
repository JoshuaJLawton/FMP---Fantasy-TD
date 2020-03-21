using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // Knight has medium attack speed
        AttackSpeed = 1; // 1 Second
    }

    // Executes the Knight's attack routine
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

            GetUnitClass(AttackTarget).Health -= AttackDamage;
            Debug.Log("Successful attack");
        }      
    }
}
