using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class FogOfWarVisual : MonoBehaviour
{
    [System.Serializable]
    public struct FogOfWarSpriteUV
    {
        public GridFogOfWarSystem.FowObject.FogOfWarSprite fogOfWarSprite;
        public Vector2Int uv00Pixels;
        public Vector2Int uv11Pixels;
    }
    private struct UVCoords
    {
        public Vector2 uv00;
        public Vector2 uv11;
    }

    [SerializeField] private bool DEBUG_MODE = false;
    
    [SerializeField] private FogOfWarSpriteUV[] fogOfWarSpriteUVArray;
    GridXZ<GridFogOfWarSystem.FowObject> grid;
    private Mesh mesh;
    private bool updateMesh;
    private Dictionary<GridFogOfWarSystem.FowObject.FogOfWarSprite, UVCoords> uvCoordsDictionary;
    public event System.Action<Mesh> OnUpdatedMeshVisual;
    Vector3[] vertices;
    Vector2[] uv;
    int[] triangles;
    private void Awake()
    {
        mesh = new Mesh();
        mesh.MarkDynamic();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().enabled = DEBUG_MODE;

        Texture texture = GetComponent<MeshRenderer>().material.mainTexture;
        float textureWidth = texture.width;
        float textureHeight = texture.height;

        uvCoordsDictionary = new Dictionary<GridFogOfWarSystem.FowObject.FogOfWarSprite, UVCoords>();
        foreach (FogOfWarSpriteUV fogOfWarSpriteUV in fogOfWarSpriteUVArray)
        {
            uvCoordsDictionary[fogOfWarSpriteUV.fogOfWarSprite] = new UVCoords
            {
                uv00 = new Vector2(fogOfWarSpriteUV.uv00Pixels.x / textureWidth, fogOfWarSpriteUV.uv00Pixels.y / textureHeight),
                uv11 = new Vector2(fogOfWarSpriteUV.uv11Pixels.x / textureWidth, fogOfWarSpriteUV.uv11Pixels.y / textureHeight)
            };
        }
        
    }
    private void Start()
    {
        
    }

    public void SetGrid(GridXZ<GridFogOfWarSystem.FowObject> grid)
    {
        this.grid = grid;
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out vertices, out uv, out triangles);
        UpdateHeatMapVisual();

        grid.OnGridObjectChanged += Grid_OnGridObjectChanged;
    }

    private void Grid_OnGridObjectChanged(object sender, GridXZ<GridFogOfWarSystem.FowObject>.OnGridObjectChangedEventArgs e)
    {
        updateMesh = true;
    }

    private void LateUpdate()
    {
        if (updateMesh)
        {
            updateMesh = false;
            UpdateHeatMapVisual();
        }
    }

    private void UpdateHeatMapVisual()
    {

        //MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();
                GridFogOfWarSystem.FowObject gridObject = grid.GetGridObject(x, y);
                GridFogOfWarSystem.FowObject.FogOfWarSprite fogOfWarSprite = gridObject.GetFogOfWarSprite();
                Vector2 gridUV00, gridUV11;
                //if(Random.Range(0, 2) == 0 )
                if (fogOfWarSprite == GridFogOfWarSystem.FowObject.FogOfWarSprite.Cleared)
                {
                    Debug.Log("FogOfWarSprite.Cleared in x:" + x + "z:" + y);
                }
                if (fogOfWarSprite == GridFogOfWarSystem.FowObject.FogOfWarSprite.revealed)
                {
                    gridUV00 = Vector2.zero;
                    gridUV11 = Vector2.zero;
                    quadSize = Vector2.zero;
                }
                else
                {
                    UVCoords uvCoords = uvCoordsDictionary[fogOfWarSprite];       
                    gridUV00 = uvCoords.uv00;
                    gridUV11 = uvCoords.uv11;
                    //Debug.Log("uvCoords.uv00: " + uvCoords.uv00 + "uvCoords.uv11: " + uvCoords.uv11);
                }
                Vector3 meshQuadSize = new Vector3(quadSize.x, 0, quadSize.y);
                Vector3 pos = grid.GetWorldPosition(x, y) + meshQuadSize * .5f;
                MeshUtils.AddToMeshArraysXZ(vertices, uv, triangles, index, pos, 0f, meshQuadSize, gridUV00, gridUV11);

            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        OnUpdatedMeshVisual?.Invoke(mesh);

        //Array.Clear() TODO?
    }
}