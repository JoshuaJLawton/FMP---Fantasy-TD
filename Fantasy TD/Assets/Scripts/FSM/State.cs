using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface State
{
    void Enter();

    void Execute();

    void Exit();
}




// Locate Target is where the unit will evaluate its surroundings and find a suitable attack target (Player Unit or Building)
public class LocateTarget : State
{
    private GameObject Owner;
    private Unit OwnerScript;

    public LocateTarget(GameObject _owner, Unit _ownerScript)
    {
        Owner = _owner;
        OwnerScript = _ownerScript;
    }

    public void Enter()
    {
        Debug.Log(Owner + " has entered the locate target state!");
    }

    public void Execute()
    {
        Debug.Log(Owner + " is running the locate target state!");

        // Run 

            // Get all potential targets (Opposition Units / Buildings) and set it as a target (Units have priority)
                // Knight / Archer / Pikeman / Wizard
                // Income Building
                // Apothecary
                // Barracks
                // Main Tower

        // Exit Clauses

            // If a target has been found
                // Change to Move State
    }

    public void Exit()
    {
        Debug.Log(Owner + " is leaving the locate target state!");
    }
}




// Move is where the unit will travel towards their target
public class Move : State
{
    private GameObject Owner;
    private Unit OwnerScript;

    public Move(GameObject _owner, Unit _ownerScript)
    {
        Owner = _owner;
        OwnerScript = _ownerScript;
    }

    public void Enter()
    {
        Debug.Log(Owner + " has entered the Move state!");
    }

    public void Execute()
    {
        Debug.Log(Owner + " is running the Move state!");

        // Exit Clauses

            // If the target has been defeated
                // Change to Locate Target State

            // If the target is within attack range
                // Change to Attack State
    }

    public void Exit()
    {
        Debug.Log(Owner + " is leaving the Move state!");
    }
}




// Attack is where the unit will conflict with their target
public class Attack : State
{
    private GameObject Owner;
    private Unit OwnerScript;

    public Attack(GameObject _owner, Unit _ownerScript)
    {
        Owner = _owner;
        OwnerScript = _ownerScript;
    }

    public void Enter()
    {
        Debug.Log(Owner + " has entered the Attack state!");
    }

    public void Execute()
    {
        Debug.Log(Owner + " is running the Attack state!");

        // Exit Clauses

            // If the target has been defeated
                // Change to Locate Target State

            // If a more threatening target has been noticed (Previously attacking building and enemy comes into sight)
                // Change to Locate Target State

            //  If the target has moved out of range
                // Change to Move State
    }

    public void Exit()
    {
        Debug.Log(Owner + " is leaving the Attack state!");
    }
}
