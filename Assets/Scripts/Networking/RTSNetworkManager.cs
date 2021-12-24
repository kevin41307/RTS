using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitBasePrefab = null;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;
    public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();
    private bool isGameInProgress = false;

    public bool DEBUG_MODE = false;

    public override void Start()
    {
        base.Start();
        if (((RTSNetworkManager)NetworkManager.singleton).DEBUG_MODE)
        {
            NetworkManager.singleton.StartHost();
        }

    }

    #region Server

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!isGameInProgress) return;
        conn.Disconnect();
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
        Players.Remove(player);
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Players.Clear();
        isGameInProgress = false;
    }

    public void StartGame()
    {
        //if(Players.Count < 2) return;
        isGameInProgress = true;
        ServerChangeScene("Scene_Map_01");
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
        Players.Add(player);
        player.SetDisplayName($"Player {Players.Count}");
        player.SetTeamColor(new Color(UnityEngine.Random.Range(0, .9f), UnityEngine.Random.Range(0, .9f), UnityEngine.Random.Range(0, .9f)));

        player.SetPartyOwner(Players.Count == 1);

        if(DEBUG_MODE)
        {
            GameObject unitBaseInstance = Instantiate(
                unitBasePrefab, 
                conn.identity.transform.position, 
                conn.identity.transform.rotation);
            NetworkServer.Spawn(unitBaseInstance, conn);
        }        
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
            foreach (RTSPlayer player in Players)
            {
                GameObject unitBaseInstance = Instantiate(
                    unitBasePrefab,
                    GetStartPosition().position,
                    Quaternion.identity);
                NetworkServer.Spawn(unitBaseInstance, player.connectionToClient);
            }
        }
    }
    #endregion
    #region Client

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();

    }


    #endregion




}
