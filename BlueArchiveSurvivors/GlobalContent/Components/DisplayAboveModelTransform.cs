using UnityEngine;
using RoR2;
internal class DisplayAboveModelTransform : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 2f, 0);
    public HealthComponent victimHealthComponent;
    public BuffDef parentBuff;

    private Collider victimCollider;
    private Transform victimTransform;

    void Start()
    {
        if (victimHealthComponent == null)
        {
            Destroy(gameObject);
            return;
        }

        victimTransform = victimHealthComponent.transform;
        victimCollider = victimHealthComponent.GetComponent<Collider>();

        if (victimCollider != null)
        {
            offset = new Vector3(0, victimCollider.bounds.extents.y + 0.5f, 0);
        }
    }

    void FixedUpdate()
    {
        if (victimHealthComponent == null || !victimHealthComponent.alive)
        {
            Destroy(gameObject);
            return;
        }

        if (parentBuff != null && !victimHealthComponent.body.HasBuff(parentBuff))
        {
            Destroy(gameObject);
            return;
        }

        if (victimTransform != null)
        {
            transform.position = victimTransform.position + offset;
        }
    }
}