using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    // Holds the Game Manager
    public GameObject Gm;
    public GameManager GM;

    // Controls the NavMesh
    public NavMeshAgent agent;

    // Holds the unit's attack target
    public GameObject AttackTarget;
    // Holds the unit's target location
    public Vector3 MoveLocationTarget;

    // Holds the unit's leader
    public GameObject UnitLeader;
    // Position to follow a leader
    public Vector3 FollowOffset;

    // Holds the unit's class script
    public Unit UnitClass;
    // Holds the Unit's Maximum amount of health
    public float MaxHealth;
    // Holds the unit's health
    public float Health;
    // Holds the unit's Attack Damage
    public float AttackDamage;
    // Holds the distance the unit has to be within to the enemy in order to attack
    public float Range;
    // Holds the time between attacks
    public float AttackSpeed;
    // If the Unit is recharging their attack
    public bool RechargingAttack;

    // PLAYER UNIT ONLY
    // Holds the Unit's main position
    public Vector3 HoldPosition;


    // Holds Arrow / Spell Prefab
    public GameObject ProjectilePrefab;
    // Holds Speed in which projectiles fly
    public int ProjectileSpeed;

    // Whether the unit is being healed by the apothecary
    public bool IsBeingHealed;

    // Whether the unit is within castle walls
    public bool IsInCastle;

    // Holds the state machine which will control the AI's behaviour
    public StateMachine AIBehaviour;

    // Moves the unit to the target location
    public Vector3 MoveTo(Vector3 MoveLocationTarget)
    {
        if (AttackTarget != null)
        {
            return AttackTarget.gameObject.transform.position;
        }
        else
        {
            return AttackTarget.gameObject.transform.position;
        }
    }

    #region Find / Initiate Functions

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





    #endregion




    #region Attacking Functions


    public bool CanAttack()
    {
        //Debug.DrawRay(this.transform.position, this.transform.forward * Range, Color.red);
        //Debug.DrawRay(this.transform.position, (AttackTarget.transform.position - this.transform.position).normalized * Range, Color.blue);

        
        // CLOSE RANGE UNITS REQUIRE A BOOST TO THEIR RANGE IN ORDER TO REGISTER AN ATTACK ON BUILDINGS
        if (this.gameObject.GetComponent<E_Knight>() != null || this.gameObject.GetComponent<E_Pikeman>() != null)
        {
            switch (AttackTarget.gameObject.tag)
            {
                case "Income":
                case "Barracks":
                case "Apothecary":
                case "Main Tower":
                    Range = 10;
                    break;
                default:
                    Range = 3;
                    break;
            }
        }
        

        Vector3 THIS = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);
        Vector3 TARGET = new Vector3(AttackTarget.transform.position.x, AttackTarget.transform.position.y + 1, AttackTarget.transform.position.z);

        Vector3 Forward = new Vector3(transform.forward.x, this.transform.forward.y, this.transform.forward.z);

        RaycastHit ObjectInfo = new RaycastHit();
        bool hit = Physics.Raycast(THIS, Forward /*(TARGET - THIS).normalized*/ * Range, out ObjectInfo);
        //Debug.DrawRay(THIS, Forward /*(TARGET - THIS).normalized*/ * Range, Color.red);

        // If the attack target is in attack range
        if (Vector3.Distance(THIS, TARGET) < Range)
        {
            // If a raycast pointing at the attack target hits
            if (hit)
            {
                if (ObjectInfo.transform.gameObject == AttackTarget)
                {
                    Debug.Log("Can Attack");
                    return true;
                }
                // if the raycast hits a teammate (there is a teammate directly in front of them)
                else if (ObjectInfo.transform.gameObject.tag == this.gameObject.tag)
                {
                    Vector3 FRIENDLY = new Vector3(ObjectInfo.transform.position.x, ObjectInfo.transform.position.y + 1, ObjectInfo.transform.position.z);
                    //RaycastHit ObjectInfo2 = new RaycastHit();
                    // Shoots a raycast out the remainder of the way to see if it hits the target
                    bool hit2 = Physics.Raycast(FRIENDLY, transform.forward /*(TARGET - THIS).normalized*/ * (Range - Vector3.Distance(THIS, FRIENDLY)), out ObjectInfo);
                    if (hit2)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

        

    // CANNOT  USE COROUTINE IN STATE SO PASS IT THROUGH THIS FUNCTION
    public void StartAttackRoutine()
    {
        if (!RechargingAttack)
        {
            StartCoroutine(AttackRoutine());
        }
    }


    // Runs the attack routine once per amount of time in Attack speed
    public IEnumerator AttackRoutine()
    {
        RechargingAttack = true;
        Attack();
        yield return new WaitForSeconds(AttackSpeed);
        RechargingAttack = false;
    }


    // Attack the target
    public void Attack()
    {
        if (AttackTarget != null)
        {
            // If this unit is a Knight or Pikeman
            if (this.GetComponent<F_Knight>() != null || this.GetComponent<E_Knight>() != null || this.GetComponent<F_Pikeman>() != null || this.GetComponent<E_Pikeman>() != null)
            {
                // If the Attack Target is a building
                if (AttackTarget.GetComponent<Building>() != null)
                {
                    // Deal damage to the building
                    AttackTarget.GetComponent<Building>().Health -= (AttackDamage + Random.Range(-4, 4));
                }
                else
                {
                    // Checks to see if the enemy is a pikeman (Pikemen deal back 20% of all close range damage taken)
                    if (AttackTarget.GetComponent<F_Pikeman>() != null || AttackTarget.GetComponent<E_Pikeman>() != null)
                    {
                        UnitClass.Health -= UnitClass.AttackDamage * 0.2f;
                    }

                    GetUnitClass(AttackTarget).Health -= (AttackDamage + Random.Range(-4, 4));
                }
            }
            // If this unit is an Archer or Wizard
            else if (this.GetComponent<F_Archer>() != null || this.GetComponent<E_Archer>() != null || this.GetComponent<F_Wizard>() != null || this.GetComponent<E_Wizard>() != null)
            {
                Debug.Log("Attack Initiated");

                GameObject Projectile = Instantiate(ProjectilePrefab, this.transform.Find("Projectile Spawn Point").transform.position, this.transform.Find("Projectile Spawn Point").transform.rotation);
                Projectile.GetComponent<Projectile>().Attacker = this.gameObject;
                Projectile.GetComponent<Projectile>().Damage = (AttackDamage + Random.Range(-4, 4));
                Projectile.GetComponent<Projectile>().Speed = ProjectileSpeed;
            }
        }
    }


    // Turn to face the attack target
    public void FaceEnemy()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(AttackTarget.transform.position - transform.position), 2.5f * Time.deltaTime);
    }


    #endregion



    // Destroys the unit if it runs out of health
    public void IsAlive()
    {
        if (this.Health <= 0)
        {
            // Holds all friendly units
            GameObject[] FriendlyUnits;
            // Holds all opposition units 
            GameObject[] OppositionUnits;

            // Determines whether the dying unit is a Player or Enemy unit and gets stores all opposition units in an array
            if (this.gameObject.tag == "Player")
            {
                FriendlyUnits = GameObject.FindGameObjectsWithTag("Player");
                OppositionUnits = GameObject.FindGameObjectsWithTag("Enemy");
            }
            else
            {
                FriendlyUnits = GameObject.FindGameObjectsWithTag("Enemy");
                OppositionUnits = GameObject.FindGameObjectsWithTag("Player");
            }

            // Checks each friendly unit and clears their Unit Leader if they are following this unit
            foreach (GameObject Unit in FriendlyUnits)
            {
                // If the opposition unit is targeting this unit
                if (GetUnitClass(Unit).UnitLeader = this.gameObject)
                {
                    // Clears the AttackTarget
                    GetUnitClass(Unit).UnitLeader = null;
                }
            }

            // Checks each opposition unit and clears their attack target if they are targeting this unit
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

    

    // Heals the unit over time
    public void HealUnit(float MaxHealth)
    {
        if (IsBeingHealed && Health < MaxHealth)
        {
            UnitClass.Health += 0.75f * Time.deltaTime;
        }
        else if (UnitClass.Health > MaxHealth)
        {
            // When healing to max, sometimes health goes higher than max. This resets that
            UnitClass.Health = MaxHealth;
        }
    }



    #region Player Functions

    // Detects enemies within range of the unit's view range
    public GameObject DetectEnemy()
    {
        GameObject CurrentTarget = null;
        GameObject[] PotentialTargets;

        // First check all opposition units
        PotentialTargets = GameObject.FindGameObjectsWithTag("Enemy");

        // Checks every opposition unit
        foreach (GameObject Unit in PotentialTargets)
        {

            Vector3 THIS = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);
            Vector3 UNIT = new Vector3(Unit.transform.position.x, Unit.transform.position.y + 1, Unit.transform.position.z);

            // Debug.DrawRay(THIS, (UNIT - THIS).normalized * Vector3.Distance(THIS, UNIT), Color.yellow);

            // If the opposition unit is in sight range (50) and within sight range of the hold position
            // (The unit only attacks if the enemy is in range of their set area
            if (Vector3.Distance(HoldPosition, UNIT) < 50 && Vector3.Distance(THIS, UNIT) < 50)
            {
                // If this is the first unit being looked at
                if (CurrentTarget == null)
                {
                    CurrentTarget = Unit;
                }
                // If this unit is closer than the previous closest unit
                else if (Vector3.Distance(THIS, UNIT) < Vector3.Distance(THIS, new Vector3(CurrentTarget.transform.position.x, CurrentTarget.transform.position.y + 1, CurrentTarget.transform.position.z)))
                {
                    CurrentTarget = Unit;
                }
            }
        }

        return CurrentTarget;
    }

    #endregion


    #region AI Functions

    /*
    public GameObject FindLeader()
    {
        GameObject CurrentLeader = null;
        GameObject[] PotentialLeaders;

        // First check all friendly Units
        PotentialLeaders = GameObject.FindGameObjectsWithTag("Enemy");

        // Checks all friendlies
        foreach (GameObject Unit in PotentialLeaders)
        {
            Vector3 THIS = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);
            Vector3 UNIT = new Vector3(Unit.transform.position.x, Unit.transform.position.y + 1, Unit.transform.position.z);

            // Is the unit within sight distance?
            if (Vector3.Distance(THIS, UNIT) < 50)
            {
                // If this is the first unit being looked at
                if (CurrentLeader == null)
                {
                    CurrentLeader = Unit;
                }


            }
        }
    }
    */

    public GameObject GetAITarget()
    {
        GameObject CurrentTarget = null;
        GameObject[] PotentialTargets;

        // First check all opposition units
        PotentialTargets = GameObject.FindGameObjectsWithTag("Player");

        // Checks every opposition unit
        foreach (GameObject Unit in PotentialTargets)
        {           
            Vector3 THIS = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);
            Vector3 UNIT = new Vector3(Unit.transform.position.x, Unit.transform.position.y + 1, Unit.transform.position.z);

            // Debug.DrawRay(THIS, (UNIT - THIS).normalized * Vector3.Distance(THIS, UNIT), Color.yellow);

            // If the opposition unit is in sight range (50)
            if (Vector3.Distance(THIS, UNIT) < 50)
            {
                // If this is the first unit being looked at
                if (CurrentTarget == null)
                {
                    CurrentTarget = Unit;
                }
                // If this unit is closer than the previous closest unit
                else if (Vector3.Distance(THIS, UNIT) < Vector3.Distance(THIS, new Vector3(CurrentTarget.transform.position.x, CurrentTarget.transform.position.y + 1, CurrentTarget.transform.position.z)))
                {
                    CurrentTarget = Unit;
                }
            }  
        }

        // At this point, if there are opposition units within 50, the closest is stored in CurrentTarget

        // Holds the type of building
        string BuildingType = null;       

        // If there is no opposition unit to target
        if (CurrentTarget == null)
        {
            // Runs this code 4 times (Once for each building type)
            for (int x = 0; x < 4; x++)
            {
                // Changes the tag being assessed
                switch (x)
                {
                    case 0:
                        BuildingType = "Income";
                        break;
                    case 1:
                        BuildingType = "Apothecary";
                        break;
                    case 2:
                        BuildingType = "Barracks";
                        break;
                    case 3:
                        BuildingType = "Main Tower";
                        break;
                }

                // Gets all the buildings under tag
                PotentialTargets = GameObject.FindGameObjectsWithTag(BuildingType);

                // Checks all buildings of that tag
                foreach (GameObject Building in PotentialTargets)
                {
                    // If this is the first threat in range
                    if (CurrentTarget == null)
                    {
                        CurrentTarget = Building;
                    }
                    // If the current closest building is further away than the building being looked at
                    else if (Vector3.Distance(this.gameObject.transform.position, Building.transform.position) < Vector3.Distance(this.gameObject.transform.position, CurrentTarget.transform.position))
                    {
                        CurrentTarget = Building;
                    }
                }
            }
        }

        Debug.Log("Attack target is " + CurrentTarget);
        return CurrentTarget;
    }

    #endregion
}
