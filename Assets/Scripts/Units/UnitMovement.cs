using UnityEngine;
using Mirror;
using UnityEngine.AI;
using System;
public class UnitMovement : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private VFX_MovePath vfx_MovePath;

    [Header("Settings")]
    [SerializeField] private float chaseRange = 10f;
    
    //Private member
    [HideInInspector] public bool isMoving = false;
    private float maxChaseRange;
    private Vector3 holdPosition;
    private Targetable target = null;


    #region Server
    public override void OnStartServer()
    {
        maxChaseRange = 1.5f * chaseRange;
        holdPosition = transform.position;
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }
    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }
    [Server]
    private void ServerHandleGameOver()
    {
        if(agent.hasPath)
            agent.ResetPath();
        isMoving = false;
        holdPosition = transform.position;
        RpcClientHandleUnitResetPath();
    }

    [ServerCallback]
    private void Update()
    {
        //Chase User set target;
        target = targeter.GetTarget();
        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                agent.SetDestination(target.transform.position);
                isMoving = true;
                vfx_MovePath.SetPath( OffsetUpVector(transform.position), OffsetUpVector(target.transform.position));
            }
            else if (agent.hasPath)
            {
                agent.ResetPath();
                isMoving = false;
                holdPosition = transform.position;
                RpcClientHandleUnitResetPath();
            }
            return;
        }

        //Chase Auto Target
        if (target == null) target = targeter.GetAutoTarget();
        if (target != null)
        {
            float distanceSqr = (target.transform.position - holdPosition).sqrMagnitude;
            if (distanceSqr >= maxChaseRange * maxChaseRange)
            {
                targeter.ClearAutoTarget();
            }
            else if (distanceSqr > chaseRange * chaseRange && distanceSqr < maxChaseRange * maxChaseRange)
            {
                agent.SetDestination(target.transform.position);
                isMoving = true;
                vfx_MovePath.SetPath(OffsetUpVector(transform.position), OffsetUpVector(target.transform.position));
            }
            else if (agent.hasPath)
            {   
                agent.ResetPath();
                isMoving = false;
                RpcClientHandleUnitResetPath();
            }
            return;
        }


        if (!agent.hasPath) return;
        if (agent.remainingDistance > agent.stoppingDistance) return;
        agent.ResetPath();
        isMoving = false;
        holdPosition = transform.position;
        RpcClientHandleUnitResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        ServerMove(position);
    }

    [Server]
    public void ServerMove(Vector3 position)
    {
        targeter.ClearTarget();
        targeter.ClearAutoTarget();
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;
        agent.SetDestination(hit.position);
        //ClientHandleShowUnitMovePath(hit.position);
        vfx_MovePath.SetPath( OffsetUpVector(transform.position), OffsetUpVector(position));
        isMoving = true;
        
        RpcClientHandleShowUnitMovePath(hit.position);

    }
    #endregion

    #region Client
    [ClientRpc]
    public void RpcClientHandleShowUnitMovePath(Vector3 position)
    {
        //Debug.Log("ClientHandleShowUnitMovePath");
        vfx_MovePath.SetPath( OffsetUpVector(transform.position), OffsetUpVector(position));
        isMoving = true;
    }


    
    [ClientRpc]
    public void RpcClientHandleUnitResetPath()
    {
        //Debug.Log("ClientHandleUnitResetPath");
        vfx_MovePath.ClearPath();
        holdPosition = transform.position;
        isMoving = false;
    }
    
    #endregion

    public Vector3 OffsetUpVector(Vector3 position) => position + Vector3.up * 0.1f;

}
