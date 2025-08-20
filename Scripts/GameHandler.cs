using UnityEngine;
using FishNet.Managing;
using Unity.Services.Relay.Models;
using FishNet.Transporting.UTP;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using FishNet.Component.Spawning;
using FishNet.Transporting;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;

    [Header("NETWORK MANAGERS")]
    [SerializeField] private GameObject networkManagerRelay;
    [SerializeField] private GameObject networkManagerOffline;

    [SerializeField] private Transform[] spawns = new Transform[0];

    private string joinCode;

    private void Awake()
    {
        instance = this;
    }

    public string GetJoinCode()
    {
        return joinCode;
    }

    public async void HostGame()
    {
        SpawnNetworkManager(true);

        MainMenuUI.instance.OpenTab(MainMenuUI.Tab.connecting);
        MainMenuUI.instance.SetConnectingText("STARTING HOST...", false);


        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn) await AuthenticationService.Instance.SignInAnonymouslyAsync();

        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(7);
            string code = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("Join Code: " + code);
            joinCode = code;

            NetworkManager.instance.TransportManager.GetTransport<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));

            NetworkManager.instance.ServerManager.StartConnection();
            NetworkManager.instance.ClientManager.StartConnection();
        }
        catch (RelayServiceException ex)
        {
            MainMenuUI.instance.SetConnectingText("STARTING HOST FAILED", true);
            DestroyNetworkManager();
        }


    }

    public async void JoinGame()
    {
        SpawnNetworkManager(true);

        MainMenuUI.instance.OpenTab(MainMenuUI.Tab.connecting);
        MainMenuUI.instance.SetConnectingText("JOINING GAME...", false);

        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn) await AuthenticationService.Instance.SignInAnonymouslyAsync();

        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(MainMenuUI.instance.GetJoinCode());
            NetworkManager.instance.TransportManager.GetTransport<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));
            NetworkManager.instance.ClientManager.StartConnection();
        }
        catch (RelayServiceException ex)
        {
            MainMenuUI.instance.SetConnectingText("JOINING GAME FAILED", true);
            DestroyNetworkManager();
        }
    }

    public void HostOfflineGame()
    {
        SpawnNetworkManager(false);

        MainMenuUI.instance.OpenTab(MainMenuUI.Tab.connecting);
        MainMenuUI.instance.SetConnectingText("LOADING...", false);

        NetworkManager.instance.ServerManager.StartConnection();
        NetworkManager.instance.ClientManager.StartConnection();
    }

    private void SpawnNetworkManager(bool usingRelay)
    {
        GameObject networkManager = Instantiate(usingRelay ? networkManagerRelay : networkManagerOffline);

        PlayerSpawner.instance.Spawns = spawns;

        networkManager.GetComponent<NetworkManager>().ClientManager.OnClientConnectionState += HandleClientConnectionState;
        networkManager.GetComponent<NetworkManager>().ServerManager.OnServerConnectionState += HandleServerConnectionState;
    }

    private void HandleClientConnectionState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            if (MainMenuUI.instance.IsConnecting()) MainMenuUI.instance.SetConnectingText("CONNECTION FAILED", true);
            DestroyNetworkManager();
        }
    }

    private void HandleServerConnectionState(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            if (MainMenuUI.instance.IsConnecting()) MainMenuUI.instance.SetConnectingText("SERVER FAILED TO START", true);
            DestroyNetworkManager();
        }
    }

    public void DestroyNetworkManager()
    {
        Destroy(NetworkManager.instance.gameObject);
    }

    public void Disconnect()
    {
        NetworkManager.instance.ClientManager.StopConnection();
        NetworkManager.instance.ServerManager.StopConnection(false);

        CleanUp();

        UI.instance.OpenMainMenu();
    }

    public void CleanUp()
    {
        foreach (Stone stone in FindObjectsByType<Stone>(FindObjectsSortMode.None))
        {
            Destroy(stone.gameObject);
        }

        foreach (BounceNumber bounceNumber in FindObjectsByType<BounceNumber>(FindObjectsSortMode.None))
        {
            Destroy(bounceNumber.gameObject);
        }
    }
}
