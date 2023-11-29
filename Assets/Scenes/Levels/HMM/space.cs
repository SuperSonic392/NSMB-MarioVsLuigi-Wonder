using UnityEngine;
using UnityEngine.InputSystem.XR;

public class space : MonoBehaviour
{
    public GameManager manager;
    private PlayerController controller;
    public space myself;
    public float divideGrav = 4;
    // Update is called once per frame
    void Update()
    {
        controller = manager.localPlayer.GetComponent<PlayerController>();
        if(controller != null)
        {
            controller.normalGravity = controller.normalGravity / divideGrav;
            controller.slowriseGravity = controller.slowriseGravity / divideGrav;
            controller.flyingGravity = controller.flyingGravity / divideGrav;
            Destroy(myself);
        }
    }
}
