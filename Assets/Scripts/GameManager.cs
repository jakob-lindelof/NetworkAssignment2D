using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public Dictionary<ulong, NetworkObject> playerMap;
    public List<GameObject> playerList;
    private List<FixedString128Bytes> presetChatMessages;

    private NetworkVariable<int> playerScore = new();

    private NetworkVariable<bool> onScreenMessagePlayer1 = new();
    private NetworkVariable<float> messageTimePlayer1 = new();

    private NetworkVariable<bool> onScreenMessagePlayer2 = new();
    private NetworkVariable<float> messageTimePlayer2 = new();

    private NetworkVariable<Vector2> messagePosPlayer1 = new();
    private NetworkVariable<Vector2> messagePosPlayer2 = new();

    private NetworkVariable<int> player1Health = new();
    private NetworkVariable<int> player2Health = new();


    private bool messageOnScreen;

    [SerializeField] private Canvas gameUI;
    [SerializeField] private TMP_Text player1Score;
    [SerializeField] private TMP_Text player2Score;
    [SerializeField] private TMP_Text player1Message;
    [SerializeField] private TMP_Text player2Message;

    private void Awake()
    {
        playerList = new List<GameObject>();
        playerMap = new Dictionary<ulong, NetworkObject>();
        player1Health.OnValueChanged += UpdatePlayerHealth;
    }
    private void Start()
    {
        presetChatMessages = new List<FixedString128Bytes>
        {
            "Hello!",
            "U suck!",
            "Good Game!"
        };
        gameUI = GameObject.Find("Canvas").GetComponent<Canvas>();
        player1Message.text = "fgsdfg";
        player2Message.text = "sgdfgsdhsfg";
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
    private void UpdateMessageRPC(int messageIndex,  ulong clientId)
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
            
            default:
                break;
        }
    }

    [Rpc(SendTo.Server)]
    private void UpdatePlayerHealth(int previosuHealth, int newHealth)
    {

    }
}
