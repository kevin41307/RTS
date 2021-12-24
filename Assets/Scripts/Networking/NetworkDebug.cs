using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
[DefaultExecutionOrder(-1)]
public class NetworkDebug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RTSNetworkManager.ClientOnConnected += HandleClinetOnConnected;
        NetworkManager.singleton.StartHost();
    }

    private void OnDestroy() 
    {
        RTSNetworkManager.ClientOnConnected -= HandleClinetOnConnected;       
    }
    void HandleClinetOnConnected()
    {
        //((RTSNetworkManager)NetworkManager.singleton).SetIsGameInProgress(true);
    }

}
