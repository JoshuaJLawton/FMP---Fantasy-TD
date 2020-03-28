using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class E_Wizard : Wizard
{
    // Start is called before the first frame update
    void Start()
    {
        InitiateWizard();

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
