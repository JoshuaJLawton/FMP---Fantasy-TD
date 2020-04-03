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

        /// WHEN SELECTING A TARGET FROM THEIR SPAWN POSITION, UNITS SOMETIMES MISS OUT CLOSEST TARGETS FROM ENTRANCE TO CASTLE
        /// A TRIPWIRE HAS BEEN ATTATCHED TO THE CASTLE GATE TO TRIGGER A BOOL (IsInCastle) TO SIGNIFY THAT THE UNIT IS CLOSE TO THE CASTLE GATE
        /// THE UNIT'S DECISION WILL REMAIN INACTIVE UNTIL THIS BOOL IS TRUE

        OwnerScript.AttackTarget = OwnerScript.GetAITarget();

        /*
        // If the Unit is not in the castle, don't start decision making
        if (!OwnerScript.IsInCastle)
        {
            // Move towards nearest gate
            OwnerScript.agent.SetDestination(OwnerScript.FindNearestGate().transform.position);
        }
        // Can begin decision making
        else
        {
            OwnerScript.AttackTarget = OwnerScript.GetAITarget();
        }

        /*

        // 
        // If the Unit is not in the castle
        if (!OwnerScript.IsInCastle)
        {
            // Move towards nearest gate
            OwnerScript.agent.SetDestination(OwnerScript.FindNearestGate().transform.position);
        }
        else
        {   // If there is an opposition unit(s) nearby
            if (OwnerScript.Threat() != null)
            {
                // Attack the closest unit
                OwnerScript.AttackTarget = OwnerScript.Threat();
            }
            // if there is no opposition unit nearby
            else
            {
                // Attack the closest building
                OwnerScript.AttackTarget = OwnerScript.TargetBuilding();
            }
        }
        */
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
        OwnerScript.agent.isStopped = true;
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
        Debug.Log(Owner + "Is Attacking" + OwnerScript.AttackTarget);
    }

    public void Execute()
    {
        // Execute
        if (OwnerScript.AttackTarget != null)
        {

            Vector3 THIS = new Vector3(Owner.transform.position.x, Owner.transform.position.y + 1, Owner.transform.position.z);
            Vector3 TARGET = new Vector3(OwnerScript.AttackTarget.transform.position.x, OwnerScript.AttackTarget.transform.position.y + 1, OwnerScript.AttackTarget.transform.position.z);

            RaycastHit ObjectInfo = new RaycastHit();
            bool hit = Physics.Raycast(THIS, (TARGET - THIS).normalized * Vector3.Distance(THIS, TARGET), out ObjectInfo);

            // If a raycast shot at the attack target hits
            if (hit)
            {
                // If it hits the attack target
                if (ObjectInfo.transform.gameObject == OwnerScript.AttackTarget)
                {
                    // Move towards the point where the raycast hit the target
                    OwnerScript.agent.SetDestination(ObjectInfo.point);
                }
                else
                {
                    // Move towards the attack target
                    OwnerScript.agent.SetDestination(OwnerScript.AttackTarget.transform.position);
                }
            }
            else
            {
                // Move towards the attack target
                OwnerScript.agent.SetDestination(OwnerScript.AttackTarget.transform.position);
            }         
        }  

        // Exit Clauses

        // If the target has been defeated
        // Change to Locate Target State
        if (OwnerScript.AttackTarget == null)
        {
            Debug.Log(Owner + " has lost their target");
            OwnerScript.AIBehaviour.ChangeState(new LocateTarget(Owner, OwnerScript));
        }

        // If an enemy or a closer target comes into view
        else if (OwnerScript.GetAITarget() != OwnerScript.AttackTarget)
        {
            OwnerScript.AIBehaviour.ChangeState(new LocateTarget(Owner, OwnerScript));
        }

        // If the target is within attack range
        // Change to Attack State
        else if (OwnerScript.CanAttack())
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
        
        // If a raycast directly out in front hits something
        // if the raycast has hit the attack target

        // If there is a target
        if (OwnerScript.AttackTarget != null)
        {
            Vector3 This = new Vector3(Owner.transform.position.x, Owner.transform.position.y + 1, Owner.transform.position.z);
            Vector3 Target = new Vector3(OwnerScript.AttackTarget.transform.position.x, OwnerScript.AttackTarget.transform.position.y + 1, OwnerScript.AttackTarget.transform.position.z);
            RaycastHit ObjectInFront = new RaycastHit();
            bool Hit = Physics.Raycast(This, Owner.transform.forward, out ObjectInFront, OwnerScript.Range);

            // Turn to face the enemy
            OwnerScript.FaceEnemy();

            // If something is in aim
            if (Hit)
            {
                // If the Attack Target is in Aim
                if (ObjectInFront.transform.gameObject == OwnerScript.AttackTarget)
                {
                    OwnerScript.StartAttackRoutine();
                }
            }   
        }

        // Exit Clauses

        // If the target has been defeated
        // Change to Locate Target State
        if (OwnerScript.AttackTarget == null)
        {
            Debug.Log("Lost Attack Target");
            OwnerScript.AIBehaviour.ChangeState(new LocateTarget(Owner, OwnerScript));
        }

        // If a more threatening target has been noticed (Previously attacking building and enemy comes into sight)
        // Change to Locate Target State

        // If the attack target is a building
        else if (OwnerScript.AttackTarget.GetComponent<Building>() != null)
        {
            switch (OwnerScript.AttackTarget.tag)  // This line produces an error (Archer / Wizard) but it doesn't seem to affect gameplay
            {
                // Can only determine a bigger threat if currently targetting a building
                case "Main Castle":
                case "Apothecary":
                case "Income":
                case "Barracks":

                    // Changes state to LocateTarget if there is a threat in range
                    if (OwnerScript.GetAITarget() != OwnerScript.AttackTarget)
                    {
                        OwnerScript.AIBehaviour.ChangeState(new LocateTarget(Owner, OwnerScript));
                    }
                    break;
            }
        }
        
        //  If the target has moved out of range (Can't attack)
        // Change to Move State
        else if (!OwnerScript.CanAttack())
        {
            OwnerScript.AIBehaviour.ChangeState(new Move(Owner, OwnerScript));
        }
    }

    public void Exit()
    {
        OwnerScript.agent.isStopped = true;
        Debug.Log("Exiting Attack State");
    }
}
