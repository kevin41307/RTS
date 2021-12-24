using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
public class Building : NetworkBehaviour
{
    [SerializeField] private GameObject buildingPreview = null;
    [SerializeField] private Sprite icon = null;
    [SerializeField] private HealthDisplay healthDisplay = null;
    [SerializeField] private BuildingLimit buildingLimit = null;
    [SerializeField] private RTSPlayer player = null;
    [SerializeField] private Explorer explorer = null;
    [SerializeField] private GameObject explorerVisualMask = null;
    //[SerializeField] private GameObject vfx_buildingLimitParent;

    [Header("Building Params")]
    [SerializeField] private int id = -1;
    [SerializeField] private int price = 100;

    //public GameObject GetBunildingLimitParent() => vfx_buildingLimitParent;

    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;
    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;

    
    public GameObject GetBuildingPreview() => buildingPreview;
    public Sprite GetIcon() => icon;
    public int GetId() => id;
    public int GetPrice() => price;

    private bool isPointerOver;

    private void Awake()
    {
        if (((RTSNetworkManager)NetworkManager.singleton).DEBUG_MODE == false)
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>(); //pass data from network gameobject to non-network gameobject by store data in RTSPlayer
    }

    private void Update()
    {
        try
        {
            if (player == null)
            {
                player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
        //Debug.Log(transform.name);

    }

    #region Server
    public override void OnStartServer()
    {
        ServerOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBuildingDespawned?.Invoke(this);
    }
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        AuthorityOnBuildingSpawned?.Invoke(this);

        BuildingButton.OnBuildingModeStarted += Building_OnBuildingModeStarted;
        BuildingButton.OnBuildingModeEnded += Building_OnBuildingModeEnded;
        if(explorer != null)
        {
            explorer.enabled = true;
            explorerVisualMask.SetActive(true);
        }
    }
    public override void OnStartClient()
    {
        healthDisplay.OnPointerEntered += Building_OnPointerEntered;
        healthDisplay.OnPointerExited += Building_OnPointerExited;

    }
    public override void OnStopClient()
    {
        if (!hasAuthority) return;
        AuthorityOnBuildingDespawned?.Invoke(this);
        healthDisplay.OnPointerEntered -= Building_OnPointerEntered;
        healthDisplay.OnPointerExited -= Building_OnPointerExited;
        BuildingButton.OnBuildingModeStarted -= Building_OnBuildingModeStarted;
        BuildingButton.OnBuildingModeEnded -= Building_OnBuildingModeEnded;
    }

    [Client]
    private void Building_OnBuildingModeStarted()
    {
        if(player.GetMyBuildings().Contains(this))
            buildingLimit.Show();
    }
    [Client]
    private void Building_OnBuildingModeEnded()
    {
        if (player.GetMyBuildings().Contains(this))
            buildingLimit.Hidden();
    }

    private void Building_OnPointerEntered()
    {
        isPointerOver = true;
        Debug.Log("Building_OnPointerEntered");
        healthDisplay.DisplayOrNot(isPointerOver);
    }
    private void Building_OnPointerExited()
    {
        isPointerOver = false;
        Debug.Log("Building_OnPointerExited");
        healthDisplay.DisplayOrNot(isPointerOver);
    }
    #endregion

}
