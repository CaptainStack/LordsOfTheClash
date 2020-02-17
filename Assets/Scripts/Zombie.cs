using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zombie : Unit
{
    public float attackDamage = 1f;

    override protected void Attack()
    {
        currentTarget.health -= attackDamage;
    }
}