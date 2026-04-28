using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount = 25f;
    public string playerTag = "Player";

    [Header("Pickup Zone")]
    [Tooltip("Radius of the pickup trigger that's auto-added on Start.")]
    public float pickupRadius = 1f;
    [Tooltip("How high above the pivot the pickup sphere is centered.")]
    public float pickupHeight = 0.5f;

    [Header("Visual")]
    public float spinSpeed = 90f;

    [Header("Lifetime")]
    [Tooltip("How long before the pickup disappears on its own. 0 = never.")]
    public float lifetime = 15f;

    private bool consumed = false;

    void Awake()
    {
        SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = pickupRadius;
        trigger.center = new Vector3(0f, pickupHeight, 0f);
    }

    void Start()
    {
        if (lifetime > 0f) Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        TryPickup(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        TryPickup(collision.collider);
    }

    void TryPickup(Collider other)
    {
        if (consumed) return;

        Debug.Log($"{name}: entered by {other.name} (tag='{other.tag}')", this);

        if (!other.CompareTag(playerTag)) return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph == null) ph = other.GetComponentInParent<PlayerHealth>();
        if (ph == null)
        {
            Debug.LogWarning($"{name}: {other.name} is tagged Player but has no PlayerHealth component.", this);
            return;
        }

        ph.Heal(healAmount);
        consumed = true;
        Destroy(gameObject);
    }
}
