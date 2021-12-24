using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
public class GameOverHandler : NetworkBehaviour
{
    public static event Action ServerOnGameOver;
    public static event Action<string> ClientOnGameOver;
    private List<UnitBase> bases = new List<UnitBase>();
    Dictionary<UnitBase, string> baseNames = new Dictionary<UnitBase, string>();
    #region Server
    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }
    [Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        bases.Add(unitBase);
        string playerName = unitBase.connectionToClient.identity.GetComponent<RTSPlayer>().GetDisplayName();
        baseNames.Add(unitBase, playerName);
        unitBase.name = playerName + " base";
    }
    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);
        if( bases.Count > 1) return;

        if(baseNames.TryGetValue(bases[0], out string winname))
        {
            //Debug.Log("w" + winname);
        }
        RpcGameOver($"{winname}");
        //RpcGameOver($"Player {playerId}");
        ServerOnGameOver?.Invoke();
    }
    #endregion
    #region Client
    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }



    #endregion
}
