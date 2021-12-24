using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointerController : MonoBehaviour
{
    [SerializeField] private VFX_MoveUnit VFX_MoveUnitPrefab;
    [SerializeField] private LayerMask blockLayer = new LayerMask();
    private ObjectPooler<VFX_MoveUnit> VFX_MoveUnitPool;
    private Camera mainCamera;
    private void Awake() 
    {
        VFX_MoveUnitPool = new ObjectPooler<VFX_MoveUnit>();
        VFX_MoveUnitPool.Initialize(8, VFX_MoveUnitPrefab);
        mainCamera = Camera.main;

    }

    private void Update() 
    {
        if(Mouse.current.rightButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if(!Physics.Raycast(ray, out RaycastHit hit, 100f, blockLayer)) return;

            VFX_MoveUnit vfx = VFX_MoveUnitPool.GetNew(VFX_MoveUnitPrefab.expiredTime);
            if(vfx == null) return;
            vfx.transform.SetPositionAndRotation(hit.point + Vector3.up * 0.1f, Quaternion.Euler(hit.normal));
        }
    }

}
