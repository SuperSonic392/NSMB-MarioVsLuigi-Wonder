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
    public List<float> rots;
    public List<float> spds;
    public List<string> anims;
    public PlayerController target;
    private void Start() //set fresnel color to player photon id
    {
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
            Debug.LogError("No Target!");
            return;
        }
        if(!target.gameObject.activeSelf)
        {
            return;
        }
        Animator targetAnim = target.gameObject.GetComponent<Animator>();
        poss.Add(target.transform.position);
        rots.Add(target.AnimationController.models.transform.rotation.eulerAngles.y);
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
        if (poss.Count >= delay && rots.Count >= delay && spds.Count >= delay && anims.Count >= delay)
        {
            transform.position = new Vector3(poss[0].x, poss[0].y, -2);
            poss.RemoveAt(0);
            transform.rotation = Quaternion.Euler(0, rots[0], 0);
            rots.RemoveAt(0);
            if (transform.rotation.eulerAngles.y < 180f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x),transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            me.speed = spds[0];
            spds.RemoveAt(0);
            me.Play(anims[0], 0);
            anims.RemoveAt(0);
        }
        //last ditch effort to maintain
        if(poss.Count >= delay)
        {
            poss.RemoveRange(delay, poss.Count - delay);
        }
        if(rots.Count >= delay)
        {
            rots.RemoveRange(delay, rots.Count - delay);
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
