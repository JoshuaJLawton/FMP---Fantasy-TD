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

        // Knight has medium health
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
        if (AttackTarget != null)
        {
            // Checks to see if the attacker is a player unit
            if (this.GetComponent<F_Knight>() != null)
            {
                // Checks which kind of script is on the target unit based on its class
                if (AttackTarget.GetComponent<E_Knight>() != null)
                {
                    if (AttackTarget.GetComponent<E_Knight>().Health > 0)
                    {
                        AttackTarget.GetComponent<E_Knight>().Health = AttackTarget.GetComponent<E_Knight>().Health - AttackDamage;
                        Debug.Log("Successful Attack");
                    }
                    else
                    {
                        AttackTarget = null;
                    }
                }
                else if (AttackTarget.GetComponent<E_Archer>() != null)
                {
                    if (AttackTarget.GetComponent<E_Archer>().Health > 0)
                    {
                        AttackTarget.GetComponent<E_Archer>().Health = AttackTarget.GetComponent<E_Archer>().Health - AttackDamage;
                        Debug.Log("Successful Attack");
                    }
                    else
                    {
                        AttackTarget = null;
                    }
                }
                else if (AttackTarget.GetComponent<E_Pikeman>() != null)
                {
                    if (AttackTarget.GetComponent<E_Pikeman>().Health > 0)
                    {
                        AttackTarget.GetComponent<E_Pikeman>().Health = AttackTarget.GetComponent<E_Pikeman>().Health - AttackDamage;
                        Debug.Log("Successful Attack");
                    }
                    else
                    {
                        AttackTarget = null;
                    }
                }
                else if (AttackTarget.GetComponent<E_Wizard>() != null)
                {
                    if (AttackTarget.GetComponent<E_Wizard>().Health > 0)
                    {
                        AttackTarget.GetComponent<E_Wizard>().Health = AttackTarget.GetComponent<E_Wizard>().Health - AttackDamage;
                        Debug.Log("Successful Attack");
                    }
                    else
                    {
                        AttackTarget = null;
                    }
                }
            }
            // Else the attacker is an enemy unit
            else
            {
                // Checks which kind of script is on the target unit based on its class
                if (AttackTarget.GetComponent<F_Knight>() != null)
                {
                    if (AttackTarget.GetComponent<F_Knight>().Health > 0)
                    {
                        AttackTarget.GetComponent<F_Knight>().Health = AttackTarget.GetComponent<F_Knight>().Health - AttackDamage;
                        Debug.Log("Successful Attack");
                    }
                    else
                    {
                        AttackTarget = null;
                    }
                }
                else if (AttackTarget.GetComponent<F_Archer>() != null)
                {
                    if (AttackTarget.GetComponent<F_Archer>().Health > 0)
                    {
                        AttackTarget.GetComponent<F_Archer>().Health = AttackTarget.GetComponent<F_Archer>().Health - AttackDamage;
                        Debug.Log("Successful Attack");
                    }
                    else
                    {
                        AttackTarget = null;
                    }
                }
                else if (AttackTarget.GetComponent<F_Pikeman>() != null)
                {
                    if (AttackTarget.GetComponent<F_Pikeman>().Health > 0)
                    {
                        AttackTarget.GetComponent<F_Pikeman>().Health = AttackTarget.GetComponent<F_Pikeman>().Health - AttackDamage;
                        Debug.Log("Successful Attack");
                    }
                    else
                    {
                        AttackTarget = null;
                    }
                }
                else if (AttackTarget.GetComponent<F_Wizard>() != null)
                {
                    if (AttackTarget.GetComponent<F_Wizard>().Health > 0)
                    {
                        AttackTarget.GetComponent<F_Wizard>().Health = AttackTarget.GetComponent<F_Wizard>().Health - AttackDamage;
                        Debug.Log("Successful Attack");
                    }
                    else
                    {
                        AttackTarget = null;
                    }
                }
            }            
        }      
    }
}
