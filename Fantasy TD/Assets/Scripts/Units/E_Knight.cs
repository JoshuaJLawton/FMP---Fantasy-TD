﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Knight : Knight
{
    // Start is called before the first frame update
    void Start()
    {
        InitiateKnight();
    }

    // Update is called once per frame
    void Update()
    {
        IsAlive();
    }
}
