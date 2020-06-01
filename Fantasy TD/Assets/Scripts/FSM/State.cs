using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base State class type
public interface State
{
    void Enter();

    void Execute();

    void Exit();
}

/////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////// PLAYER UNITS ONLY ////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////

public class HoldPosition : State
{
    // The unit currently using this state
    private GameObject Owner;
    // That unit's script
    private Unit OwnerScript;

    public HoldPosition(GameObject _owner, Unit _ownerScript)
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
        /////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////// EXECUTE ///////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////

        // If the unit has a leader they follow the leader and share an attack target with them
        // Otherwise the unit stays at their Hold Position until they see an enemy 
        // They attack the enemy and then return to their hold position

        // If the unit has a leader
        if (OwnerScript.UnitLeader != null)
        {            
            // Follow the leader
            OwnerScript.agent.SetDestination(OwnerScript.UnitLeader.transform.position + OwnerScript.FollowOffset);

            // If the leader has an attack target
            if (OwnerScript.GetUnitClass(OwnerScript.UnitLeader).AttackTarget != null)
            {
                // Set attack target to match leader's
                OwnerScript.AttackTarget = OwnerScript.GetUnitClass(OwnerScript.UnitLeader).AttackTarget;
            }
        }
        // If the unit has no leader and autoattack is enabled
        else if (OwnerScript.GM.AutoAttack)
        {
            // If the unit does not detect an enemy
            if (OwnerScript.DetectEnemy() == null)
            {
                // Hold their set position
                OwnerScript.agent.SetDestination(OwnerScript.HoldPosition);
            }
            else
            {
                // Look for enemy
                OwnerScript.AttackTarget = OwnerScript.DetectEnemy();
            }
        }
        // If the unit has no leader and autoattack is not enabled
        else
        {
            // Hold their set position
            OwnerScript.agent.SetDestination(OwnerScript.HoldPosition);
        }


        /////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////// EXIT CLAUSES ////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////

        // If a target has been found
        if (OwnerScript.AttackTarget != null)
        {
            // Change to Move State
            OwnerScript.AIBehaviour.ChangeState(new Move(Owner, OwnerScript));
        }

    }

    public void Exit()
    {
        OwnerScript.agent.isStopped = true;
    }
}


/////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////// ENEMY UNITS ONLY ///////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////

// Locate Target is where the unit will evaluate its surroundings and find a 
// suitable attack target (Player Unit or Building)

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
        /////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////// EXECUTE ///////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////
       
        // Sets the attack target
        OwnerScript.AttackTarget = OwnerScript.GetAITarget();
        
        /////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////// EXIT CLAUSES ////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////

        // If a target has been found
        if (OwnerScript.AttackTarget != null)
        {
            // Change to Move State
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
    }

    public void Execute()
    {
        /////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////// EXECUTE ///////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////

        if (OwnerScript.AttackTarget != null)
        {
            // RAYCASTS SHOT FROM UNIT MUST BE SHOT FROM POSITIONS HIGHER UP TO AVOID TOUCHING THE GROUND
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

        /////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////// EXIT CLAUSES ////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////

        // If the target has been defeated
        // Change to Default state
        if (OwnerScript.AttackTarget == null)
        {
            //Debug.Log(Owner + " has lost their target");
            switch (Owner.tag)
            {
                case "Player":
                    OwnerScript.AIBehaviour.ChangeState(new HoldPosition(Owner, OwnerScript));
                    break;

                case "Enemy":
                    OwnerScript.AIBehaviour.ChangeState(new LocateTarget(Owner, OwnerScript));
                    break;
            }            
        }

        // ONLY A CHECK FOR ENEMY UNITS
        // If an enemy or a closer target comes into view
        else if (Owner.tag == "Enemy" && OwnerScript.GetAITarget() != OwnerScript.AttackTarget)
        {
            // Change to locate target state
            OwnerScript.AIBehaviour.ChangeState(new LocateTarget(Owner, OwnerScript));
        }

        // If the target is within attack range
        else if (OwnerScript.CanAttack())
        {
            // Change to Attack State
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
        /////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////// EXECUTE ///////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////

        // If a raycast directly out in front hits something
        // if the raycast has hit the attack target

        // If there is a target
        if (OwnerScript.AttackTarget != null)
        {

            // Turn to face the enemy
            OwnerScript.FaceEnemy();

            // If the unit can attack
            if (OwnerScript.CanAttack())
            {
                // Attack
                OwnerScript.StartAttackRoutine();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////// EXIT CLAUSES ////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////

        // If the target has been defeated
        if (OwnerScript.AttackTarget == null)
        {
            // Change to Locate Target State
            switch (Owner.tag)
            {
                case "Player":
                    OwnerScript.AIBehaviour.ChangeState(new HoldPosition(Owner, OwnerScript));
                    break;

                case "Enemy":
                    OwnerScript.AIBehaviour.ChangeState(new LocateTarget(Owner, OwnerScript));
                    break;
            }
        }

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
        else if (!OwnerScript.CanAttack())
        {
            // Change to Move State
            OwnerScript.AIBehaviour.ChangeState(new Move(Owner, OwnerScript));
        }
    }

    public void Exit()
    {
        OwnerScript.agent.isStopped = true;
    }
}
