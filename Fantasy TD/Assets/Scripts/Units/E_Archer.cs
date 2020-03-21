using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Archer : Archer
{
    // Start is called before the first frame update
    void Start()
    {
        InitiateArcher();

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
