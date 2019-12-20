using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpHP : Interactable
{
    public override void Interaction(PlayerController player)
    {
        if (!player.hasHealthKit)
        {
            player.hasHealthKit = true;
            Destroy(gameObject);
        }
    }
}
