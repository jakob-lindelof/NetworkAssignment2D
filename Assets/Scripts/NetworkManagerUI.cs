using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    private void OnGUI()
    {
        if (GUILayout.Button("Host"))
        {
            networkManager.StartHost();
        }
        
        if (GUILayout.Button("Join"))
        {
            networkManager.StartClient();
        }

        if (GUILayout.Button("Disconnect"))
        {
            networkManager.DisconnectClient(networkManager.LocalClientId);
        }

        if (GUILayout.Button("Shutdown"))
        {
            networkManager.Shutdown();
        }

        if (GUILayout.Button("Quit"))
        {
            Application.Quit();
        }
    }
}
