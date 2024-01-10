using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricShock : MonoBehaviourPun
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerStay2D(collision);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if(player != null && player.shockTimer <= 0 && !player.mtl)
            player.photonView.RPC(nameof(PlayerController.PlayerShock), RpcTarget.All, false);
    }
}
