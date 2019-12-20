using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : Interactable
{
    public enum ItemType
    {
        Health, Ammo, Destroy
    }
    public ItemType itemType;


    public override void Interaction(PlayerController player)
    {
        if(PickUp(itemType, player))
            Destroy(gameObject);
    }

    private bool PickUp(ItemType type, PlayerController player)
    {
      bool pickedUp = false;

        switch (type)
        {
            case ItemType.Health:
                if (!player.hasHealthKit)
                {
                    player.hasHealthKit = true;
                    pickedUp = true;
                }
                break;

            case ItemType.Ammo:
                
                break;
        }

        return pickedUp;
    }
}
