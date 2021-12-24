using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;
public class Targeter : NetworkBehaviour 
{
    [SerializeField] private Targetable target;
    [SerializeField] private Targetable autoTarget;

    public Targetable GetTarget() => target;
    public Targetable GetAutoTarget() => autoTarget;

    #region Server
    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }
    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }


    [Server]
    private void ServerHandleGameOver()
    {
        ClearTargets();
    }

    [Server]
    public void ServerSetTarget(GameObject targetGameObject)
    {
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable newTarget)) return;
        newTarget.AddChaser(this);
        target = newTarget;

    }
    [Server]
    public void ServerSetAutoTarget(GameObject targetGameObject)
    {
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable newTarget)) return;
        newTarget.AddChaser(this);
        autoTarget = newTarget;
    }

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        ServerSetTarget(targetGameObject);
        /*
        if(!targetGameObject.TryGetComponent<Targetable>(out Targetable newTarget)) return ;
        target = newTarget;
        */
    }

    [Server]
    public void ClearTarget()
    {
        if (target == null) return;
        target.RemoveChaser(this);
        target = null;

    }
    [Server]
    public void ClearAutoTarget()
    {
        if (autoTarget == null) return;
        autoTarget.RemoveChaser(this);
        autoTarget = null;
    }
    [Server]
    public void ClearTargets()
    {
        ClearAutoTarget();
        ClearTarget();
    }
    #endregion
}
