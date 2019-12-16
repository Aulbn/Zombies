using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : Interactable
{
    public int ammoGain = 50;
    public Weapon.AmmoType ammoType;

    public override void Interaction(PlayerController player)
    {
        player.PickUpAmmo(ammoType, ammoGain);
        Destroy(gameObject);
    }
}
