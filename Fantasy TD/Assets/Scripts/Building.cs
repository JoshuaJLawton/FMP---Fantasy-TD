using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public float Health;

    // Start is called before the first frame update
    void Start()
    {
        switch(this.gameObject.tag)
        {
            case "Income":
                Health = 200;
                break;
            case "Apothecary":
                Health = 250;
                break;
            case "Barracks":
                Health = 300;
                break;
            case "Main Tower":
                Health = 500;
                break;
        }
    }

    void Update()
    {
        if (this.Health <= 0)
        {
            // Holds all opposition units 
            GameObject[] OppositionUnits;

            // Only enemies can target a building so only enemies need to be checked
            OppositionUnits = GameObject.FindGameObjectsWithTag("Enemy");

            // Checks each opposition unit and clears their attack target if they are targeting this building
            foreach (GameObject Unit in OppositionUnits)
            {
                // If the opposition unit is targeting this unit
                if (GetUnitClass(Unit).AttackTarget = this.gameObject)
                {
                    // Clears the AttackTarget
                    GetUnitClass(Unit).AttackTarget = null;
                }
            }

            Destroy(this.gameObject);
        }
    }

    // Checks which kind of script is on the a particular unit
    public Unit GetUnitClass(GameObject Unit)
    {
        if (Unit.GetComponent<E_Knight>() != null)
        {
            return Unit.GetComponent<E_Knight>();
        }
        else if (Unit.GetComponent<E_Archer>() != null)
        {
            return Unit.GetComponent<E_Archer>();
        }
        else if (Unit.GetComponent<E_Pikeman>() != null)
        {
            return Unit.GetComponent<E_Pikeman>();
        }
        else if (Unit.GetComponent<E_Wizard>() != null)
        {
            return Unit.GetComponent<E_Wizard>();
        }
        else if (Unit.GetComponent<F_Knight>() != null)
        {
            return Unit.GetComponent<F_Knight>();
        }
        else if (Unit.GetComponent<F_Archer>() != null)
        {
            return Unit.GetComponent<F_Archer>();
        }
        else if (Unit.GetComponent<F_Pikeman>() != null)
        {
            return Unit.GetComponent<F_Pikeman>();
        }
        else if (Unit.GetComponent<F_Wizard>() != null)
        {
            return Unit.GetComponent<F_Wizard>();
        }
        else
        {
            return null;
        }
    }

}

