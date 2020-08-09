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
    public PlayerController target;
    public float hitRange = 1f;

    [Header("Sight")]
    public float fov = 75f;
    public float sightRange = 20f;
    public float discoverySpeed = 1f;
    //[HideInInspector]
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

    public void Attack()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward * hitRange / 2, hitRange);
        foreach (Collider col in colliders)
        {
            PlayerController player = col.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ChangeHealth(-10);
                //Debug.Log("Hit player");
                break;
            }
        }
    }

    public void Die(Rigidbody rigidbody, Vector3 force)
    {
        ToggleRagdoll(true);
        rigidbody.AddForce(force);
        StartCoroutine(Dissolve(3f));
    }

    public void Damage(float damage, Rigidbody rigidbody, Vector3 force, PlayerController player)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hit");

        if (!animator.GetBool("Aggressive"))
            SpreadAggro(player);

        if (currentHealth <= 0)
        {
            Die(rigidbody, force);
        }
    }

    public void Aggro(PlayerController player)
    {
        animator.SetBool("Aggressive", true);
        AIDirector.SetInCombat(player, true);
    }

    public void SpreadAggro(PlayerController player)
    {
        Aggro(player);
        int layerMask = 1 << 16;
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange, layerMask);
        //Debug.Log("Colliders: " + colliders.Length);
        foreach (Collider col in colliders)
        {
            Hitbox hitbox = col.GetComponent<Hitbox>();
            if (hitbox == null) continue;
            hitbox.ZombieParent.Aggro(player);
        }
    }

    public void Look()
    {
        if (PlayerController.AllPlayers == null) return;
        bool seen = false;

        foreach(PlayerController player in PlayerController.AllPlayers)
        {
            //Debug.Log("Player " + player);
            if (Vector3.Distance(player.transform.position, transform.position) > sightRange) continue;
            Vector3 playerDir = ((player.transform.position + Vector3.down / 2) - transform.position).normalized;
            if (Vector3.Angle(transform.forward, playerDir) > fov / 2) continue;

            if (Physics.Raycast(transform.position + Vector3.up, playerDir, out RaycastHit hit, sightRange))
            {
                if (hit.transform != player.transform) continue;

                seen = true;
                playerDiscovery += Time.deltaTime * discoverySpeed * ((sightRange - hit.distance) / sightRange);

                if (playerDiscovery >= 1)
                    SpreadAggro(player);
            }
        }

        if (!seen)
        {
            if (playerDiscovery > 0)
                playerDiscovery -= Time.deltaTime;
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
}
