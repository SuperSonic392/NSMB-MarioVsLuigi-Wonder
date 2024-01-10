using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AmpWalk : KillableEntity, IFreezableEntity
{
    private Vector3 origin;
    public GameObject elec;
    public override void Start()
    {
        base.Start();
        origin = transform.position;
    }
    // Update is called once per frame
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        elec.SetActive(!Frozen && !dead);
        if(!Frozen && !dead)
            transform.position = origin + (Vector3.up * Mathf.Sin(Time.time*2) * 0.15f);
    }
    public override void InteractWithPlayer(PlayerController player)
    {
        if (player.Frozen)
            return;

        player.groundpound = false;
        if (player.goomba)
        {
            player.photonView.RPC(nameof(PlayerController.Powerdown), RpcTarget.All, false);
        }
        else
        {
            player.photonView.RPC(nameof(PlayerController.PlayerShock), RpcTarget.All, false);
        }
    }
    public override void InteractWithPlayerSpin(PlayerController player)
    {
        if (player.Frozen)
            return;

        player.groundpound = false;
        player.Spinning = false;
        if (player.goomba)
        {
            player.photonView.RPC(nameof(PlayerController.Powerdown), RpcTarget.All, false);
        }
        else
        {
            player.photonView.RPC(nameof(PlayerController.PlayerShock), RpcTarget.All, false);
        }
    }

    [PunRPC]
    public override void Unfreeze(byte reasonByte)
    {
        Frozen = false;
        animator.enabled = true;
        hitbox.enabled = true;
        audioSource.enabled = true;

        int shoulddie = reasonByte switch
        {
            (byte)IFreezableEntity.UnfreezeReason.Groundpounded => 1,
            _ => 0
        };
        if(shoulddie > 0)
        {
            SpecialKill(false, false, 0);
        }
    }

    [PunRPC]
    public override void Freeze(int cube)
    {
        base.Freeze(cube);
        elec.SetActive(false);
    }
}
