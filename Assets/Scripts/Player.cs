using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;

public class Player : NetworkBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction shootAction;
    private InputAction chatMessage1;
    private InputAction chatMessage2;
    private InputAction chatMessage3;

    private UnityAction<int> SendChatEvent;

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
        chatMessage1 = playerInput.actions.FindAction("Chat Message 1");
        chatMessage2 = playerInput.actions.FindAction("Chat Message 2");
        chatMessage3 = playerInput.actions.FindAction("Chat Message 3");
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.playerList.Add(gameObject);
        gm.playerMap.Add(OwnerClientId, NetworkObject);
        Debug.Log(gm.playerList.Count);
        Debug.Log("ownerclientId: " + OwnerClientId);
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
            transform.position += (Vector3)moveInput.Value * (Time.deltaTime * playerSpeed);
            transform.up = mousePositionNormalized.Value;
        }
    }

    private void ReadInput(Vector2 input, Vector2 mouseDirection)
    {
        //Movement
        MoveRPC(input, mouseDirection);

        //Shooting
        if (shootAction.WasPressedThisFrame())
        {
            SpawnRPC();
        }

        //Chat messages
        if (chatMessage1.WasPressedThisFrame())
        {
            SendChat(0);
            Debug.Log("1");
        }
        if (chatMessage2.WasPressedThisFrame())
        {
            SendChat(1);
            Debug.Log("2");
        }
        if (chatMessage3.WasPressedThisFrame())
        {
            SendChat(2);
            Debug.Log("3");
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
        projectile.velocity.Value = mousePosWorldNorm;
        projectile.transform.rotation = transform.rotation;
        obj.Spawn();
    }

    private void SendChat(int index)
    {
        
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        /*
        int playerIndex = -1;
        for (int i = 0; i < gm.playerMap.Count; i++)
        {
            if (gm.playerMap[i] = NetworkObject)
            {
                playerIndex = i;
            }
        }
        */
        gm.SubmitMessageRPC(index, OwnerClientId);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TriggerCheckTag(collision.gameObject);
    }

    private void TriggerCheckTag(GameObject go)
    {
        if (!IsServer)
        { return; }
        if (go.CompareTag("Projectile"))
        {
            CollisionRPC(OwnerClientId);
            go.GetComponent<NetworkObject>().Despawn();
            Destroy(go);
        }
    }

    [Rpc(SendTo.Server)]
    private void CollisionRPC(ulong clientId)
    {
        if (IsServer)
        {
            switch (clientId)
            {
                case 0:

                    break;

                case 1: 

                    break;
                default:
                    break;
            }
        }
    }

}
