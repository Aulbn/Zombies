using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEntity : MonoBehaviour
{
    [Header("VFX")]
    public ParticleSystem muzzleFlare;

    [Header("Physical")]
    public Vector3 position;
    public Vector3 rotation;

    public void SetParent(Transform parent)
    {
        transform.parent = parent;
        transform.localPosition = position;
        transform.localEulerAngles = rotation;
    }

    public void Shoot()
    {
        if (muzzleFlare != null) muzzleFlare.Play(true);
        //Play gun animation
    }

    public void SetLayer(PlayerController player, bool showToSelf)
    {
        GeneralHelper.SetLayerRecursively(gameObject, showToSelf ? player.ArmsLayerID : player.BodyLayerID);
    }
}
