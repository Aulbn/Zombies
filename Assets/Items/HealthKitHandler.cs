using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthKitHandler : ItemHandler
{
    public float casttime = 0.5f;

    private IEnumerator UseCoroutine;
    void UseItem()
    {
        if (UseCoroutine == null)
        {
            StartCoroutine(UseCoroutine = (WaitForAction(casttime, () =>
            {
                owner.ChangeHealth(100);
            })));
        }
    }
}
