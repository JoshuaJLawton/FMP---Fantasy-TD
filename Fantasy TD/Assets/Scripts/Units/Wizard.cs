﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wizard : Unit
{
    // The prefab of the spells which will be cast
    public GameObject SpellPrefab;

    // Initiates the Wizard class
    public void InitiateWizard()
    {
        // Sets the Game Manager
        Gm = GameObject.Find("Game Manager");
        GM = Gm.GetComponent<GameManager>();

        // Sets the Knight's class
        UnitClass = GetUnitClass(this.gameObject);
        // Wizard has low health
        MaxHealth = 25;
        Health = 25;
        // Wizard deals medium damage
        AttackDamage = 7.5f;
        // Wizard has medium range
        Range = 25;
        // Wizard has medium attack speed
        AttackSpeed = 3f; // 3 Seconds
        // Spells fly slower
        ProjectileSpeed = 50;

        agent = this.gameObject.GetComponent<NavMeshAgent>();
    }

    /*

    // Executes the Wizard's attack routine
    public void CastSpell()
    {
        Debug.Log("Attack Initiated");

        if (AttackTarget != null)
        {
            GameObject Arrow = Instantiate(SpellPrefab, this.transform.Find("Spell Spawn Point").transform.position, this.transform.Find("Spell Spawn Point").transform.rotation);
            Arrow.GetComponent<Projectile>().Attacker = this.gameObject;
            Arrow.GetComponent<Projectile>().Damage = this.AttackDamage;
            Arrow.GetComponent<Projectile>().Speed = 50;
        }
    }
    */
}
