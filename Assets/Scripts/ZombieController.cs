using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    [HideInInspector]public NavMeshAgent agent;

    public float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private SkinnedMeshRenderer skin;
    public float fov = 75f;
    public float sightRange = 20f;
    public PlayerController target;
    public float playerDiscovery = 0;

    public Animator animator;
    private bool ragdolling = false;
    private Collider[] colliders;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.gameObject.GetComponent<Hitbox>().SetInfo(this);
        }
        ToggleRagdoll(false);
    }

    private void Start()
    {
        currentHealth = maxHealth;
        SetDissolve(0, 0);
    }

    public void SetTarget()
    {
        if (target == null) target = PlayerController.GetClosestPlayer(transform.position);

        foreach(PlayerController player in PlayerController.AllPlayers)
        {
            if (Vector3.Angle(transform.forward, player.transform.position - transform.position) < fov / 2)
            {
                if (Physics.Raycast(transform.position, player.transform.position - transform.position, out RaycastHit hit, sightRange))
                {
                    if (Vector3.Distance(transform.position, player.transform.position) < Vector3.Distance(transform.position, target.transform.position))
                    {
                        target = player;
                    }
                }
            }
        }
    }

    public void Die(Rigidbody rigidbody, Vector3 force)
    {
        ToggleRagdoll(true);
        rigidbody.AddForce(force);
        StartCoroutine(Dissolve(3f));
    }

    public void Damage(float damage, Rigidbody rigidbody, Vector3 force)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hit");

        Aggro();

        if (currentHealth <= 0)
        {
            Die(rigidbody, force);
        }
    }

    private void Aggro()
    {
        animator.SetBool("Aggressive", true);
        //Raycast to other Zombies and Aggro them!
    }

    public void Look()
    {
        foreach(PlayerController player in PlayerController.AllPlayers)
        {
            if (Vector3.Distance(player.transform.position, transform.position) > sightRange) continue;
            // IF IN FOV
            //if (Quaternion.Angle)
            if (Physics.Raycast(transform.position + Vector3.up, player.transform.position - transform.position, out RaycastHit hit, sightRange))
            {
                if (hit.transform != player.transform) continue;

            }
        }
    }

    private void ToggleRagdoll(bool isRagdoll)
    {
        ragdolling = isRagdoll;
        foreach (Collider c in colliders)
        {
            c.isTrigger = !isRagdoll;
            animator.enabled = !isRagdoll;
            c.attachedRigidbody.isKinematic = !isRagdoll;
            agent.enabled = !isRagdoll;
        }
    }

    private void SetDissolve(float ammount, float height)
    {
        Vector2 dissolveHeight = new Vector2(.8f, -1f); //Arbitrary

        skin.material.SetFloat("_Dissolve", ammount);
        skin.material.SetFloat("_DissolveHeight", Mathf.Lerp(dissolveHeight.x, dissolveHeight.y, height));
    }

    private IEnumerator Dissolve(float time)
    {
        float currentTime = 0;
        float val;

        while (currentTime < time)
        {
            currentTime += Time.deltaTime;
            val = currentTime / time;
            SetDissolve(val* .5f, val);
            yield return null;
        }
        Destroy(gameObject);
    }

    //private void Update()
    //{
    //    if (currentHealth <= 0 && !ragdolling)
    //        ToggleRagdoll(true);
    //}
}
