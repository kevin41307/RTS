using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
public class UnitSpawner : NetworkBehaviour, IPointerDownHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private TMP_Text remainingUnitsText = null;
    [SerializeField] private Image unitProcessImage = null;
    [SerializeField] private int maxUnitQueue = 5;
    [SerializeField] private float spawnMoveRange = 7f;
    [SerializeField] private float unitSpawnDuration = 5f;

    [SyncVar(hook = nameof(ClientHandleQueueUnitsUpdated))]
    private int queueUnits;
    [SyncVar]
    private float unitTimer;
    private RTSPlayer player;
    private float processImageVelocity;

    private void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }

        if (isClient)
        {
            UpdateTimerDisplay();
        }

        /*
        unitTimer += Time.deltaTime;
        if(unitTimer > unitSpawnDuration)
        {
            queueUnits -= 1;
            unitTimer = 0;
        }
        */
    }



    #region Server
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }
    [Server]
    private void ProduceUnits()
    {
        if (queueUnits == 0) return;
        unitTimer += Time.deltaTime;
        if (unitTimer < unitSpawnDuration) return;
        GameObject unitInstance = Instantiate(
            unitPrefab.gameObject,
            unitSpawnPoint.position,
            unitSpawnPoint.rotation);
        NetworkServer.Spawn(unitInstance, connectionToClient);
        Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = unitSpawnPoint.position.y;

        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);
        queueUnits--;
        unitTimer = 0f;
    }


    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if (queueUnits == maxUnitQueue) return;
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();
        if(player.GetResources() < unitPrefab.GetResourceCost()) return;
        queueUnits++;
        player.SetRources(player.GetResources() - unitPrefab.GetResourceCost());
    }

    #endregion
    #region Client

    private void UpdateTimerDisplay()
    {
        float newProgress = unitTimer / unitSpawnDuration;
        if(newProgress < unitProcessImage.fillAmount)
        {
            unitProcessImage.fillAmount = newProgress;
        }
        else
        {
            unitProcessImage.fillAmount = Mathf.SmoothDamp(
                unitProcessImage.fillAmount,
                newProgress,
                ref processImageVelocity,
                0.1f
            );
        }
    }

    private void ClientHandleQueueUnitsUpdated(int oldUnits, int newUnits)
    {
        remainingUnitsText.text = newUnits.ToString();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!hasAuthority) return;

        CmdSpawnUnit();
    }

    #endregion
}

/*
public void OnPointerClick(PointerEventData eventData)
{
    Debug.Log("OnPointerClick");
    if(eventData.button != PointerEventData.InputButton.Left) return;
    if(!hasAuthority) return;

    CmdSpawnUnit();

}
*/