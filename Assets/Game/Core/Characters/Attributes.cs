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
}
