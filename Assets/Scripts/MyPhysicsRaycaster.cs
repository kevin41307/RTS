using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyPhysicsRaycaster : MonoBehaviour
{
    [SerializeField] private LayerMask blockLayer = new LayerMask();
    private Camera m_Camera;
    //RaycastHit[] raycastHits = new RaycastHit[10];
    Ray ray;

    private void Start()
    {
        m_Camera = Camera.main;
    }

    private void Update()
    {
        ray = m_Camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if(Physics.Raycast(ray, out RaycastHit hit, 100f, blockLayer, QueryTriggerInteraction.Ignore))
        {
            Debug.Log(hit.transform.name);
        }
    }
}
