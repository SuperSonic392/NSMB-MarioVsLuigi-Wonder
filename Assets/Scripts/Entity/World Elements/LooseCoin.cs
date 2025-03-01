using UnityEngine;
using Photon.Pun;
using NSMB.Utils;

public class LooseCoin : MonoBehaviourPun {

    public float despawn = 10;

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private PhysicsEntity physics;
    private Animator animator;
    private BoxCollider2D hitbox;
    private AudioSource sfx;
    private Vector2 prevFrameVelocity;
    private float despawnTimer;
    public Transform attractionTarget;
    public bool Collected { get; set; }

    public void Start() {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        hitbox = GetComponent<BoxCollider2D>();
        physics = GetComponent<PhysicsEntity>();
        animator = GetComponent<Animator>();
        sfx = GetComponent<AudioSource>();
        body.velocity = Vector2.up * Random.Range(2f, 3f);
    }

    public void FixedUpdate() {
        if (GameManager.Instance && GameManager.Instance.gameover) {
            body.velocity = Vector2.zero;
            animator.enabled = false;
            body.isKinematic = true;
            return;
        }
        bool inWall = Utils.IsAnyTileSolidBetweenWorldBox(body.position + hitbox.offset, hitbox.size * transform.lossyScale * 0.5f);
        gameObject.layer = inWall ? Layers.LayerHitsNothing : Layers.LayerLooseCoin;

        physics.UpdateCollisions();
        if (physics.onGround) {
            body.velocity -= body.velocity * Time.fixedDeltaTime;
            if (physics.hitRoof && photonView.IsMine)
                PhotonNetwork.Destroy(photonView);

            if (prevFrameVelocity.y < -1f) {
                sfx.PlayOneShot(Enums.Sounds.World_Coin_Drop.GetClip());
            }
        }

        spriteRenderer.enabled = !(despawnTimer > despawn-3 && despawnTimer % 0.3f >= 0.15f);

        if(attractionTarget != null)
        {
            Vector2 mov = (attractionTarget.position - transform.position).normalized;
            transform.LookAt((Vector2)attractionTarget.position);
            body.velocity = transform.forward * 50;
            transform.rotation = Quaternion.identity;
        }
        prevFrameVelocity = body.velocity;

        if ((despawnTimer += Time.deltaTime) >= despawn) {
            if (photonView.IsMine)
                PhotonNetwork.Destroy(photonView);
            return;
        }
    }

    public void OnDrawGizmos() {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(body.position + hitbox.offset, hitbox.size * transform.lossyScale);
    }

    [PunRPC]
    public void ApplyForce(float forcex, float forcey)
    {
        body.velocity += new Vector2(forcex, forcey);
    }
}
