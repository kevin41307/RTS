using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;
public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building building = null;
    [SerializeField] private Image iconImage = null;
    [SerializeField] private TMP_Text priceText = null;
    [SerializeField] private LayerMask floorMask = new LayerMask();
    private Camera mainCamera;
    private BoxCollider buildingCollider;
    private RTSPlayer player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingRendererInstance;
    public static event Action OnBuildingModeStarted;
    public static event Action OnBuildingModeEnded;


    private void Start()
    {
        mainCamera = Camera.main;
        iconImage.sprite = building.GetIcon();
        priceText.text = building.GetPrice().ToString();
        if(((RTSNetworkManager)NetworkManager.singleton).DEBUG_MODE == false) 
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>(); //pass data from network gameobject to non-network gameobject by store data in RTSPlayer

        buildingCollider = building.GetComponent<BoxCollider>();

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

        if (buildingPreviewInstance == null) return;
        UpdateBuildingPreview();
        //Debug.Log(transform.name);

    }
    public void OnPointerDown(PointerEventData eventData) //
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (player.GetResources() < building.GetPrice()) return;
        UnitSelectionHandler.isPreviewingBuilding = true;
        buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

        buildingPreviewInstance.SetActive(false);
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UnitSelectionHandler.isPreviewingBuilding = false;
        if (buildingPreviewInstance == null) return;
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            // place building
            player.CmdTryPlaceBuilding(building.GetId(), hit.point);
        }
        OnBuildingModeEnded?.Invoke();
        Destroy(buildingPreviewInstance);
    }


    private void UpdateBuildingPreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) return;
        buildingPreviewInstance.transform.position = hit.point;
        if (!buildingPreviewInstance.activeSelf)
        {
            buildingPreviewInstance.SetActive(true);
            OnBuildingModeStarted?.Invoke();
        }
        Color color = player.CanPlaceBuilding(buildingCollider, hit.point) ? Color.green : Color.red;
        buildingRendererInstance.material.SetColor("_BaseColor", color);
    }

}
