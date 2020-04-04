using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class F_Pikeman : Pikeman
{
    public GameObject MainCam;
    private Camera Cam;

    // Start is called before the first frame update
    void Start()
    {
        InitiatePikeman();

        AIBehaviour = new StateMachine();
        AIBehaviour.ChangeState(new HoldPosition(this.gameObject, this));

        HoldPosition = this.transform.position;
        agent = this.gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        this.AIBehaviour.ExecuteStateUpdate();

        IsAlive();
        //IsAttacking();
        //MoveUnit();
        HealUnit(MaxHealth);
    }

    #region Attacking

    void IsAttacking()
    {
        // If there is an attack target
        if (AttackTarget != null)
        {
            // If the unit has a clear sight of the enemy
            if (CanAttack())
            {
                Vector3 THIS = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);
                Vector3 TARGET = new Vector3(AttackTarget.transform.position.x, AttackTarget.transform.position.y + 1, AttackTarget.transform.position.z);

                RaycastHit ObjectInFront = new RaycastHit();
                bool Hit = Physics.Raycast(THIS, this.transform.forward, out ObjectInFront, Range);

                // If something is in aim and within range
                if (Hit)
                {
                    // If the Attack Target is in Aim
                    if (ObjectInFront.transform.gameObject == AttackTarget)
                    {
                        // Turn to face the enemy
                        FaceEnemy();
                        StartAttackRoutine();
                    }
                }
            }

            agent.SetDestination(AttackTarget.transform.position);
        }
    }

    #endregion

    #region Collisions

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.name)
        {
            // If the unit has entered the Apothecary Guild
            case "Apothecary Guild":
                IsBeingHealed = true;
                break;
            // If the unit has exited the Apothecary Guild
            case "Exit":
                IsBeingHealed = false;
                break;
        }
    }

    #endregion
}