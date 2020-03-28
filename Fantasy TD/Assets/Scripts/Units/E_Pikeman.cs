using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class E_Pikeman : Pikeman
{
    // Start is called before the first frame update
    void Start()
    {
        InitiatePikeman();

        AIBehaviour = new StateMachine();
        AIBehaviour.ChangeState(new LocateTarget(this.gameObject, this));
    }

    // Update is called once per frame
    void Update()
    {
        IsAlive();

        this.AIBehaviour.ExecuteStateUpdate();
    }
}
