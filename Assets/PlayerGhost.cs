using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using NSMB.Utils;
using Photon.Pun;

public class PlayerGhost : MonoBehaviour
{
    public int delay;
    public Animator me;
    public List<Vector2> poss;
    public List<bool> rights;
    public List<float> spds;
    public List<string> anims;
    public PlayerController target;
    private void Start() //set fresnel color to player photon id
    {
        if (!target)
        {
            return;
        }
        foreach (SkinnedMeshRenderer render in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            foreach (Material mat in render.materials)
            {
                mat.SetColor("_Tint", Utils.GetPlayerColor(target.photonView.Owner));
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!target)
        {
            return;
        }
        if (target.DoesHaveBadge(PlayerController.WonderBadge.JetRun))
        {
            delay = 30;
        }
        else
        {
            delay = 50;
        }
        if (!target.gameObject.activeSelf || target.pipeEntering)
        {
            me.speed = 0;
            return;
        }
        transform.localScale = target.transform.localScale;
        Animator targetAnim = target.gameObject.GetComponent<Animator>();
        poss.Add(target.transform.position);
        rights.Add(target.facingRight);
        spds.Add(targetAnim.GetCurrentAnimatorStateInfo(0).speed * targetAnim.GetCurrentAnimatorStateInfo(0).speedMultiplier * targetAnim.speed); //this is just so i don't have to do anything special in the dummy animator
        AnimatorClipInfo[] clipInfo = targetAnim.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            anims.Add(clipInfo[0].clip.name.ToString());
        }
        else
        {
            anims.Add(anims[anims.Count-1]); //just take the last one we used
        }
        if (poss.Count >= delay && rights.Count >= delay && spds.Count >= delay && anims.Count >= delay)
        {
            transform.position = new Vector3(poss[0].x, poss[0].y, -2);
            poss.RemoveAt(0);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (rights[0] ? 1 : -1), transform.localScale.y, transform.localScale.z);
            rights.RemoveAt(0);
            me.speed = spds[0];
            spds.RemoveAt(0);
            me.Play(anims[0], 0);
            anims.RemoveAt(0);
        }
        //last ditch effort to maintain order
        if(poss.Count >= delay)
        {
            poss.RemoveRange(delay, poss.Count - delay);
        }
        if(rights.Count >= delay)
        {
            rights.RemoveRange(delay, rights.Count - delay);
        }
        if (spds.Count >= delay)
        {
            spds.RemoveRange(delay, spds.Count - delay);
        }
        if (anims.Count >= delay)
        {
            anims.RemoveRange(delay, anims.Count - delay);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerController con = collision.GetComponent<PlayerController>();
        if (con != null && con.photonView.IsMine)
        {
            con.photonView.RPC(nameof(PlayerController.Powerdown), RpcTarget.All, false);
        }
    }
}
