using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes : MonoBehaviour
{
    [Header("Vitals")]
    [SerializeField] public int Health = 10;
    [SerializeField] public int MaxHealth = 10;

    [Header("Skills")]
    [SerializeField] public int CookingSkill = 10;

    [Header("Combat")]
    [SerializeField] public Weapon MeleeWeapon;
    [SerializeField] public Weapon RangedWeapon;

    public delegate void OnHealthExhaustedEvent();
    public OnHealthExhaustedEvent OnHealthExhausted;

    public void Harm(int damage)
    {
        Health -= damage;

        HealthBoundsCheck();
    }

    public void Heal(int recovery)
    {
        Health += recovery;

        HealthBoundsCheck();
    }

    private void HealthBoundsCheck()
    {
        if (Health <= 0)
        {
            if (OnHealthExhausted != null)
                OnHealthExhausted();
        }

        Health = Mathf.Clamp(Health, 0, MaxHealth);
    }
}
