using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Wizard : Wizard
{
    // Start is called before the first frame update
    void Start()
    {
        InitiateWizard();
    }

    // Update is called once per frame
    void Update()
    {
        IsAlive();
    }
}
