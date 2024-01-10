using UnityEngine;

public class Move : MonoBehaviour //WHY ARE THERE NO FUCKING COMMENTS!?
{
    //Hyper from Wonder speaking, this code sucks and should not be examined. it's from HMM after all.
    public Vector2 setVelocity;
    private Rigidbody2D body;
    private PlayerController controller;
    public bool FixtoX, fixtoY;
    FireballMover fireballMover;
    public bool playInsertSound, playExitSound;
    public AudioClip InsertSound, ExitSound;
    public AudioSource source;
    public LayerMask mask;
    private void FixedUpdate()
    {

        source = GetComponent<AudioSource>();
        if (body != null)
        {
            body.velocity = setVelocity;

            if (FixtoX)
                body.transform.position = new Vector2(transform.position.x, body.transform.position.y);
            if (fixtoY)
                body.transform.position = new Vector2(body.transform.position.x, transform.position.y);
        }

        if (controller != null)
        {
            controller.zoomtube = true;
            controller.Spinning = false;
            controller.jumpHeld = false;
            controller.groundpound = false;
            controller.crouching = false;
            controller.drill = false;
            controller.flying = false;
            controller.doGroundSnap = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, mask))
        {
            body = collision.GetComponent<Rigidbody2D>();
            controller = collision.GetComponent<PlayerController>();
            if (playInsertSound && body != null)
            {
                source = GetComponent<AudioSource>();
                source.clip = InsertSound;
                source.Play();
            }
        }
        
    }
    public bool IsInLayerMask(GameObject obj, LayerMask layerMask) //https://www.w3schools.blog/check-object-is-in-layermask-unity
    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    } //but why?

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(IsInLayerMask(collision.gameObject, mask))
        {
            body = null;
            if (controller != null)
            {
                controller.zoomtube = false;
                controller.jumping = true;
                controller.triplejump = false;
                controller.doublejump = false;
            }
            controller = null;
            if (playExitSound)
            {
                source = GetComponent<AudioSource>();
                source.clip = ExitSound;
                source.Play();
            }
        }
    }
    public float yvel;
    private void GizmoSimulate()
    {
        Vector3 rememberpos = endpos;
        yvel -= 1.5f;
        endpos = new Vector3(endpos.x + setVelocity.x/25, endpos.y + yvel/25, endpos.z);
        Gizmos.DrawLine(rememberpos, endpos);
    }
    public Vector3 endpos;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        endpos = transform.position;
        yvel = setVelocity.y;
        for (int i = 0; i < 12; i++)
        {
             GizmoSimulate();
        }


    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < 15; i++)
        {
            GizmoSimulate();
        }
    }
}
