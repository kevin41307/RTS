using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Test : NetworkBehaviour
{

    public override void OnStartServer()
    {
        GetComponent<Renderer>().material.SetColor("_BaseColor", Random.ColorHSV(0, 1f, 0.9f, 0.9f, 0, 1f));
    }
}
