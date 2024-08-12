using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JazzyLucas.Core.Utils;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using L = JazzyLucas.Core.Utils.Logger;

public class RelayManager : Singleton<RelayManager>
{
    private const string JOINCODE_FILE_NAME = "JoinCode.txt";
    
    private UnityTransport UnityTransport => NetworkManager.Singleton.GetComponent<UnityTransport>();

    protected override void Init()
    {
    }

    public async Task<string> StartHostWithRelay(int maxConnections = 5)
    {
        L.Log("Starting host with Relay...");
        
        await UnityServices.InitializeAsync();
        await HandleSignOn();

        var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        UnityTransport.SetRelayServerData(new(allocation, "dtls"));

        if (NetworkManager.Singleton.StartHost())
        {
            // Save join code to a file in the Assets folder
            var filePath = Path.Combine(Path.GetDirectoryName(Application.dataPath) ?? throw new InvalidOperationException(), JOINCODE_FILE_NAME);
            await File.WriteAllTextAsync(filePath, joinCode);
        
            L.Log("Started host with Relay");
            return joinCode;
        }
        
        L.Log("Failed to start host with Relay");
        return null;
    }

    public async Task<bool> StartClientWithRelay()
    {
        L.Log("Starting client with Relay...");
        
        await UnityServices.InitializeAsync();
        await HandleSignOn();

        // Read join code from file in the Assets folder
        var filePath = Path.Combine(Path.GetDirectoryName(Application.dataPath) ?? throw new InvalidOperationException(), JOINCODE_FILE_NAME);
        string joinCode = await File.ReadAllTextAsync(filePath);

        var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        UnityTransport.SetRelayServerData(new(allocation, "dtls"));
        
        L.Log("Started client with Relay");

        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    private async Task HandleSignOn()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            L.Log("Signing into Relay anon...");
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            L.Log("Signed into Relay anon.");
        }
        else
        {
            L.Log("Already signed into Relay anon.");
        }
    }
}
