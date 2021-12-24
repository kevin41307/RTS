using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
public class Explorer : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private GameObject paintMask;

    public event Action OnExplorerPositionUpdated;
    public event Action<Explorer> OnExplorerExited;

    private GridXZ<GridFogOfWarSystem.FowObject> grid;
    private float tolerence;
    private Vector3 lastReportedPosition = default;

    public float GetRadius() => radius; //meter
    public Vector3 GetEraseMaskLocalScale() => paintMask.transform.localScale;

    private void Start()
    {
        grid = GridFogOfWarSystem.Instance.GetGrid();
        tolerence = grid.GetCellSize();
        lastReportedPosition = transform.position;
        GridFogOfWarSystem.Instance.TriggerExplorerEntered(this);

        paintMask.SetActive(true);
    }

    private void OnDisable()
    {
        ExplorerMaskEraser.Instance.ClearUnitRenderTexture(transform.position, GetEraseMaskLocalScale());
    }

    private void OnDestroy()
    {
        OnExplorerExited?.Invoke(this);
    }


    private void Update()
    {
        if((transform.position - lastReportedPosition ).sqrMagnitude > tolerence * tolerence)
        {
            OnExplorerPositionUpdated?.Invoke(); //impact performance
            lastReportedPosition = transform.position;
        }

        
    }

}
