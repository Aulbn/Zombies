using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunHandler : WeaponHandler
{
    [Header("Gun Specific")]
    public int pellets = 8;
    public float randomAngle = 1.3f;

    protected override void WeaponFire(bool aimAssist)
    {
        RaycastHit hit;
        Ray ray = new Ray(owner.cam.transform.position, owner.cam.transform.forward);

        //Debug
        //int head = 0;
        //int torso = 0;
        //int limbs = 0;

        if (aimAssist)
            ray = AimAssistRayCorrection(ray);

        for (int i = 0; i < pellets; i++)
        {
            Vector3 direction = Quaternion.AngleAxis(Random.Range(-randomAngle/2, randomAngle/2), owner.cam.transform.right) * owner.cam.transform.forward;
            direction = Quaternion.AngleAxis(Random.Range(-randomAngle/2, randomAngle/2), owner.cam.transform.up) * direction;

            //Debug
            //Debug.DrawRay(owner.cam.transform.position, direction * weapon.fireRange);


            if (Physics.Raycast(owner.cam.transform.position, direction, out hit, weapon.fireRange, owner.ShootingLayerMask))
            {
                if (weapon.hitEffect != null)
                {
                    ParticleSystem ps = Instantiate(weapon.hitEffect, hit.point, Quaternion.Euler(hit.normal)).GetComponent<ParticleSystem>();
                    ps.transform.rotation = Quaternion.LookRotation(hit.normal);
                }

                Hitbox hitbox = hit.transform.GetComponent<Hitbox>();
                if (hitbox != null)
                {
                    //    switch (hitbox.bodyPart)
                    //    {
                    //        case Hitbox.BodyPart.Head:
                    //            head++;
                    //            break;
                    //        case Hitbox.BodyPart.Torso:
                    //            torso++;
                    //            break;
                    //        case Hitbox.BodyPart.Limb:
                    //            limbs++;
                    //            break;
                    //    }
                    float damageAfterRange = weapon.GetDamage(Vector3.Distance(transform.position, hit.point)) / pellets; //Ska jag dividera med antalet pellets? Blir enklare att räkna om jag kan göra så.
                    hitbox.Damage(damageAfterRange, owner.cam.transform.forward * damageAfterRange * 700, owner);
                }
            }
        }
        //Debug.Log("Head: " + head + " | Torso: " + torso + " | Limbs: " + limbs + " | Missed: " + (pellets - head-torso-limbs));
    }
}
