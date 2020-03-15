using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    // Holds the Game Manager
    public GameObject Gm;
    public GameManager GM;

    // Holds the unit's attack target
    public GameObject AttackTarget;
    // Holds the unit's target location
    public Vector3 MoveLocationTarget;

    // Holds the unit's health
    public float Health;
    // Holds the unit's Attack Damage
    public float AttackDamage;
    // Holds the distance the unit has to be within to the enemy in order to attack
    public float Range;
    // Holds the time between attacks
    public float AttackSpeed;

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
}
