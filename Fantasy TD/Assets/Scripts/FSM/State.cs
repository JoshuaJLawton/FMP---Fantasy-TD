using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private GameObject Owner;
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

        // The unit stays at their Hold Position until they see an enemy 
        // They attack the enemy and then return to their hold position

        if (OwnerScript.DetectEnemy() == null)
        {
            OwnerScript.agent.SetDestination(OwnerScript.HoldPosition);
        }
        else
        {
            OwnerScript.AttackTarget = OwnerScript.DetectEnemy();
        }



        //OwnerScript.agent.SetDestination(OwnerScript.HoldPosition);


        /////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////// EXIT CLAUSES ////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////

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
        /////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////// EXECUTE ///////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////

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

        /////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////// EXIT CLAUSES ////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////

        // If the target has been defeated
        // Change to Locate Target State
        if (OwnerScript.AttackTarget == null)
        {
            Debug.Log(Owner + " has lost their target");
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
            switch (Owner.tag)
            {
                case "Enemy":
                    OwnerScript.AIBehaviour.ChangeState(new LocateTarget(Owner, OwnerScript));
                    break;
            }
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
        /////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////// EXECUTE ///////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////

        // If a raycast directly out in front hits something
        // if the raycast has hit the attack target

        // If there is a target
        if (OwnerScript.AttackTarget != null)
        {
            Vector3 THIS = new Vector3(Owner.transform.position.x, Owner.transform.position.y + 1, Owner.transform.position.z);
            Vector3 TARGET = new Vector3(OwnerScript.AttackTarget.transform.position.x, OwnerScript.AttackTarget.transform.position.y + 1, OwnerScript.AttackTarget.transform.position.z);
            RaycastHit ObjectInFront = new RaycastHit();
            bool Hit = Physics.Raycast(THIS, Owner.transform.forward, out ObjectInFront, OwnerScript.Range);

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

        /////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////// EXIT CLAUSES ////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////

        // If the target has been defeated
        // Change to Locate Target State
        if (OwnerScript.AttackTarget == null)
        {
            Debug.Log("Lost Attack Target");
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
