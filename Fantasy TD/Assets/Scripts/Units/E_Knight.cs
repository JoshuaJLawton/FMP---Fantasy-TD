using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class E_Knight : Knight
{
    // Start is called before the first frame update
    void Start()
    {
        InitiateKnight();

        AIBehaviour = new StateMachine();
        AIBehaviour.ChangeState(new LocateTarget(this.gameObject, this));
    }

    // Update is called once per frame
    void Update()
    {
        IsAlive();

        this.AIBehaviour.ExecuteStateUpdate();
    }

    #region Collisions

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.name)
        {
            // If the unit has entered the Apothecary Guild
            case "Gate Entry":
                IsInCastle = true;
                break;
            // If the unit has exited the Apothecary Guild
            case "Gate Exit":
                IsInCastle = false;
                break;
        }
    }

    #endregion
}