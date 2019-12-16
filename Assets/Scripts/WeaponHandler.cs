using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    protected PlayerController owner;
    public Weapon weapon;

    [Header("Ammo")]
    public int currentAmmo;
    private float rememberShootTimer = 0;
    private float rememberShootTime = 0.1f;
    private IEnumerator ShootRemember;
    private IEnumerator ReloadCoroutine;
    private IEnumerator UnholsterCoroutine;

    [Header("Physical")]
    public WeaponEntity weaponHand;
    public WeaponEntity weaponBody;
    private MultiAudioSource audioSource;

    [Header("Animation")]
    public float armAimHeight;
    private bool isUnholstered = true;

    //private InventoryItemDisplay uiElement;
    private float fireRateTimer = 0;
    public bool CanShoot { get { return fireRateTimer <= 0 && currentAmmo > 0 && rememberShootTimer > 0 && isUnholstered; } }

    private void Start()
    {
        audioSource = gameObject.AddComponent<MultiAudioSource>();
    }

    private void Update()
    {
        TimersCounter();

        if (CanShoot)
            Shoot();
    }

    public void SetOwner(PlayerController player)
    {
        owner = player;
        weaponHand.SetLayer(owner, true);
        weaponBody.SetLayer(owner, false);

        currentAmmo = weapon.magSize; //Borde ändras ifall ammo sparas i vapnet när det droppas (vilket det borde)
    }

    private void TimersCounter()
    {
        if (rememberShootTimer > 0)
            rememberShootTimer -= Time.deltaTime;
        if (fireRateTimer > 0)
            fireRateTimer -= Time.deltaTime;
    }

    private void ResetShootTimers()
    {
        fireRateTimer = weapon.fireRate;
        rememberShootTimer = 0;
    }

    public void TryShoot()
    {
        rememberShootTimer = rememberShootTime;
    }

    private void Shoot()
    {
        if (ReloadCoroutine != null) StopCoroutine(ReloadCoroutine); //Ifall jag vill avbryta mitt i en reload
        currentAmmo--;

        weaponHand.Shoot();
        weaponBody.Shoot();

        audioSource.PlayOneShot(weapon.gunSound, 0.5f);
        ResetShootTimers();

        WeaponFire(owner.aimAssist); //Raycast

        owner.ShootCallback(weapon.recoilAmmount);
    }

    protected virtual void WeaponFire(bool aimAssist)
    {
        RaycastHit hit;
        Ray ray = new Ray(owner.cam.transform.position, owner.cam.transform.forward);

        if (aimAssist)
            ray = AimAssistRayCorrection(ray);

        if (Physics.Raycast(ray, out hit, weapon.fireRange, owner.ShootingLayerMask))
        {
            if (weapon.hitEffect != null)
            {
                ParticleSystem ps = Instantiate(weapon.hitEffect, hit.point, Quaternion.Euler(hit.normal)).GetComponent<ParticleSystem>();
                ps.transform.rotation = Quaternion.LookRotation(hit.normal);
            }

            Hitbox hitbox = hit.transform.GetComponent<Hitbox>();
            if (hitbox != null)
            {
                float damageAfterRange = weapon.GetDamage(Vector3.Distance(transform.position, hit.point));
                hitbox.Damage(damageAfterRange, owner.cam.transform.forward * damageAfterRange * 700);
            }
        }
    }

    protected Ray AimAssistRayCorrection(Ray ray)
    {
        //Boxcast
        if (Physics.SphereCast(ray, owner.aimAssistRadius, out RaycastHit sphereHit, owner.aimAssistRange, owner.ShootingLayerMask))
        {
            Hitbox hitbox = sphereHit.transform.GetComponent<Hitbox>();
            if (hitbox != null)
            {
                float maxAngle = Mathf.Asin(owner.aimAssistRadius / sphereHit.distance);
                float enemyAngle = GeneralHelper.AngleAroundAxis(owner.cam.transform.forward, hitbox.ZombieParent.transform.position - owner.cam.transform.position, Vector3.up);
                maxAngle = maxAngle * Mathf.Rad2Deg * -(enemyAngle / Mathf.Abs(enemyAngle));

                Vector3 maxRot = Quaternion.AngleAxis(maxAngle, Vector3.up) * owner.cam.transform.forward;
                Vector3 enemyRot = Quaternion.AngleAxis(-enemyAngle, Vector3.up) * owner.cam.transform.forward;

                if (Mathf.Abs(GeneralHelper.AngleAroundAxis(ray.direction, enemyRot, Vector3.up)) < Mathf.Abs(maxAngle))
                    ray = new Ray(owner.cam.transform.position, enemyRot);
                else
                    ray = new Ray(owner.cam.transform.position, maxRot);
            }
        }
        return ray;
    }

    public void Unholster()
    {
        isUnholstered = false;
        StartCoroutine(UnholsterCoroutine = WaitForAction(weapon.reloadTime, () => { isUnholstered = true; }));
    }

    public void Reload()
    {
        if (currentAmmo >= weapon.magSize) return; //Kanske borde kolla ifall man kan skjuta också, så att man inte kan ladda om under en recoil, t.ex.

        if (ReloadCoroutine != null) StopCoroutine(ReloadCoroutine);
        StartCoroutine(ReloadCoroutine = WaitForAction(weapon.reloadTime, () => { currentAmmo = weapon.magSize; owner.PlayerUI.SetAmmoText(this); }));

    }

    private IEnumerator WaitForAction(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    public void Show(bool value)
    {
        if (!value && ReloadCoroutine != null) StopCoroutine(ReloadCoroutine);
        weaponHand.gameObject.SetActive(value);
        weaponBody.gameObject.SetActive(value);
    }

    public void Drop()
    {
        Rigidbody r = Instantiate(weapon.weaponPickupPrefab, owner.cam.transform.position + owner.cam.transform.forward, Quaternion.identity).GetComponent<Rigidbody>();
        r.AddForce(owner.cam.transform.forward * 1.5f);
        Destroy(weaponHand.gameObject);
        Destroy(weaponBody.gameObject);
        Destroy(gameObject);
    }
}
