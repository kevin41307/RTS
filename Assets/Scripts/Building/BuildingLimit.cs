using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class BuildingLimit : MonoBehaviour
{
    [SerializeField] private GameObject buildingLimitParent = null;
    private Renderer m_renderer;
    private Material mat;
    [SerializeField] private float radius = 8f;
    private float localScaleReciprocal = 0.5f;
    private void Awake()
    {
        m_renderer = GetComponent<Renderer>();
        mat = m_renderer.material;
        mat.SetFloat("_Radius", radius * localScaleReciprocal);
        buildingLimitParent.gameObject.SetActive(false);

    }
    private void Start() // execution order is slower than eventSystem-Invoke
    {
    }
    public void Show()
    {
        buildingLimitParent.SetActive(true);
    }
    public void Hidden()
    {
        buildingLimitParent.SetActive(false);
    }

}
