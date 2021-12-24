using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarCollider : MonoBehaviour
{
    [SerializeField] private FogOfWarVisual fogOfWarVisual;
    MeshCollider meshCollider;
    private void Start()
    {
        //mesh = fogOfWarVisual.GetMesh();
        fogOfWarVisual.OnUpdatedMeshVisual += FogOfWarVisual_OnUpdatedMeshVisual;
        meshCollider = GetComponent<MeshCollider>();
    }
    private void OnDestroy()
    {
        fogOfWarVisual.OnUpdatedMeshVisual -= FogOfWarVisual_OnUpdatedMeshVisual;
    }

    private void FogOfWarVisual_OnUpdatedMeshVisual(Mesh mesh)
    {

        meshCollider.sharedMesh = mesh;
    }
}
