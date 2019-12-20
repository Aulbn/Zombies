using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpHP : Interactable
{
    public enum ItemType
    {
        Healthkit,
        SomethingElse
    };
    public ItemType itemType;
    public override void Interaction(PlayerController player)
    {
        if(PickUp(itemType, player))
            Destroy(gameObject);
    }
    private bool PickUp(ItemType type, PlayerController player)
    {
      bool PickedUp = false;
        switch (type)
        {
            case ItemType.Healthkit:
                if (player.hasHealthKit == false)
                {
                    player.hasHealthKit = true;
                    PickedUp = true;
                }
                break;
            case ItemType.SomethingElse:
                break;
        }

        return PickedUp;
    }
}
