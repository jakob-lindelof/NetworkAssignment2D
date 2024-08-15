using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : NetworkBehaviour
{
    public Dictionary<ulong, NetworkObject> playerMap;
    private List<FixedString128Bytes> presetChatMessages;
    
    private NetworkVariable<bool> onScreenMessagePlayer1 = new();
    private NetworkVariable<float> messageTimePlayer1 = new();

    private NetworkVariable<bool> onScreenMessagePlayer2 = new();
    private NetworkVariable<float> messageTimePlayer2 = new();

    private NetworkVariable<int> player1Health = new(5);
    private NetworkVariable<int> player2Health = new(5);

    public NetworkVariable<bool> winnerDecided = new(false);

    public UnityAction<ulong> playerHit;

    public UnityAction playerWin;


    private bool messageOnScreen;

    [SerializeField] private Canvas gameUI;
    [SerializeField] private TMP_Text player1HealthTMP;
    [SerializeField] private TMP_Text player2HealthTMP;
    [SerializeField] private TMP_Text player1Message;
    [SerializeField] private TMP_Text player2Message;
    [SerializeField] private TMP_Text winnerMessage;

    private void Awake()
    {
        playerMap = new Dictionary<ulong, NetworkObject>();
        player1Health.OnValueChanged += SubmitPlayer1HealthRPC;
        player2Health.OnValueChanged += SubmitPlayer2HealthRPC;
        playerWin += PlayerWinRPC;
    }

 

    private void Start()
    {
        presetChatMessages = new List<FixedString128Bytes>
        {
            "Hello!",
            "U suck!",
            "Good Game!"
        };
        player1Message.text = "";
        player2Message.text = "";
        winnerMessage.text = "";
        playerHit += CollisionRPC;
        if (IsServer)
        {
            SubmitPlayer1HealthRPC(5, 5);
            SubmitPlayer2HealthRPC(5,5);
        }
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            if (onScreenMessagePlayer1.Value)
            {
                messageTimePlayer1.Value -= Time.fixedDeltaTime;
                if (messageTimePlayer1.Value <= 0f)
                {
                    player1Message.text = "";
                    onScreenMessagePlayer1.Value = false;
                }
            }

            if (onScreenMessagePlayer2.Value)
            {
                messageTimePlayer2.Value -= Time.fixedDeltaTime;
                if (messageTimePlayer2.Value <= 0f)
                {
                    player2Message.text = "";
                    onScreenMessagePlayer2.Value = false;
                }
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void SubmitMessageRPC(int index, ulong clientId)
    {
        UpdateMessageRPC(index, clientId);
        switch (clientId)
        {
            case 0:
                messageTimePlayer1.Value = 4f;
                break;
            case 1:
                messageTimePlayer2.Value = 4f;
                break;
        }
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateMessageRPC(int messageIndex, ulong clientId)
    {
        switch (clientId)
        {
            case 0:
                player1Message.text = presetChatMessages[messageIndex].ToString();
                onScreenMessagePlayer1.Value = true;
                break;
            case 1:
                player2Message.text = presetChatMessages[messageIndex].ToString();
                onScreenMessagePlayer2.Value = true;
                break;
        }
    }

    [Rpc(SendTo.Server)]
    private void SubmitPlayer1HealthRPC(int previousHealth, int newHealth)
    {
        if (newHealth <= 0)
        {
            playerWin.Invoke();
        }
        UpdatePlayer1HealthRPC(newHealth);
    }

    [Rpc(SendTo.Everyone)]
    private void UpdatePlayer1HealthRPC(int newHealth)
    {
        player1HealthTMP.text = "P1 Health: " + newHealth;
    }

    [Rpc(SendTo.Server)]
    private void SubmitPlayer2HealthRPC(int previousHealth, int newHealth)
    {
        if (newHealth <= 0)
        {
            playerWin.Invoke();
        }
        UpdatePlayer2HealthRPC(newHealth);
    }

    [Rpc(SendTo.Everyone)]
    private void UpdatePlayer2HealthRPC(int newHealth)
    {
        player2HealthTMP.text = "P2 Health: " + newHealth;
    }
    
    
    [Rpc(SendTo.Server)]
    private void CollisionRPC(ulong clientId)
    {
        if (IsServer)
        {
            switch (clientId)
            {
                case 0:
                    Debug.Log("Collided with player 1");
                    player1Health.Value -= 1;
                    break;

                case 1: 
                    Debug.Log("Collided with player 2");
                    player2Health.Value -= 1;
                    break;
            }
        }
    }
    
    [Rpc(SendTo.Server)]
    private void PlayerWinRPC()
    {
        if (IsServer)
        {
            winnerDecided.Value = true;

            DisplayWinnerRPC();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void DisplayWinnerRPC()
    {
        if (player1Health.Value <= 0)
        {
            winnerMessage.text = "Player 2 Wins!";
        }

        if (player2Health.Value <= 0)
        {
            winnerMessage.text = "Player 1 Wins!";
        }
    }
}
