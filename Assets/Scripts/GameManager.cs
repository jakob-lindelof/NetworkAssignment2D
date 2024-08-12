using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public Dictionary<GameObject, ulong> playerMap;
    public List<GameObject> playerList;
    private List<FixedString128Bytes> presetChatMessages;

    private NetworkVariable<int> playerScore = new();

    private NetworkVariable<float> messageTime = new();

    private NetworkVariable<Vector2> messagePosPlayer1 = new();
    private NetworkVariable<Vector2> messagePosPlayer2 = new();

    private bool messageOnScreen;

    [SerializeField] private Canvas gameUI;
    [SerializeField] private TMP_Text player1Score;
    [SerializeField] private TMP_Text player2Score;
    [SerializeField] private TMP_Text player1Message;
    [SerializeField] private TMP_Text player2Message;


    private void Start()
    {
        playerList = new List<GameObject>();
        presetChatMessages = new List<FixedString128Bytes>();
        presetChatMessages.Add("Hello!");
        presetChatMessages.Add("U suck!");
        presetChatMessages.Add("Good Game!");
        gameUI = GameObject.Find("Canvas").GetComponent<Canvas>();
        player1Message.text = "fgsdfg";
        player2Message.text = "sgdfgsdhsfg";
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            if (messageOnScreen)
            {
                messageTime.Value -= Time.fixedDeltaTime;
                if (messageTime.Value <= 0f)
                {
                    player1Message.text = "";
                    player2Message.text = "";
                    messageOnScreen = false;
                }
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void SubmitMessageRPC(int index, int playerIndex)
    {
        UpdateMessageRPC(index, playerIndex);
        messageTime.Value = 4f;
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateMessageRPC(int messageIndex, int playerIndex)
    {
        switch (playerIndex)
        {
            case 0:
                player1Message.text = presetChatMessages[messageIndex].ToString();
                break;
            case 1:
                player2Message.text = presetChatMessages[messageIndex].ToString();
                break;
            
            default:
                break;
        }
        
        messageOnScreen = true;
    }
}
