using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : Interactable
{
    public Weapon weapon;

    public override void Interaction(PlayerController player)
    {
        player.AddWeapon(weapon);
        Destroy(gameObject);
    }
}
