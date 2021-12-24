using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Targeter))]
public class AutoTargeter : NetworkBehaviour
{
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private UnitFiring unitFiring = null;
    [SerializeField] private LayerMask blockLayer = new LayerMask();
    public bool startAutoTargeter = false;
    private float fireRange = 0;
    private Collider[] hits = new Collider[10];
    #region Server
    public override void OnStartServer()
    {
        fireRange = unitFiring.GetFireRange();
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        InvokeRepeating(nameof(AutoDetect), 1f, .8f);
    }
    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        CancelInvoke();
    }

    [Server]
    private void AutoDetect()
    {
        if (startAutoTargeter == false) return;
        if (targeter.GetTarget() != null) targeter.ClearAutoTarget();
        if (unitMovement.isMoving) return;
        int count = Physics.OverlapSphereNonAlloc(transform.position, fireRange, hits, blockLayer, QueryTriggerInteraction.Ignore);
        if (count <= 1) return; // only itself
        float minDistance = 9999f;
        Collider nextTargetable = null;

        for(int i = 0; i < count; i++)
        {
            if (hits[1].gameObject == null) break; //other hitted collider start at index1, if this is empty slot, break
            if (hits[i].gameObject == this.gameObject) continue; 
            if (hits[i].TryGetComponent<NetworkIdentity>(out NetworkIdentity identity)) // skip gameobject without identity and itself identity
            {
                if (connectionToClient.connectionId == identity.connectionToClient.connectionId)
                    continue;
            }
            else
                continue;

            float min = (hits[i].transform.position - transform.position).sqrMagnitude;
            if(min < minDistance)
            {
                minDistance = min;
                nextTargetable = hits[i];
            }
        }
        if (nextTargetable == null) return;
        
        targeter.ServerSetAutoTarget(nextTargetable.gameObject);

        
    }
    [Server]
    private void ServerHandleGameOver()
    {
        CancelInvoke();
    }

    #endregion

}
