using Reccy.ScriptExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ranged Weapon", menuName = "Weapons/Ranged")]
public class RangedWeapon : ScriptableObject
{
    public int Damage = 5;
    public string[] AttackVerbs = { "attacked" };

    public string GetAttackVerb()
    {
        return AttackVerbs.SelectRandom();
    }
}
