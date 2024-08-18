using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkDebugButtons : MonoBehaviour 
{
    private void OnGUI() 
    {
        GUILayout.BeginArea(new(10, 10, 500, 500));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) 
        {
            if (GUILayout.Button("Host"))
            {
                _ = RelayUtils.StartHostWithRelay();
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
                _ = RelayUtils.StartClientWithRelay();
                //NetworkManager.Singleton.StartClient();
            }
        }

        GUILayout.EndArea();
    }
}
