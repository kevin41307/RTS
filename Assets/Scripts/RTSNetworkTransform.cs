using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class RTSNetworkTransform : NetworkTransform
{

    public enum UpdateMode
    {
        fast,
        normal,
        low,
        stop
    }

    public void EnableSyncTransform()
    {
        syncPosition = true;
        syncRotation = true;
        
        //syncScale = true;
    }
    public void DisableSyncTransform()
    {
        syncPosition = false;
        syncRotation = false;
        //syncScale = false;  
    }
    
}
