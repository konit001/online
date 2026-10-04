using UnityEngine;
using Unity.Netcode;

public class ThrowableItem : NetworkBehaviour
{
    [Header("Knockback Setting")]
    [SerializeField] float knockbackForce = 12f;

    private ulong throwerClientId;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetThrower(ulong clientId)
    {
        throwerClientId = clientId;
    }

    void OnCollisionEnter(Collision other)
    {
        if (!IsServer) return;

        playerControl hitPlayer = other.gameObject.GetComponentInParent<playerControl>();
        if (hitPlayer == null) return;

        if (hitPlayer.OwnerClientId == throwerClientId) return;

        Vector3 direction = rb != null && rb.linearVelocity.sqrMagnitude > 0.0001f
            ? rb.linearVelocity.normalized
            : (hitPlayer.transform.position - transform.position).normalized;

        hitPlayer.ApplyKnockback(direction * knockbackForce);
    }
}
