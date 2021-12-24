using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Targetable : NetworkBehaviour
{
    [SerializeField] private Transform aimAtPoint = null;
    [SerializeField] private List<Targeter> chaserList = new List<Targeter>();


    public Transform GetAimPoint()
    {
        return aimAtPoint;
    }
    [Server]
    public void AddChaser(Targeter chaser)
    {
        if(!chaserList.Contains(chaser))
            chaserList.Add(chaser);
    }
    [Server]
    public void RemoveChaser(Targeter chaser)
    {
        chaserList.Remove(chaser);
    }

    public void Disappear()
    {
        for (int i = chaserList.Count - 1; i >= 0 ; i--)
        {
            if(chaserList[i] != null)
                chaserList[i].ClearTargets();
        }
        chaserList.Clear();
    }

}
