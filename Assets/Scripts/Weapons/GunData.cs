using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "ScriptableObjects/GunData")]
public class GunData : ScriptableObject
{
    public enum gunTypes {
        raycast,
        projectile,
        laser,
        squirtmachine
    }

    public gunTypes gunType;

    public float bulletDistance;
    public float damage;
    public bool auto = true;

    // Recoil
    [Header("Recoil Settings")]
    public float recoilX;
    public float recoilY;

    // Plasma Settings
    [Header("Plasma Settings")]
    public float chargeTime;
    public float laserInterval;
    public float laserInitialKnockback;
    public float laserKnockback;
    public float laserRange;
    public float laserDrag;
    public float laserDragMultiplier;

    // Projectile Settings
    [Header("Projectile Settings")]
    public float projectileForce;
    public float projectileInterval;
}
