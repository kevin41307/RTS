using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
public class GridFogOfWarSystem : MonoBehaviour
{
    public static GridFogOfWarSystem Instance;

    [SerializeField] private FogOfWarVisual fogOfWarVisual;
    [SerializeField] private Vector3 pivot;
    [SerializeField] private List<Explorer> explorerList;
    private int gridEdge = 50;
    private float gridMeter = 100f;
    private GridXZ<FowObject> grid;
    
    public GridXZ<FowObject> GetGrid() => grid;
    private bool updateFog = false;

    private void Awake()
    {
        Instance = this;
        //  如果cellSize不是整數值, 會少一小段
        grid = new GridXZ<FowObject>(gridEdge, gridEdge, gridMeter / gridEdge, pivot, (GridXZ<FowObject> g, int x, int y) => new FowObject(g, x, y));
        
    
    }

    private void Update()
    {
        if (updateFog == true)
        {
            CalculateFogStuff();
            updateFog = false;
        }
    }

    public void TriggerExplorerEntered(Explorer explorer)
    {
        if (!explorerList.Contains(explorer))
        {
            explorerList.Add(explorer);
            Explorer_OnExplorerPositionUpdated();
            explorer.OnExplorerPositionUpdated += Explorer_OnExplorerPositionUpdated;
            explorer.OnExplorerExited += Explorer_OnExplorerExited;
        }
    }
    private void Explorer_OnExplorerExited(Explorer explorer)
    {
        explorer.OnExplorerPositionUpdated -= Explorer_OnExplorerPositionUpdated;
        explorer.OnExplorerExited -= Explorer_OnExplorerExited;
        explorerList.Remove(explorer);
        Explorer_OnExplorerPositionUpdated();
    }
    private void Explorer_OnExplorerPositionUpdated()
    {
        updateFog = true;
    }

    private void CalculateFogStuff()
    {
        ClearFogOfWarInfo();
        for (int i = explorerList.Count - 1; i >= 0; i--)
        {
            if (explorerList[i] == null) continue;
            CalCulateRevealeadPart(explorerList[i]);
        }
        CalculateExploredPart();
        grid.TriggerAllGridObjectChanged();
    }


    private void ClearFogOfWarInfo()
    {
        FowObject fowObject = null; //TODO: remember each explorer last frame revealed grid, ignore clear it
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int z = 0; z < grid.GetHeight(); z++)
            {
                fowObject = grid.GetGridObject(x, z);
                fowObject.SetFogOfWarSprite(FowObject.FogOfWarSprite.Cleared, true);
            }
        }
    }
    private void CalCulateRevealeadPart(Explorer explorer)
    {
        float radius = explorer.GetRadius();
        int radiusInGridScale = Mathf.CeilToInt(radius / grid.GetCellSize());
        grid.GetXZ(explorer.transform.position, out int centerX, out int centerZ);
        Vector2 centerXZ = new Vector2(centerX, centerZ);
        //Debug.Log("centerXZ" + centerXZ + "radiusInGridScale" + radiusInGridScale);
        FowObject fowObject = null;
        for (int x = centerX - radiusInGridScale; x <= centerX + radiusInGridScale; x++)
        {
            for (int z = centerZ - radiusInGridScale; z <= centerZ + radiusInGridScale; z++)
            {
                if (x >= 0 && z >= 0 && x < grid.GetWidth() && z < grid.GetWidth())
                {
                    fowObject = grid.GetGridObject(x, z);
                    //Debug.Log("X" + x + "Z" + z + "grid.GetCenterWorldPosition(x, z)" + grid.GetCenterWorldPosition(x, z));
                    if ((grid.GetCenterWorldPosition(x, z) - explorer.transform.position).sqrMagnitude < radius * radius)
                    {
                        //FowObject fowObject = grid.GetGridObject(x, z);
                        fowObject.SetFogOfWarSprite(FowObject.FogOfWarSprite.revealed, true);
                    }
                }

            }
        }
    }
    private void CalculateExploredPart()
    {
        FowObject fowObject = null;
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int z = 0; z < grid.GetHeight(); z++)
            {
                fowObject = grid.GetGridObject(x, z);
                if (fowObject.GetFogOfWarSprite() == FowObject.FogOfWarSprite.revealed) continue;
                if (fowObject.IsRevealed)
                {
                    fowObject.SetFogOfWarSprite(FowObject.FogOfWarSprite.explored, true);
                } else {
                    fowObject.SetFogOfWarSprite(FowObject.FogOfWarSprite.notExplored, true);
                }    
            }
        }
    }

    private void Start()
    {
        fogOfWarVisual.SetGrid(this.grid);
    }

    public class FowObject
    {
        public enum FogOfWarSprite
        {
            notExplored,
            explored,
            revealed,
            Cleared
        }

        private GridXZ<FowObject> grid;
        private int x;
        private int z;
        public bool IsRevealed { get; private set; } = false;
        private FogOfWarSprite fogOfWarSprite = default;

        public FowObject(GridXZ<FowObject> grid, int x, int z )
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetFogOfWarSprite(FogOfWarSprite fogOfWarSprite, bool mute = false)
        {
            this.fogOfWarSprite = fogOfWarSprite;
            if(fogOfWarSprite == FogOfWarSprite.revealed)
            {
                IsRevealed = true;
            }
            if (mute == false)
            {
                grid.TriggerGridObjectChanged(x, z);
            }
            
        }
        public FogOfWarSprite GetFogOfWarSprite()
        {
            return fogOfWarSprite;
        }

        public override string ToString()
        {
            return x + ", " + z + "\n";
        }
    }
}