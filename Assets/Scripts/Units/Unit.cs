using Mirror;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Unit : NetworkBehaviour
{
    [SerializeField] private int resourceCost = 10;
    [SerializeField] private Health health = null;
    [SerializeField] private HealthDisplay healthDisplay = null;
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private Explorer explorer = null;
    [SerializeField] private GameObject explorerVisualMask = null;

    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;
    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    public int GetResourceCost() => resourceCost;
    public UnitMovement GetUnitMovement() => unitMovement;
    public Targeter GetTargeter() => targeter;

    private bool isSelected;
    private bool isPointerOver;

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
        explorer.enabled = false;
        //Unit Clear RT
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region  Client
    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
        if (explorer != null)
        {
            explorer.enabled = true;
            explorerVisualMask.SetActive(true);
        }
    }

    public override void OnStartClient()
    {
        
        healthDisplay.OnPointerEntered += ClientHandlePointerEntered;
        healthDisplay.OnPointerExited += ClientHandlePointerExited;
        
    }
    public override void OnStopClient()
    {
        if (!hasAuthority) return;
        AuthorityOnUnitDespawned?.Invoke(this);
        healthDisplay.OnPointerEntered -= ClientHandlePointerEntered;
        healthDisplay.OnPointerExited -= ClientHandlePointerExited;
    }

    private void ClientHandlePointerEntered()
    {
        isPointerOver = true;
        healthDisplay.DisplayOrNot(isSelected || isPointerOver);
    }

    private void ClientHandlePointerExited()
    {
        isPointerOver = false;
        healthDisplay.DisplayOrNot(isSelected || isPointerOver);
    }

    [Client]
    public void Select()
    {
        if (!hasAuthority) return;
        isSelected = true;
        onSelected?.Invoke();
        healthDisplay.DisplayOrNot(isSelected || isPointerOver);
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) return;
        isSelected = false;
        onDeselected?.Invoke();
        healthDisplay.DisplayOrNot(isSelected || isPointerOver);
    }
    #endregion
}