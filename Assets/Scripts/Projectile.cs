using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private NetworkVariable<Vector2> projectilePosition = new();

    public Vector2 velocity;

    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float destroyTimer = 1f;

    private void Start()
    {
        DestroyProjectileRPC();
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            Move();
            projectilePosition.Value = transform.position;
        }
    }

    private void Move()
    {
        //projectilePosition.Value = Vector2.up;
        transform.position += (Vector3)velocity * Time.deltaTime * projectileSpeed;
    }

    [Rpc(SendTo.Server)]
    private void DestroyProjectileRPC()
    {
        Destroy(this, destroyTimer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer)
        {  return; }
        Destroy(this);
    }
}
