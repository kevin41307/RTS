using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText = null;
    private RTSPlayer player;

    private void Start()
    {
        if (((RTSNetworkManager)NetworkManager.singleton).DEBUG_MODE == false)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            ClientHandleResourcesUpdated(player.GetResources());
            player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
        }

    }

    private void Update()
    {
        try
        {
            if (player == null)
            {
                player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
                if (player != null)
                {
                    ClientHandleResourcesUpdated(player.GetResources());
                    player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
                }
            }

        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }

    }


    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(int newReources)
    {
        resourcesText.text = "Resources: " + newReources.ToString();
    }

}
