using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reccy.ScriptExtensions;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public int Damage = 5;
    public string[] AttackVerbs = { "attacked" };

    public string GetAttackVerb()
    {
        return AttackVerbs.SelectRandom();
    }
}
