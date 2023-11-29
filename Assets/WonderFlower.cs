using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NSMB.Utils;

public class WonderFlower : MonoBehaviourPun
{
    private void Start()
    {
        UIUpdater.Instance.CreateFlowerIcon(gameObject);
    }
    public GameObject findFlowerID(int id)
    {
        foreach(WonderFlower w in FindObjectsOfType<WonderFlower>())
        {
            if(id == w.GetComponent<PhotonView>().ViewID)
            {
                return w.gameObject;
            }
        }
        return null;
    }
}
