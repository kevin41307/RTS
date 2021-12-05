using Mirror;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Unit : NetworkBehaviour
{
    [SerializeField] private int resourceCost = 10;
    [SerializeField] private Health health = null;
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;
    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    public int GetResourceCost() => resourceCost;
    public UnitMovement GetUnitMovement() => unitMovement;
    public Targeter GetTargeter() => targeter;

    #region  Server
    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        health.ServerOnDie += ServerHandleDie;

    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);
        health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region  Client
    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);

    }

    public override void OnStopClient()
    {
        if (!hasAuthority) return;
        AuthorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        if (!hasAuthority) return;
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) return;
        onDeselected?.Invoke();
    }
    #endregion
}