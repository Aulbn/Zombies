using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactionString;

    public virtual void Interaction(PlayerController player)
    {
        Debug.Log("Interaction!");
    }

    public void Highlight(bool highlight)
    {
        foreach(MeshRenderer r in GetComponentsInChildren<MeshRenderer>())
        {
            foreach (Material m in r.materials)
            {
                m.SetFloat("_Emission", highlight ? 2f : 0);
            }
        }
    }
}
