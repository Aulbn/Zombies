using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public enum BodyPart
    {
        Head, Torso, Limb
    }
    public BodyPart bodyPart = BodyPart.Torso;

    public ZombieController ZombieParent { get; private set; }

    public void SetInfo(ZombieController zombieParent)
    {
        this.ZombieParent = zombieParent;
    }

    public void Damage(float damage, Vector3 force)
    {
        ZombieParent.Damage(CalculatedDamage(damage), GetComponent<Rigidbody>(), force);
    }

    public float CalculatedDamage(float damage)
    {
        float newDamage;
        switch (bodyPart)
        {
            case BodyPart.Head:
                newDamage = damage * 2;
                break;
            case BodyPart.Torso:
                newDamage = damage * 1;
                break;
            case BodyPart.Limb:
                newDamage = damage * 0.5f;
                break;
            default:
                newDamage = damage;
                break;
        }
        return newDamage;
    }
}
