using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction shootAction;
    
    private NetworkVariable<Vector2> moveInput = new(writePerm: NetworkVariableWritePermission.Owner);

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
            ReadInput();
            mousePos = Input.mousePosition;
            mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
            mousePosWorldNorm = mousePosWorld - (Vector2)transform.position;
            mousePosWorldNorm.Normalize();
            
            Debug.DrawLine(transform.position, mousePosWorld, Color.blue);
        }
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            transform.position += (Vector3)moveInput.Value * Time.deltaTime * playerSpeed;
            transform.up = mousePosWorldNorm;
        }
    }

    private void ReadInput()
    {
        moveInput.Value = moveAction.ReadValue<Vector2>();
        if (shootAction.WasPressedThisFrame())
        {
            SpawnRPC();
        }
    }
    
    
    [Rpc(SendTo.Server)]
    private void MoveRPC(Vector2 data)
    {
        moveInput.Value = data;
    }
    


    [Rpc(SendTo.Server)]
    private void SpawnRPC()
    {
        NetworkObject obj = Instantiate(gameObjectoToSpawn, bulletMuzzle.transform.position, Quaternion.identity).GetComponent<NetworkObject>();
        
        obj.Spawn();

    }
    
}
