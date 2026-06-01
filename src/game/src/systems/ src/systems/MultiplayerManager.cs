using UnityEngine;
using System.Collections.Generic;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance { get; private set; }

    [SerializeField] private string networkAddress = "localhost:7777";
    [SerializeField] private int maxConnections = 1000;
    [SerializeField] private float connectionTimeout = 30f;

    private Dictionary<string, NetworkPlayer> connectedPlayers = new Dictionary<string, NetworkPlayer>();
    private bool isServerRunning = false;
    private bool isConnected = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartServer()
    {
        isServerRunning = true;
        Debug.Log($"Server started on {networkAddress}");
        OnServerStarted?.Invoke();
    }

    public void StopServer()
    {
        isServerRunning = false;
        Debug.Log("Server stopped");
        OnServerStopped?.Invoke();
    }

    public void ConnectToServer(string address)
    {
        networkAddress = address;
        isConnected = true;
        Debug.Log($"Connecting to server at {address}");
        OnConnectionEstablished?.Invoke();
    }

    public void Disconnect()
    {
        isConnected = false;
        connectedPlayers.Clear();
        Debug.Log("Disconnected from server");
        OnDisconnected?.Invoke();
    }

    public void FindMatch(int minRank, int maxRank)
    {
        Debug.Log($"Searching for match (Rank: {minRank}-{maxRank})");
        OnMatchSearchStarted?.Invoke();
    }

    public void CancelMatchSearch()
    {
        Debug.Log("Match search cancelled");
        OnMatchSearchCancelled?.Invoke();
    }

    public void SendMatchData(string opponentId, int selectedSharkId)
    {
        Debug.Log($"Sending match data to {opponentId}");
    }

    public void ReceiveMatchData(NetworkMatchData matchData)
    {
        Debug.Log($"Match data received from {matchData.OpponentId}");
        OnMatchDataReceived?.Invoke(matchData);
    }

    public bool IsConnected() => isConnected;
    public bool IsServerRunning() => isServerRunning;
    public int GetPlayerCount() => connectedPlayers.Count;

    public event System.Action OnServerStarted;
    public event System.Action OnServerStopped;
    public event System.Action OnConnectionEstablished;
    public event System.Action OnDisconnected;
    public event System.Action OnMatchSearchStarted;
    public event System.Action OnMatchSearchCancelled;
    public event System.Action<NetworkMatchData> OnMatchDataReceived;
}

public class NetworkPlayer
{
    public string PlayerId { get; set; }
    public string Username { get; set; }
    public int Rank { get; set; }
    public int SelectedSharkId { get; set; }
    public long LastHeartbeat { get; set; }
}

public class NetworkMatchData
{
    public string OpponentId { get; set; }
    public string OpponentUsername { get; set; }
    public int OpponentRank { get; set; }
    public int OpponentSharkId { get; set; }
    public long MatchStartTime { get; set; }
}
