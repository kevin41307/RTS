using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class ExplorerMaskEraser : MonoBehaviour
{
    public static ExplorerMaskEraser Instance;

    private ObjectPooler<ExplorerEraseMask> eraserPool;
    [SerializeField] private ExplorerEraseMask eraserPrefab;

    private void Awake()
    {
        Instance = this;
        eraserPool = new ObjectPooler<ExplorerEraseMask>();
        eraserPool.Initialize(20, eraserPrefab);

    }
    public void ClearUnitRenderTexture(Vector3 position, Vector3 scale)
    {
        ExplorerEraseMask eraser = eraserPool.GetNew();
        if (eraser == null) return;
        eraser.transform.position = position;
        eraser.SetBrushSize(scale);
        StartCoroutine(Clean(eraser));
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    IEnumerator Clean(ExplorerEraseMask eraser)
    {
        yield return 0;
        yield return 0;
        yield return 0;
        yield return 0;
        yield return 0;
        yield return 0;
        yield return 0;
        yield return 0;
        yield return 0;
        yield return 0;
        yield return 0;

        yield return 0;
        yield return 0;
        yield return 0;
        yield return 0;
        yield return 0;
        yield return 0;
        yield return 0;
        yield return 0;
        yield return 0;


        eraserPool.Free(eraser);
    }

}
