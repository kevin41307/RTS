using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AutoClearRT : MonoBehaviour
{
    Camera m_Camera;
    CameraClearFlags cameraClearFlags;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GetComponent<Camera>();
        RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
        cameraClearFlags = m_Camera.clearFlags;
        m_Camera.clearFlags = CameraClearFlags.Color;


    }
    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext arg1, Camera arg2)
    {
        OnPostRender();
    }


    private void OnPostRender()
    {
        Debug.Log("ClearRT");
        m_Camera.clearFlags = cameraClearFlags;
        RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
    }
}
