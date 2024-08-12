using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction shootAction;

    private NetworkVariable<Vector2> moveInput = new();

    private NetworkVariable<Vector2> mousePositionNormalized = new();

    private Vector2 mousePos;
    
    private Vector2 mousePosWorld;

    private Vector2 mousePosWorldNorm;

    [SerializeField] private GameObject bulletMuzzle;

    [SerializeField] private GameObject gameObjectoToSpawn;

    [SerializeField] private float playerSpeed = 10;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
        shootAction = playerInput.actions.FindAction("Shoot");
    }

    void Update()
    {
        if (IsLocalPlayer)
        {
            mousePos = Input.mousePosition;
            mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
            mousePosWorldNorm = mousePosWorld - (Vector2)transform.position;
            mousePosWorldNorm.Normalize();
            ReadInput(moveAction.ReadValue<Vector2>(), mousePosWorldNorm);
            
            Debug.DrawLine(transform.position, mousePosWorld, Color.blue);
        }
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            moveInput.Value.Normalize();
            mousePositionNormalized.Value.Normalize();
            transform.position += (Vector3)moveInput.Value * Time.deltaTime * playerSpeed;
            transform.up = mousePosWorldNorm;
        }
    }

    private void ReadInput(Vector2 input, Vector2 mouseDirection)
    {
        MoveRPC(input, mouseDirection);
        if (shootAction.WasPressedThisFrame())
        {
            SpawnRPC();
        }
    }
    
    
    [Rpc(SendTo.Server)]
    private void MoveRPC(Vector2 movement, Vector2 mouseDirection)
    {
        moveInput.Value = movement;
        mousePositionNormalized.Value = mouseDirection;
    }
    


    [Rpc(SendTo.Server)]
    private void SpawnRPC()
    {
        NetworkObject obj = Instantiate(gameObjectoToSpawn, bulletMuzzle.transform.position, Quaternion.identity).GetComponent<NetworkObject>();
        Projectile projectile = obj.GetComponent<Projectile>();
        projectile.velocity = mousePosWorldNorm;
        projectile.transform.rotation = transform.rotation;
        obj.Spawn();
    }
    
}
