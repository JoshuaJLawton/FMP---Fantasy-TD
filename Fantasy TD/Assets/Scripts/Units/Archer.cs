using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Archer : Unit
{
    // The prefab of the arrows which will be fired
    public GameObject ArrowPrefab;

    // Initiates the Archer class
    public void InitiateArcher()
    {
        // Sets the Game Manager
        Gm = GameObject.Find("Game Manager");
        GM = Gm.GetComponent<GameManager>();

        // Sets the Knight's class
        UnitClass = GetUnitClass(this.gameObject);
        // Archer has low health
        MaxHealth = 25;
        Health = 25;
        // Archer deals high damage
        AttackDamage = 20;
        // Archer has long range
        Range = 40;
        // Archer has low attack speed (Reload time)
        AttackSpeed = 5; // 5 Seconds

        agent = this.gameObject.GetComponent<NavMeshAgent>();
    }

    // Executes the Archer's attack routine
    public void FireArrow()
    {
        Debug.Log("Attack Initiated");

        GameObject Arrow = Instantiate(ArrowPrefab, this.transform.Find("Arrow Spawn Point").transform.position, this.transform.Find("Arrow Spawn Point").transform.rotation);
        Arrow.GetComponent<Projectile>().Attacker = this.gameObject;
        Arrow.GetComponent<Projectile>().Damage = this.AttackDamage;
        Arrow.GetComponent<Projectile>().Speed = 100;
    }
}
