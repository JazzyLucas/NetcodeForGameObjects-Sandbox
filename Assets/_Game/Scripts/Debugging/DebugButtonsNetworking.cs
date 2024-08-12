using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DebugButtonsNetworking : MonoBehaviour 
{
    private void OnGUI() 
    {
        GUILayout.BeginArea(new(10, 10, 500, 500));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) 
        {
            if (GUILayout.Button("Host"))
            {
                _ = RelayManager.Instance.StartHostWithRelay();
                //NetworkManager.Singleton.StartHost();
            }

            /*
            if (GUILayout.Button("Server"))
            {
                NetworkManager.Singleton.StartServer();
            }
            */

            if (GUILayout.Button("Client"))
            {
                _ = RelayManager.Instance.StartClientWithRelay();
                //NetworkManager.Singleton.StartClient();
            }
        }

        GUILayout.EndArea();
    }
}
