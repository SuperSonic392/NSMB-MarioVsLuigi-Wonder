using UnityEngine;
using Photon.Pun;

public abstract class HoldableEntity : KillableEntity {

    public PlayerController holder, previousHolder;
    public Vector3 holderOffset, selfOffset;
    public bool canPlace = true;
    public float wakeupTimer = 5;

    #region Unity Methods
    public void LateUpdate() {
        if (!holder)
            return;
        Vector3 off = selfOffset;
        off.x *= holder.transform.localScale.x;
        off.y *= holder.transform.localScale.y;
        off.z *= holder.transform.localScale.z;
        body.velocity = Vector2.zero;
        body.position = transform.position = holder.transform.position + holderOffset + (off * (holder.state == Enums.PowerupState.Small ? 0.5f : 1f));
        return;
    }

    public override void FixedUpdate() {
        if (!holder)
            base.FixedUpdate();
    }
    #endregion

    #region PunRPCs
    [PunRPC]
    public abstract void Kick(bool fromLeft, float kickFactor, bool groundpound);

    [PunRPC]
    public abstract void Throw(bool facingLeft, bool crouching, Vector2 pos);

    [PunRPC]
    public abstract void Toss(bool facingLeft, bool crouching, Vector2 pos);

    [PunRPC]
    public virtual void Pickup(int view) {
        if (holder)
            return;

        PhotonView holderView = PhotonView.Find(view);
        holder = holderView.gameObject.GetComponent<PlayerController>();
        previousHolder = holder;
        photonView.TransferOwnership(holderView.Owner);
    }

    [PunRPC]
    public override void Kill() {
        if (dead)
            return;

        if (holder)
            holder.SetHolding(-1);

        base.Kill();
    }

    [PunRPC]
    public override void SpecialKill(bool right, bool groundpound, int combo) {
        if (dead)
            return;

        if (holder)
            holder.SetHolding(-1);

        base.SpecialKill(right, groundpound, combo);
    }
    #endregion
}
