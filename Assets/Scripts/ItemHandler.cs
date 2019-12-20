using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    protected PlayerController owner;
    
    protected IEnumerator WaitForAction(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

}
