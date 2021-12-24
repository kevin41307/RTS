using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorerEraseMask : MonoBehaviour, IPooled<ExplorerEraseMask>
{
    public int poolID { get; set; }
    public ObjectPooler<ExplorerEraseMask> pool { get; set; }

    [SerializeField] private Transform child;
    public void SetBrushSize(Vector3 newScale) => child.transform.localScale = newScale;

}
