using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NSMB.Utils;

public class WonderFlower : MonoBehaviourPun
{
    public bool collected;
    public float collectTimer;
    public Animator anim;
    public Collider2D box;
    private void Update()
    {
        if (collectTimer > 0)
        {
            collectTimer -= Time.deltaTime;
            if(collectTimer <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
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
    public void CollectFlower()
    {
        if(collected) //woah bucko! we shouldn't even be able to do that. 
        {
            return; //hopefully this code should never be reached.
        }
        box.enabled = false;
        anim.SetTrigger("get");
        collected = true;
        collectTimer = 1f;
    }
}
