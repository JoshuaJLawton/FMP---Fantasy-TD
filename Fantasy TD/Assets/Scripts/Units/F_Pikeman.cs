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

        MainCam = GameObject.Find("Main Camera");
        Cam = MainCam.GetComponent<Camera>();

        agent = this.gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        IsAlive();
        IsAttacking();
        //MoveUnit();
        HealUnit(MaxHealth);
    }

    #region Attacking

    bool IsRecharging = false;

    void IsAttacking()
    {
        if (AttackTarget != null)
        {
            // If the unit is too far away to attack
            if (Vector3.Distance(this.gameObject.transform.position, AttackTarget.transform.position) > Range)
            {
                // Move towards enemy
                agent.SetDestination(AttackTarget.transform.position);
            }
            else
            {
                //agent.isStopped = true;
                FaceEnemy();
                if (!IsRecharging)
                {
                    StartCoroutine(AttackRoutine());
                }
            }
        }
    }

    public IEnumerator AttackRoutine()
    {
        IsRecharging = true;
        Attack();
        yield return new WaitForSeconds(AttackSpeed);
        IsRecharging = false;
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