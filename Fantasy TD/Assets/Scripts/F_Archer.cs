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
        Gm = GameObject.Find("Game Manager");
        GM = Gm.GetComponent<GameManager>();
        MainCam = GameObject.Find("Main Camera");
        Cam = MainCam.GetComponent<Camera>();
        agent = this.gameObject.GetComponent<NavMeshAgent>();

        Health = 100;
    }

    // Update is called once per frame
    void Update()
    {
        MoveUnit();
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
                        // Move the Unit
                        agent.SetDestination(Hit.point);
                    }
                }
            }
        }
    }

}

