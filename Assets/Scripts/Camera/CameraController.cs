using System.Collections.Generic;
using UnityEngine;

using NSMB.Utils;
using JetBrains.Annotations;

public class CameraController : MonoBehaviour
{ 

    //refactor stuff
    public float offset; //used for lookahead
    public float lastFloor;
    public Vector3 currentPosition;
    private static readonly float groundOffset = .5f;
    public bool IsControllingCamera { get; set; } = false;

    //ipod... please...
    private static readonly Vector2 airOffset = new(0, .65f);

    public static float ScreenShake = 0;

    private Vector2 airThreshold = new(0.5f, 1.3f), groundedThreshold = new(0.5f, 0f);
    private readonly List<SecondaryCameraPositioner> secondaryPositioners = new();
    private PlayerController controller;
    private Vector3 smoothDampVel, playerPos;
    private Camera targetCamera;
    private float startingZ;

    public void Awake()
    {
        //only control the camera if we're the local player.
        targetCamera = Camera.main;
        startingZ = targetCamera.transform.position.z;
        controller = GetComponent<PlayerController>();
        targetCamera.GetComponentsInChildren(secondaryPositioners);
    }

    public void LateUpdate()
    {
        if (controller.dead)
        {
            return;
        }
        PositionCameraNew(); CalculateNewPosition();
        if (IsControllingCamera) //this looks mostly ok
        {
            Vector3 shakeOffset = Vector3.zero;
            if ((ScreenShake -= Time.deltaTime) > 0 && controller.onGround)
                shakeOffset = new Vector3((Random.value - 0.5f) * ScreenShake, (Random.value - 0.5f) * ScreenShake);

            targetCamera.transform.position = currentPosition + shakeOffset;

            if (BackgroundLoop.Instance)
                BackgroundLoop.Instance.Reposition();

            secondaryPositioners.RemoveAll(scp => scp == null);
            secondaryPositioners.ForEach(scp => scp.UpdatePosition());
        }
#if false //old, pre refactor stuff. should be deleted
        currentPosition = CalculateNewPosition();
        if (IsControllingCamera) {

            Vector3 shakeOffset = Vector3.zero;
            if ((ScreenShake -= Time.deltaTime) > 0 && controller.onGround)
                shakeOffset = new Vector3((Random.value - 0.5f) * ScreenShake, (Random.value - 0.5f) * ScreenShake);

            targetCamera.transform.position = currentPosition + shakeOffset;
            if (BackgroundLoop.Instance)
                BackgroundLoop.Instance.Reposition();

            secondaryPositioners.RemoveAll(scp => scp == null);
            secondaryPositioners.ForEach(scp => scp.UpdatePosition());
        }
#endif
    }
    public void PositionCameraNew()
    {
        if (!controller.spawned)
        {
            currentPosition = controller.transform.position;
            lastFloor = currentPosition.y;
            currentPosition.z = startingZ;
            return;
        }
        float minY = GameManager.Instance.cameraMinY, heightY = GameManager.Instance.cameraHeightY;
        float minX = GameManager.Instance.cameraMinX, maxX = GameManager.Instance.cameraMaxX;
        float vOrtho = targetCamera.orthographicSize;
        float xOrtho = vOrtho * targetCamera.aspect;
        if (controller.pipeEntering)
        {
            offset = 0;
            if (controller.pipeEntering.instant)
            {
                currentPosition = controller.transform.position;
            }
            else
            {
                currentPosition.x = Mathf.Lerp(currentPosition.x, transform.position.x, Time.deltaTime * 5);
                currentPosition.y = Mathf.Lerp(currentPosition.y, transform.position.y, Time.deltaTime * 5);
            }
            currentPosition.z = startingZ;
            SetLastFloor();
            currentPosition.x = Mathf.Clamp(currentPosition.x, minX + xOrtho, maxX - xOrtho);
            currentPosition.y = Mathf.Clamp(currentPosition.y, minY + vOrtho, heightY == 0 ? (minY + vOrtho) : (minY + heightY - vOrtho));
            if (Utils.WrapWorldLocation(ref playerPos))
            {
                Debug.Log("loop");
                float xDifference = Vector2.Distance(Vector2.right * currentPosition.x, Vector2.right * playerPos.x);
                bool right = currentPosition.x > playerPos.x;
                if (xDifference >= 8)
                {
                    currentPosition.x += (right ? -1 : 1) * GameManager.Instance.levelWidthTile / 2f;
                    if (IsControllingCamera)
                        BackgroundLoop.Instance.wrap = true;
                }
            }
            return;
        }
        offset += controller.body.velocity.x * Time.deltaTime / 5;
        offset = Mathf.Clamp(offset, -.25f, .25f);
        currentPosition.x = controller.transform.position.x + offset;
        if (controller.onGround)
        {
            SetLastFloor();
        }
        if (controller.transform.position.y < currentPosition.y - vOrtho + 2)
        {
            lastFloor = controller.transform.position.y + vOrtho - 2;
        }
        if (controller.transform.position.y > currentPosition.y + vOrtho - 3)
        {
            lastFloor = controller.transform.position.y - vOrtho + 3;
        }
        if (controller.flying)
        {
            lastFloor = controller.transform.position.y;
            if (controller.drill)
                currentPosition.y = Mathf.Lerp(currentPosition.y, lastFloor, Time.deltaTime * 2);
        }
        if (controller.transform.position.y < currentPosition.y - vOrtho + 1f)
        {
            currentPosition.y = controller.transform.position.y + vOrtho - 1f;
        }
        if (controller.transform.position.y > currentPosition.y + vOrtho - 1f)
        {
            currentPosition.y = controller.transform.position.y - vOrtho + 1f;
        }
        currentPosition.y = Mathf.Lerp(currentPosition.y, lastFloor, Time.deltaTime * 3);
        currentPosition.x = Mathf.Clamp(currentPosition.x, minX + xOrtho, maxX - xOrtho);
        currentPosition.y = Mathf.Clamp(currentPosition.y, minY + vOrtho, heightY == 0 ? (minY + vOrtho) : (minY + heightY - vOrtho));

        currentPosition.z = startingZ;
        if (Utils.WrapWorldLocation(ref playerPos))
        {
            Debug.Log("loop");
            float xDifference = Vector2.Distance(Vector2.right * currentPosition.x, Vector2.right * playerPos.x);
            bool right = currentPosition.x > playerPos.x;
            if (xDifference >= 8)
            {
                currentPosition.x += (right ? -1 : 1) * GameManager.Instance.levelWidthTile / 2f;
                if (IsControllingCamera)
                    BackgroundLoop.Instance.wrap = true;
            }
        }
    }
    public void Recenter()
    {
        currentPosition = (Vector2)transform.position + airOffset;
        smoothDampVel = Vector3.zero;
        LateUpdate();
        float minY = GameManager.Instance.cameraMinY, heightY = GameManager.Instance.cameraHeightY;
        float minX = GameManager.Instance.cameraMinX, maxX = GameManager.Instance.cameraMaxX;
        float vOrtho = targetCamera.orthographicSize;
        float xOrtho = vOrtho * targetCamera.aspect;
        currentPosition.x = Mathf.Clamp(currentPosition.x, minX + xOrtho, maxX - xOrtho);
        currentPosition.y = Mathf.Clamp(currentPosition.y, minY + vOrtho, heightY == 0 ? (minY + vOrtho) : (minY + heightY - vOrtho));
    }
    public void SetLastFloor()
    {
        lastFloor = controller.transform.position.y + groundOffset;
    }
    private Vector3 CalculateNewPosition()
    { //chopped up and mutilated to serve the sole purpose of wrapping the background
        float minY = GameManager.Instance.cameraMinY, heightY = GameManager.Instance.cameraHeightY;
        float minX = GameManager.Instance.cameraMinX, maxX = GameManager.Instance.cameraMaxX;

        if (!controller.dead)
            playerPos = AntiJitter(transform.position);

        float vOrtho = targetCamera.orthographicSize;
        float xOrtho = vOrtho * targetCamera.aspect;

        // instant camera movements. we dont want to lag behind in these cases

        float playerHeight = controller.WorldHitboxSize.y;
        Utils.WrapWorldLocation(ref playerPos);
        float xDifference = Vector2.Distance(Vector2.right * currentPosition.x, Vector2.right * playerPos.x);
        bool right = currentPosition.x > playerPos.x;

        if (xDifference >= 8)
        {
            currentPosition.x += (right ? -1 : 1) * GameManager.Instance.levelWidthTile / 2f;
            if (IsControllingCamera)
                BackgroundLoop.Instance.wrap = true;
        }
        return Vector3.zero;
    }
    private void OnDrawGizmos()
    { //ok, this is good. although outdated
        if (!controller)
            return;

        Gizmos.color = Color.blue;
        Vector2 threshold = controller.onGround ? groundedThreshold : airThreshold;
        Gizmos.DrawWireCube(playerPos, threshold * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new(playerPos.x, lastFloor), Vector3.right / 2);
    }

    private static Vector2 AntiJitter(Vector3 vec)
    { //you shouldn't need this
        vec.y = ((int)(vec.y * 100)) / 100f;
        return vec;
    }
}
