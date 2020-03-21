using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // The unit which fired the projectile
    public GameObject Attacker;
    // Class of the attacker
    public Unit AttackerUnitClass;
    // Class of the attacked unit
    public Unit HitUnitClass;
    // The damage of attacker
    public float Damage;
    // Speed of the projectile
    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
        AttackerUnitClass = GetUnitClass(Attacker);
    }

    // Update is called once per frame
    void Update()
    {
        MoveProjectile();
    }

    // Moves the projectile (Arrows and Spells need different directional pushes)
    void MoveProjectile()
    {
        // If the attacker is an archer
        if (Attacker.GetComponent<F_Archer>() != null || Attacker.GetComponent<E_Archer>() != null)
        {
            transform.position -= transform.up * (Speed * Time.deltaTime);
        }
        // If the attacker is a wizard
        else if (Attacker.GetComponent<F_Wizard>() != null || Attacker.GetComponent<E_Wizard>() != null)
        {
            transform.position += transform.forward * (Speed * Time.deltaTime);
        }

    }

    // Gets the class script of the unit attacked 
    /*public Unit GetClassScript(GameObject AttackedUnit)
    {
        if (AttackedUnit.GetComponent<F_Knight>() != null)
        {
            return AttackedUnit.GetComponent<F_Knight>();
        }
        else if (AttackedUnit.GetComponent<F_Archer>() != null)
        {
            return AttackedUnit.GetComponent<F_Archer>();
        }
        else if (AttackedUnit.GetComponent<F_Pikeman>() != null)
        {
            return AttackedUnit.GetComponent<F_Pikeman>();
        }
        else if (AttackedUnit.GetComponent<F_Wizard>() != null)
        {
            return AttackedUnit.GetComponent<F_Wizard>();
        }
        else if (AttackedUnit.GetComponent<E_Knight>() != null)
        {
            return AttackedUnit.GetComponent<E_Knight>();
        }
        else if (AttackedUnit.GetComponent<E_Archer>() != null)
        {
            return AttackedUnit.GetComponent<E_Archer>();
        }
        else if (AttackedUnit.GetComponent<E_Pikeman>() != null)
        {
            return AttackedUnit.GetComponent<E_Pikeman>();
        }
        else if (AttackedUnit.GetComponent<E_Wizard>() != null)
        {
            return AttackedUnit.GetComponent<E_Wizard>();
        }
        else
        {
            return null;
        }
    }*/

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

    private void OnTriggerEnter(Collider other)
    {
        // Ignores the collision if it is with the unit initiating the attack or its teammates
        if (other.gameObject != Attacker && other.gameObject.tag != Attacker.tag)
        {
            // Sets the class script of the unit attacked
            HitUnitClass = GetUnitClass(other.gameObject);

            if (HitUnitClass != null)
            {
                HitUnitClass.Health -= Damage;
                Debug.Log("Arrow hit " + other.gameObject + "for" + Damage + " damage");
            }
            Destroy(this.gameObject);
        }
    }
}
