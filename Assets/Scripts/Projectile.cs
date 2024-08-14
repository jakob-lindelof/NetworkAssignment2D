using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private NetworkVariable<Vector2> projectilePosition = new();
    public NetworkVariable<Vector2> velocity = new();

    public NetworkVariable<ulong> spawnedFromPlayerId;

    [SerializeField] NetworkVariable<float> projectileSpeed = new();
    [SerializeField] NetworkVariable<float> destroyTimer = new();
    //[SerializeField] private float projectileSpeed = 5f;
    //[SerializeField] private float destroyTimer = 1f;

    private void FixedUpdate()
    {
        if (IsServer)
        {
            Move();
            projectilePosition.Value = transform.position;
            destroyTimer.Value -= Time.fixedDeltaTime;
            if (destroyTimer.Value <= 0f)
            {
                DestroyProjectileRPC();
            }
        }
    }

    private void Move()
    {
        velocity.Value.Normalize();
        transform.position += (Vector3)velocity.Value * (Time.deltaTime * projectileSpeed.Value);
    }

    [Rpc(SendTo.Server)]
    private void DestroyProjectileRPC()
    {
        Destroy(this);
        NetworkObject obj = this.GetComponent<NetworkObject>();
        obj.Despawn();
    }
    
}
