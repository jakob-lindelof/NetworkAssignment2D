using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private NetworkVariable<Vector2> projectilePosition = new();

    [SerializeField] private float projectileSpeed = 5f;

    private void FixedUpdate()
    {
        if (IsServer)
        {
            Debug.Log("Server FixedUpdate");
            Move();
        }
    }

    private void Move()
    {
        projectilePosition.Value = Vector2.up;
        transform.position += (Vector3)projectilePosition.Value * (Time.deltaTime * projectileSpeed);
    }
}
