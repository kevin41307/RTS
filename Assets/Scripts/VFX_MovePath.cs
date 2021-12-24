using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class VFX_MovePath : NetworkBehaviour
{
    [SerializeField] private GameObject pathParent;
    [SerializeField] private LineRenderer pathRenderer;
    private Vector3[] positions = new Vector3[2];
    private bool isPathFinding = true;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        pathParent.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (isPathFinding)
            UpdateStartPosition(transform.position);   
    }


    /// <summary>
    /// position in world space
    /// </summary>
    public void SetPath(Vector3 startPosition, Vector3 endPosition) 
    {
        if (!hasAuthority) return;
        positions[0] = startPosition;
        positions[1] = endPosition;
        pathRenderer.SetPositions(positions);
        isPathFinding = true;
    }
    public void UpdateStartPosition(Vector3 startPosition)
    {
        if (!hasAuthority) return;
        positions[0] = startPosition;
        pathRenderer.SetPositions(positions);
    }

    public void ClearPath()
    {
        if (!hasAuthority) return;
        positions[0] = Vector3.zero;
        positions[1] = Vector3.zero;
        pathRenderer.SetPositions(positions);
        isPathFinding = false;
    }  

}
