using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class WireNet : MonoBehaviour
{
    private Collider2D coll;
    public GameObject MetalEffect;
    private float metalEffectTimer;
    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(PlayerController con in FindObjectsOfType<PlayerController>())
        {
            if(!con.netable)
                con.netable = coll.OverlapPoint(con.transform.position);
        }
        if(GameManager.Instance.currentWonderEffect == GameManager.WonderEffect.Metal)
        {
            metalEffectTimer -= Time.fixedDeltaTime;
            MetalEffect.SetActive(metalEffectTimer <= 0);
        }
        else
        {
            metalEffectTimer = 2;
            MetalEffect.SetActive(false);
        }
    }
}
