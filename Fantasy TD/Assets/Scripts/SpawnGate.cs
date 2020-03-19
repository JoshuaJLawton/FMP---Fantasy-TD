﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGate : MonoBehaviour
{
    public bool UnitOnGate;

    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        UnitOnGate = true;
    }

    private void OnTriggerExit(Collider other)
    {
        UnitOnGate = false;
    }

}