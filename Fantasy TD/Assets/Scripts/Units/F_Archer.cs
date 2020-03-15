using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class F_Archer : Archer
{
    public GameObject MainCam;
    private Camera Cam;
    public NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        InitiateArcher();

        MainCam = GameObject.Find("Main Camera");
        Cam = MainCam.GetComponent<Camera>();

        agent = this.gameObject.GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        IsAttacking();
        MoveUnit();
    }

    #region Original
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
                agent.isStopped = true;
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
        FireArrow();
        yield return new WaitForSeconds(AttackSpeed);
        IsRecharging = false;
    }

    void MoveUnit()
    {
        if (this.gameObject == GM.CurrentUnit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Mouse Clicked");
                Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit Hit;

                if (Physics.Raycast(ray, out Hit))
                {
                    if (Hit.transform.gameObject.tag == "Ground")
                    {
                        agent.isStopped = false;
                        // Nullifies the Attack Target
                        AttackTarget = null;
                        // Move the Unit
                        agent.SetDestination(Hit.point);

                    }
                }
            }
        }
    }

    #endregion

    #region New

    void SetMovementLocation()
    {

    }

    void SetAttackTarget()
    {

    }

    #endregion

}

