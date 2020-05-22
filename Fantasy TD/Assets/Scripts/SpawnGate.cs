using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGate : MonoBehaviour
{
    public bool UnitOnGate;

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
            case "Enemy":
                UnitOnGate = true;
                break;
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
            case "Enemy":
                UnitOnGate = false;
                break;
        }
    }

}
