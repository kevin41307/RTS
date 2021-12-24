using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
public class FogOfWarHider : MonoBehaviour
{
    [System.Serializable]
    public class HiderObject
    {
        public event System.Action<GridFogOfWarSystem.FowObject.FogOfWarSprite> OnStatusUpdated;
        [SerializeField] internal Transform transform;
        [SerializeField] internal Vector3Int gridedPosition;
        [SerializeField] private GridFogOfWarSystem.FowObject.FogOfWarSprite status ;

        public void SetStatus(GridFogOfWarSystem.FowObject.FogOfWarSprite nextStatus)
        {
            status = nextStatus;
            OnStatusUpdated?.Invoke(status);
        }
        public void SetGridedPosition<T>(GridXZ<T> grid)
        {
            grid.GetXZ(transform.position, out int x, out int z);
            gridedPosition = new Vector3Int(x, 0, z);
        }
            
    }
    //Components
    [SerializeField] private List<HiderObject> hiderObjectList;
    [SerializeField] private GameObject staticVisual;
    [SerializeField] private GameObject dynamicVisual;
    public UnityEvent OnRevealedObject = new UnityEvent();
    public UnityEvent OnHiddenObject = new UnityEvent();
    //Params
    private GridXZ<GridFogOfWarSystem.FowObject> grid;
    private Vector3 lastCheckPosition;
    private float tolerence;

    [ClientCallback]
    private void Start()
    {
        grid = GridFogOfWarSystem.Instance.GetGrid();
        grid.OnGridObjectChanged += Grid_OnGridObjectChanged;
        foreach (var hider in hiderObjectList)
        {
            if (hider.transform == null) continue;
            /*
            grid.GetXZ(hider.transform.position, out int x, out int z);
            hider.gridedPosition = new Vector3Int(x, 0, z);
            */
            hider.SetGridedPosition(grid);
            hider.OnStatusUpdated += UpdateVisual;
            hider.SetStatus(GridFogOfWarSystem.FowObject.FogOfWarSprite.Cleared);
        }

        if(hiderObjectList[0] != null)
            grid.TriggerGridObjectChanged(hiderObjectList[0].gridedPosition.x, hiderObjectList[0].gridedPosition.z);

        lastCheckPosition = transform.position;
        tolerence = grid.GetCellSize();

    }

    [ClientCallback]
    private void OnDestroy()
    {
        //Debug.Log("OnDestroy");
        if (grid == null) return;
        grid.OnGridObjectChanged -= Grid_OnGridObjectChanged;
        foreach (var hider in hiderObjectList)
        {
            hider.OnStatusUpdated -= UpdateVisual;
        }
        if (hiderObjectList[0] != null)
            grid.TriggerGridObjectChanged(hiderObjectList[0].gridedPosition.x, hiderObjectList[0].gridedPosition.z);

    }
    [ClientCallback]
    private void Grid_OnGridObjectChanged(object sender, GridXZ<GridFogOfWarSystem.FowObject>.OnGridObjectChangedEventArgs eventData)
    {
        if (grid.ContainXZ(eventData.x, eventData.z) == false) return;
        foreach (var hider in hiderObjectList)
        {
            if (eventData.x == hider.gridedPosition.x && eventData.z == hider.gridedPosition.z)
            {
                GridFogOfWarSystem.FowObject fowObject = grid.GetGridObject(eventData.x, eventData.z);
                if(fowObject != null) // exceed range
                    hider.SetStatus(fowObject.GetFogOfWarSprite());
            }
        }  
    }
    [ClientCallback]
    private void UpdateVisual(GridFogOfWarSystem.FowObject.FogOfWarSprite fogOfWarSprite)
    {
        switch (fogOfWarSprite)
        {
            case GridFogOfWarSystem.FowObject.FogOfWarSprite.notExplored:
                Hidden();
                break;
            case GridFogOfWarSystem.FowObject.FogOfWarSprite.explored: 
                Reveal();
                break;
            case GridFogOfWarSystem.FowObject.FogOfWarSprite.revealed:
                Reveal();
                break;
            case GridFogOfWarSystem.FowObject.FogOfWarSprite.Cleared:
                Hidden();
                break;
            default:
                break;
        }

    }
    [ClientCallback]
    private void Reveal()
    {
        if(staticVisual != dynamicVisual)
        {
            if (staticVisual != null)
                staticVisual.SetActive(false);
            dynamicVisual.SetActive(true);
        }
        else
        {
            dynamicVisual.SetActive(true);
        }
        OnRevealedObject?.Invoke();

    }
    [ClientCallback]
    private void Hidden()
    {
        if (staticVisual != dynamicVisual)
        {
            if (staticVisual != null)
                staticVisual.SetActive(true);
            dynamicVisual.SetActive(false);
        }
        else
        {
            dynamicVisual.SetActive(false);
        }
        OnHiddenObject?.Invoke();
    }

    [ClientCallback]
    private void TriggerOnGridObjectChanged()
    {
        foreach (var hider in hiderObjectList)
        {
            Grid_OnGridObjectChanged(this, new GridXZ<GridFogOfWarSystem.FowObject>.OnGridObjectChangedEventArgs { x = hider.gridedPosition.x, z = hider.gridedPosition.z }); ; ;
        }
    }

    [ClientCallback]
    private void UpdateHider()
    {
        foreach (var hider in hiderObjectList)
        {
            hider.SetGridedPosition(grid);
        }
    }

    [ClientCallback]
    private void Update()
    {
        if((lastCheckPosition - transform.position).sqrMagnitude > tolerence * tolerence)
        {
            TriggerOnGridObjectChanged();
            UpdateHider();
            lastCheckPosition = transform.position;
        }
    }
#if UNITY_EDITOR
#endif

}
