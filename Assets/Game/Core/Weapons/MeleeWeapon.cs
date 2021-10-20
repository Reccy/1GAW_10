using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reccy.ScriptExtensions;

[CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Weapons/Melee")]
public class MeleeWeapon : ScriptableObject
{
    public int Damage = 5;
    public string[] AttackVerbs = { "attacked" };

    public string GetAttackVerb()
    {
        return AttackVerbs.SelectRandom();
    }
}
