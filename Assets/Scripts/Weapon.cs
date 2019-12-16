using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 1)]
public class Weapon : ScriptableObject
{
    [Header("Stats")]
    public float damage;
    public int magSize;
    public enum AmmoType { small, medium, shotgun }
    public AmmoType ammoType;
    public float fireRate;
    public float fireRange;
    public float recoilAmmount;
    public AnimationCurve damageFalloff = AnimationCurve.Linear(0,1,1,1);

    [Header("UI")]
    public Sprite icon;
    public Sprite crosshair;

    [Header("VFX")]
    public ParticleSystem hitEffect;
    public AudioClip gunSound;

    [Header("Animation")]
    public AnimatorOverrideController animatorController;
    public float reloadTime, unhoslterTime;

    [Header("Physical")]
    public GameObject weaponHandlerPrefab;
    public GameObject weaponPickupPrefab;

    public float GetDamage(float range)
    {
        return damage * damageFalloff.Evaluate(range / fireRange);
    }

    public WeaponHandler SpawnWeapon(PlayerController player, Transform handParent, Transform bodyParent)
    {
        WeaponHandler w = Instantiate(weaponHandlerPrefab, player.transform, false).GetComponent<WeaponHandler>();
        w.weaponHand.SetParent(handParent);
        w.weaponBody.SetParent(bodyParent);
        return w;
    }

}
