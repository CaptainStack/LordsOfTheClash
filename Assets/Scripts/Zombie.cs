using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zombie : Character
{
    public float attackDamage = 1f;

    override protected void Attack()
    {
        Debug.Log("Zombie attacking " + currentTarget);
        currentTarget.health -= attackDamage;
    }
}