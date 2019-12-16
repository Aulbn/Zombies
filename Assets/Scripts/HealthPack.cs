using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Interactable
{
    public int hpGain = 50;

    public override void Interaction(PlayerController player)
    {
        player.health = Mathf.Clamp(player.health + hpGain, 0, player.maxHealth);
        Destroy(gameObject);
    }
}
