using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeData", menuName = "ScriptableObjects/MeleeData")]
public class MeleeData : ScriptableObject
{
    [Header("Melee Settings")]
    public float damage;
    public float range;
    public Sprite weaponSprite;
}
