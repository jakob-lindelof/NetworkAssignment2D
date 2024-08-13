using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private NetworkVariable<Vector2> projectilePosition = new();
    public NetworkVariable<Vector2> velocity = new();


    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float destroyTimer = 1f;

    private void FixedUpdate()
    {
        if (IsServer)
        {
            Move();
            projectilePosition.Value = transform.position;
            destroyTimer -= Time.fixedDeltaTime;
            if (destroyTimer <= 0f)
            {
                DestroyProjectileRPC();
            }
        }
    }

    private void Move()
    {
        velocity.Value.Normalize();
        transform.position += (Vector3)velocity.Value * (Time.deltaTime * projectileSpeed);
    }

    [Rpc(SendTo.Server)]
    private void DestroyProjectileRPC()
    {
        Destroy(this);
        NetworkObject obj = this.GetComponent<NetworkObject>();
        obj.Despawn();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionRPC();
    }

    [Rpc(SendTo.Server)]
    private void CollisionRPC()
    {
        Debug.Log("Hit");
        if (IsServer)
        {
            this.NetworkObject.Despawn();
            Destroy(this);
        }
    }
}
