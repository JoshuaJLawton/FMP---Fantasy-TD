using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool UnitOnGate;

    private void OnTriggerEnter(Collider other)
    {
        UnitOnGate = true;
    }

    private void OnTriggerExit(Collider other)
    {
        UnitOnGate = false;
    }

}
