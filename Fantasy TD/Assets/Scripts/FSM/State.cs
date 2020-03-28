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
        Debug.Log(OwnerScript.MaxHealth);
        OwnerScript.agent.isStopped = false;
    }

    public void Execute()
    {

        // Execute

        // Get all potential targets (Opposition Units / Buildings) and set it as a target (Units have priority)
        // Knight / Archer / Pikeman / Wizard
        // Income Building
        // Apothecary
        // Barracks
        // Main Tower

        if (OwnerScript.Threat() != null)
        {
            OwnerScript.AttackTarget = OwnerScript.Threat();
        }
        else
        {
            OwnerScript.AttackTarget = OwnerScript.TargetBuilding();
        }

        // Exit Clauses

        // If a target has been found
        // Change to Move State
        if (OwnerScript.AttackTarget != null)
        {
            OwnerScript.AIBehaviour.ChangeState(new Move(Owner, OwnerScript));
        }
    }

    public void Exit()
    {

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
        OwnerScript.agent.isStopped = false;
    }

    public void Execute()
    {
        // Execute
        OwnerScript.agent.SetDestination(OwnerScript.AttackTarget.transform.position);

        // Exit Clauses

        // If the target has been defeated
        // Change to Locate Target State
        if (OwnerScript.AttackTarget == null)
        {
            Debug.Log(Owner + " has lost their target");
            OwnerScript.AIBehaviour.ChangeState(new LocateTarget(Owner, OwnerScript));
        }

        // If the target is within attack range
        // Change to Attack State
        if (OwnerScript.CanAttack())
        {
            Debug.Log(Owner + " is in attack range");
            OwnerScript.AIBehaviour.ChangeState(new Attack(Owner, OwnerScript));
        }
    }

    public void Exit()
    {
        OwnerScript.agent.isStopped = true;
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
        OwnerScript.agent.isStopped = true;
    }

    public void Execute()
    {
        // Execute

        Debug.Log("AATTTAAAAAAACCKKKKKKKKK");

        // Exit Clauses

        // If the target has been defeated
        // Change to Locate Target State
        if (OwnerScript.AttackTarget == null)
        {
            OwnerScript.AIBehaviour.ChangeState(new LocateTarget(Owner, OwnerScript));
        }

        // If a more threatening target has been noticed (Previously attacking building and enemy comes into sight)
        // Change to Locate Target State
        switch (OwnerScript.AttackTarget.tag)  // This line produces an error (Archer / Wizard) but it doesn't seem to affect gameplay
        {
            // Can only determine a bigger threat if currently targetting a building
            case "Main Castle":
            case "Apothecary":
            case "Income":
            case "Barracks":

                // Changes state to LocateTarget if there is a threat in range
                if (OwnerScript.Threat() != null)
                {
                    OwnerScript.AIBehaviour.ChangeState(new LocateTarget(Owner, OwnerScript));
                }
                break;
        }

        //  If the target has moved out of range
        // Change to Move State
        if (Vector3.Distance(this.OwnerScript.gameObject.transform.position, OwnerScript.AttackTarget.transform.position) > 50)
        {
            OwnerScript.AIBehaviour.ChangeState(new Move(Owner, OwnerScript));
        }
    }

    public void Exit()
    {

    }
}
